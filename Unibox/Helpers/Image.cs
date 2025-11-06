using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Unibox.Helpers
{
    public static class Image
    {
        // Loads an image from disk into memory and closes the file so it can be replaced.
        // Uses OnLoad and freezes the BitmapImage.
        public static ImageSource UnlockedImageCopy(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return null;

            try
            {
                byte[] bytes = File.ReadAllBytes(filePath);
                using var ms = new MemoryStream(bytes);
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad; // loads into memory so stream can be closed
                // DO NOT set CreateOptions = BitmapCreateOptions.IgnoreImageCache when using StreamSource
                bmp.StreamSource = ms;
                bmp.EndInit();
                bmp.Freeze(); // safe to share across threads
                return bmp;
            }
            catch
            {
                return null;
            }
        }
    }
}