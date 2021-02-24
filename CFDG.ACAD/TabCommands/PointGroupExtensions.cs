using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;

namespace CFDG.ACAD.TabCommands
{
    public static class PointGroupExtensions
    {
        public static List<CogoPoint> GetCogoPoints(this PointGroup ptGroup)
        {
            CivilDocument civDoc = CivilApplication.ActiveDocument;
            return (from p in ptGroup.GetPointNumbers()
                    select civDoc.CogoPoints.GetPointByPointNumber(p).GetObject(OpenMode.ForRead) as CogoPoint).ToList();
        }
    }
}
