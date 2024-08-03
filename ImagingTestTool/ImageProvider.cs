namespace Test
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Image = System.Drawing.Image;

#nullable disable

    public class ImageProvider : IImageProvider
    {
        private ImageCodecInfo jpegCodec;

        public ImageProvider()
        {
            this.jpegCodec = Array.Find(ImageCodecInfo.GetImageEncoders(), codec => codec.FormatID == ImageFormat.Jpeg.Guid);
        }

        public Task<ImageData> GetImageAsync(Uri url)
        {
            return this.GetImageAsync(url, -1, -1);
        }

        public async Task<ImageData> GetImageAsync(Uri url, int width, int height)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    ImageData imageData = new ImageData();
                    byte[] imageBuffer = await response.Content.ReadAsByteArrayAsync();

                    // For performance reasons of the thumbnail server we will convert any non-JPEG image to JPEG.
                    using (MemoryStream inputMemoryStream = new MemoryStream(imageBuffer))
                    {
                        using Image image = Image.FromStream(inputMemoryStream);

                        int bitmapWidth = width == -1 ? image.Width : width;
                        int bitmapHeight = height == -1 ? image.Height : height;

                        float imageHorizontalResolution = image.HorizontalResolution > 0 ? image.HorizontalResolution : 72.0f;
                        float imageVerticalResolution = image.VerticalResolution > 0 ? image.VerticalResolution : 72.0f;

                        using Bitmap bitmap = new Bitmap(bitmapWidth, bitmapHeight);
                        bitmap.SetResolution(imageHorizontalResolution, imageVerticalResolution);

                        using (Graphics graphics = Graphics.FromImage(bitmap))
                        {
                            // Use white background
                            graphics.Clear(Color.White);

                            // A width and height of zero means not to scale the image. We only scale down, never up. There is
                            // no aspect ratio check here (all images we use are square).
                            if (image.Width > bitmapWidth && image.Height > bitmapHeight)
                            {
                                // Use highest quality scaling.
                                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                graphics.DrawImage(image, 0, 0, bitmapWidth, bitmapHeight);
                            }
                            else
                            {
                                graphics.DrawImageUnscaled(image, 0, 0);
                            }
                        }

                        using MemoryStream outputMemoryStream = new MemoryStream();
                        EncoderParameters encoderParameters = new EncoderParameters(1);
                        EncoderParameter compressionLevel = new EncoderParameter(Encoder.Quality, 100L);
                        encoderParameters.Param[0] = compressionLevel;

                        bitmap.Save(outputMemoryStream, this.jpegCodec, encoderParameters);
                        imageData.ImageBuffer = outputMemoryStream.GetBuffer();
                        imageData.Width = bitmapWidth;
                        imageData.Height = bitmapHeight;
                    }

                    if (response.Content.Headers != null)
                    {
                        imageData.LastModified = response.Content.Headers.LastModified;
                    }

                    return imageData;
                }
            }

            return null;
        }

        public async Task<DateTimeOffset?> GetLastModifiedAsync(Uri url)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            using (HttpClient httpClient = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, url);
                HttpResponseMessage response = await httpClient.SendAsync(request);

                if (response == null || !response.IsSuccessStatusCode || response.Content == null || response.Content.Headers == null || response.Content.Headers.LastModified == null)
                {
                    return null;
                }
                else
                {
                    return response.Content.Headers.LastModified;
                }
            }
        }

        public async Task<Size> GetImageSizeAsync(Uri url)
        {
            using var client = new HttpClient();
            var getResponse = await client.GetAsync(url);

            if (getResponse == null || !getResponse.IsSuccessStatusCode)
            {
                return Size.Empty;
            }

            using var imgStream = await getResponse.Content.ReadAsStreamAsync();
            var img = Image.FromStream(imgStream);
            return img.Size;
        }
    }

#nullable enable
}
