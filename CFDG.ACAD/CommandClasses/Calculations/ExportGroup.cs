using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using CivilApp = Autodesk.Civil.ApplicationServices.CivilApplication;

namespace CFDG.ACAD.CommandClasses.Calculations
{
    public class ExportGroup : ICommandMethod
    {
        [CommandMethod("ExportGroup")]
        public void InitialCommand()
        {
            PointGroupCollection pointGroups = CivilApp.ActiveDocument.PointGroups;
            UI.windows.Calculations.ExportPointGroup exportPointGroup = new UI.windows.Calculations.ExportPointGroup(pointGroups);
        }
    }
}
