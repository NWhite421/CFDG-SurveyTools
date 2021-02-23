using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFDG.API
{
    public struct Triangle
    {
        public Triangle(double hypotenuse, double angle, bool isInputRadians)
        {
            if (!isInputRadians)
            {
                new Triangle(hypotenuse, angle);
            }

            SideC = hypotenuse;
            AngleA = angle;
            AngleB = 180 - (AngleA + 90);
            SideA = Math.Cos(angle) * hypotenuse;
            SideB = Math.Sin(angle) * hypotenuse;
        }

        public Triangle(double hypotenuse, double angle)
        {
            SideC = hypotenuse;
            AngleA = angle;
            AngleB = 180 - (AngleA + 90);
            SideA = Math.Cos((Math.PI / 180) * angle) * hypotenuse;
            SideB = Math.Sin((Math.PI / 180) * angle) * hypotenuse;
        }

        public double SideA { get; set; }
        public double SideB { get; set; }
        public double SideC { get; set; }
        public double AngleA { get; set; }
        public double AngleB { get; set; }
        public double AngleC { get { return 90; } }

        private double ConvertToRadian(double degrees)
        {
            return (Math.PI / 180) * degrees;
        }

        private double ConvertToDegrees(double radian)
        {
            return (180 / Math.PI) * radian;
        }
    }
}
