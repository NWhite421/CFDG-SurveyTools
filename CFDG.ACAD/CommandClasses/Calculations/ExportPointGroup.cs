using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using AcApplication = Autodesk.AutoCAD.ApplicationServices.Application;
using CivilApp = Autodesk.Civil.ApplicationServices.CivilApplication;

namespace CFDG.ACAD.CommandClasses.Calculations
{
    public class ExportPointGroup : ICommandMethod
    {
        [CommandMethod("ExportPointGroup")]
        public void InitialCommand()
        {
            GetPointGroupCollection();
        }

        private void GetPointGroupCollection()
        {
            PointGroupCollection pgCollection;
            using (Transaction AcTran = AcApplication.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
            {
                pgCollection = CivilApp.ActiveDocument.PointGroups;
                UI.windows.Calculations.ExportPointGroup exportPointGroup = new UI.windows.Calculations.ExportPointGroup(pgCollection);
                AcTran.Commit();
            }
        }
    }
}
