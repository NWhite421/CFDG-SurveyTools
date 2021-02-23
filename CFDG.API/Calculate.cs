using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFDG.API
{
    public class Calculate
    {
        public static Triangle GetTriangle(double distance, double angle)
        {
            return new Triangle(distance, angle);
        }
    }
}
