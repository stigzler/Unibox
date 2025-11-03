using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Unibox.Helpers
{
    internal static class Image
    {
        internal static ImageSource UnlockedImageCopy(string filePath)
        {
            // 1. Create a BitmapImage instance
            BitmapImage bitmap = new BitmapImage();

            // 2. Begin initialization sequence
            bitmap.BeginInit();

            // 3. Set the CacheOption to OnLoad
            // This tells WPF to immediately load the entire image into memory
            // and close the underlying file stream, preventing the file lock.
            bitmap.CacheOption = BitmapCacheOption.OnLoad;

            // 4. Set the URI source
            bitmap.UriSource = new Uri(filePath, UriKind.Absolute);

            // 5. End initialization sequence
            bitmap.EndInit();

            // 6. Freeze the object for efficiency (optional, but recommended in WPF)
            bitmap.Freeze();

            return bitmap;
        }
    }
}