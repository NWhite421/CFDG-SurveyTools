using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace CFDG.ACAD.Functions
{
    class Imaging
    {
        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }


        public static BitmapImage BitmapToImageSource(params Bitmap[] bitmaps)
        {
            if (bitmaps.Length == 0)
                return new BitmapImage();
            int width = bitmaps.Max(map => map.Width);
            int height = bitmaps.Max(map => map.Height);
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                foreach (Bitmap map in bitmaps)
                {
                    g.DrawImage(map, System.Drawing.Point.Empty);
                }
            }
            return BitmapToImageSource(result);
        }
    }
}
