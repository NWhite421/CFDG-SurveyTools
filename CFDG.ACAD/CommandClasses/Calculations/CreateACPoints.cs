using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApplication = Autodesk.AutoCAD.ApplicationServices.Application;

namespace CFDG.ACAD.CommandClasses.Calculations
{
    public class CreateACPoints : ICommandMethod
    {
        [CommandMethod("CreateACPoints", CommandFlags.Modal | CommandFlags.NoPaperSpace)]
        public void InitialCommand()
        {
            Document AcDoc = AcApplication.DocumentManager.MdiActiveDocument;
            Editor AcEdit = AcDoc.Editor;
            Database AcDb = AcDoc.Database;

            while (true)
            {
                Point3d point = UserInput.SelectPointInDoc("Please select a point: ");

                if (point.X == -1 && point.Y == -1)
                {
                    return;
                }
                UserInput.AddPointToDrawing(point, BlockTableRecord.ModelSpace);
            }
        }
    }
}
