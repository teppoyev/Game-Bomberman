using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Game_Bomberman
{
    class HelpfulFunctions
    {
        public static System.Windows.Media.Brush BitmapToBrush(Bitmap bitmap)
        {
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return new ImageBrush(bitmapSource);
        }

        public static double AbsoluteCoord(double coord)
        {
            return Game_Logic.Object.standartSize * coord;
        }

        public static double AbsoluteCoord(double coord, double size)
        {
            return size * coord;
        }

        public static int RelativeCoord(double coord)
        {
            double a_ = coord / Game_Logic.Object.standartSize;
            if (a_ - Math.Round(a_) >= 0) return Convert.ToInt32(Math.Round(a_));
            else return Convert.ToInt32(Math.Round(a_)) - 1;
        }

        public static int RelativeCoord(double coord, double size)
        {
            double a_ = coord / size;
            if (a_ - Math.Round(a_) >= 0) return Convert.ToInt32(Math.Round(a_));
            else return Convert.ToInt32(Math.Round(a_)) - 1;
        }
    }
}
