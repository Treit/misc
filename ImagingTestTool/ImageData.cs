namespace Test
{
    using System;
    using System.Drawing;
    using System.Threading.Tasks;

#nullable disable

    public class ImageData
    {
        public byte[] ImageBuffer;
        public int Width;
        public int Height;
        public DateTimeOffset? LastModified;
    }

    public interface IImageProvider
    {
        Task<ImageData> GetImageAsync(Uri url);

        Task<ImageData> GetImageAsync(Uri url, int width, int height);

        Task<DateTimeOffset?> GetLastModifiedAsync(Uri url);

        Task<Size> GetImageSizeAsync(Uri url);
    }

#nullable enable
}
