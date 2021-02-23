using Autodesk.AutoCAD.ApplicationServices;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace CFDG.ACAD.Functions
{
    class DocumentProperties
    {
        public static string GetJobNumber(Document document)
        {
            var jobNumber = Path.GetFileNameWithoutExtension(document.Name);
            return Parse(jobNumber);
        }

        public static string GetJobNumber(string document)
        {
            var jobNumber = Path.GetFileNameWithoutExtension(document);
            return Parse(jobNumber);
        }

        /// <summary>
        /// Parses a file name to determine if the job number is in the filename.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string Parse(string fileName)
        {
            var match = Regex.Match(fileName, API.XML.ReadValue("General", "DefaultProjectNumber"));
            if (match.Success)
            {
                return match.Value;
            }
            return "";
        }
    }
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
