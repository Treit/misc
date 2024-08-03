namespace Test
{
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Processing;
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Size = System.Drawing.Size;

#nullable disable

    public class ImageSharpImageProvider : IImageProvider
    {
        private static readonly HttpClient s_httpClient;

        static ImageSharpImageProvider()
        {
            s_httpClient = new HttpClient();
        }

        public ImageSharpImageProvider()
        {
        }

        public Task<ImageData> GetImageAsync(Uri url)
        {
            return GetImageAsync(url, -1, -1);
        }

        public async Task<ImageData> GetImageAsync(Uri url, int width, int height)
        {
            ArgumentNullException.ThrowIfNull(url, nameof(url));

            var response = await s_httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode || response.Content is null)
            {
                return null;
            }

            var imageData = new ImageData();
            var imageBuffer = await response.Content.ReadAsByteArrayAsync();

            // For performance reasons of the thumbnail server we will convert any non-JPEG image to JPEG.
            using var image = Image.Load(imageBuffer);

            int bitmapWidth = width == -1 ? image.Width : width;
            int bitmapHeight = height == -1 ? image.Height : height;

            var newImage = new Image<Rgba32>(bitmapWidth, bitmapHeight);

            newImage.Mutate(x => x.BackgroundColor(Color.White));

            if (image.Width > bitmapWidth && image.Height > bitmapHeight)
            {
                var resizeOptions = new ResizeOptions
                {
                    Mode = ResizeMode.Stretch,
                    Size = new SixLabors.ImageSharp.Size(bitmapWidth, bitmapHeight),
                    Sampler = KnownResamplers.Bicubic
                };

                image.Mutate(x => x.Resize(resizeOptions));
            }

            // Draw the original image onto the new image
            newImage.Mutate(x => x.DrawImage(image, new Point(0, 0), 1));

            using MemoryStream outputMemoryStream = new MemoryStream();
            newImage.SaveAsJpeg(outputMemoryStream);

            imageData.ImageBuffer = outputMemoryStream.GetBuffer();
            imageData.Width = bitmapWidth;
            imageData.Height = bitmapHeight;
            imageData.LastModified = response.Content?.Headers?.LastModified;

            return imageData;
        }

        public async Task<DateTimeOffset?> GetLastModifiedAsync(Uri url)
        {
            ArgumentNullException.ThrowIfNull(url, nameof(url));

            using var request = new HttpRequestMessage(HttpMethod.Head, url);
            using var response = await s_httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

                return response.Content?.Headers?.LastModified;
        }

        public async Task<Size> GetImageSizeAsync(Uri url)
        {
            ArgumentNullException.ThrowIfNull(url, nameof(url));

            using var getResponse = await s_httpClient.GetAsync(url);

            if (!getResponse.IsSuccessStatusCode)
            {
                return Size.Empty;
            }

            using var imageStream = await getResponse.Content.ReadAsStreamAsync();
            var image = Image.Load(imageStream);
            var imageSize = image.Size();
            return new Size(imageSize.Width, imageSize.Height);
        }
    }

#nullable enable
}
