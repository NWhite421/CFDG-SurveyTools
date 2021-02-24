using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using CFDG.API;
using AcApplication = Autodesk.AutoCAD.ApplicationServices.Application;

namespace CFDG.ACAD
{
    public class Footprint
    {
        private static (Polyline, Point2d) EstablishLine(Point3d start, double angle)
        {
            Document AcDocument = AcApplication.DocumentManager.MdiActiveDocument;
            Editor AcEditor = AcDocument.Editor;
            Database AcDatabase = AcDocument.Database;
            double distance;


            while (true)
            {
                string distanceStr = UserInput.GetStringFromUser("Enter a distance for the side: ");
                if (string.IsNullOrEmpty(distanceStr))
                {
                    return (null, new Point2d(-1,-1));
                }
                Match match = Regex.Match(distanceStr, @"^\d+(.\d+)?$");
                if (!match.Success)
                {
                    AcEditor.WriteMessage($"\n{ distanceStr } is not a valid input. Please try again.\n");
                } 
                else
                {
                    distance = double.Parse(distanceStr);
                    break;
                }
            }

            Triangle triangle = new Triangle(distance, angle);
            Polyline polyline;
            Point2d newPoint;

            using (Transaction DbTrans = AcDatabase.TransactionManager.StartTransaction())
            {

                BlockTable blkTbl = (BlockTable)DbTrans.GetObject(AcDatabase.BlockTableId, OpenMode.ForRead);
                BlockTableRecord blkTblRcd = (BlockTableRecord)DbTrans.GetObject(blkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                polyline = new Polyline();
                polyline.SetDatabaseDefaults();
                polyline.AddVertexAt(0, new Point2d(start.X, start.Y), 0, 0, 0);
                newPoint = new Point2d(start.X + triangle.SideA, start.Y + triangle.SideB);
                polyline.AddVertexAt(1, newPoint, 0, 0, 0);
                blkTblRcd.AppendEntity(polyline);
                DbTrans.AddNewlyCreatedDBObject(polyline, true);
                DbTrans.Commit();
            }

            return (polyline, newPoint);
        }

        private static (Point2d, double) AddSide(Polyline polyline, double currentAngle, Point2d currentPoint)
        {
            Document AcDocument = AcApplication.DocumentManager.MdiActiveDocument;
            Editor AcEditor = AcDocument.Editor;
            Database AcDatabase = AcDocument.Database;

            string side = UserInput.GetStringFromUser("Enter a distance for the side: ");

            while (true)
            {
                if (string.IsNullOrEmpty(side))
                {
                    return (new Point2d(0, 0), -1);
                }
                Match match = Regex.Match(side, @"^-?\d+(.\d+)?(@\d+)?$");
                if (!match.Success)
                {
                    AcEditor.WriteMessage($"\n{ side } is not a valid input. Please try again.\n");
                }
                else
                {
                    double distance;
                    double angle;
                    if (side.Contains('@'))
                    {
                        string[] parts = side.Split('@');
                        angle = double.Parse(parts[0]) > 0 ? double.Parse(parts[1]) * -1 : double.Parse(parts[1]);
                        distance = Math.Abs(double.Parse(parts[0]));
                    }
                    else
                    {
                        angle = double.Parse(side) > 0 ? -90 : 90;
                        distance = Math.Abs(double.Parse(side));
                    }
                    (Vector2d deltas, double newAngle) = GetVectorsFromAngle(currentAngle, angle, distance);
                    angle = newAngle;

                    using (Transaction AcDbTransaction = AcDatabase.TransactionManager.StartTransaction())
                    {
                        BlockTable blkTbl = (BlockTable)AcDbTransaction.GetObject(AcDatabase.BlockTableId, OpenMode.ForRead);
                        BlockTableRecord blkTblRcd = (BlockTableRecord)AcDbTransaction.GetObject(blkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                        var Polyobject = (Polyline)AcDbTransaction.GetObject(polyline.ObjectId, OpenMode.ForWrite);
                        currentPoint = new Point2d(currentPoint.X + deltas.X, currentPoint.Y + deltas.Y);
                        Polyobject.AddVertexAt(Polyobject.NumberOfVertices, new Point2d(currentPoint.X, currentPoint.Y), 0, 0, 0);

                        AcDbTransaction.Commit();
                    }
                    return (currentPoint, angle);
                } 
            }
        }

        private static (Vector2d deltas, double newAngle) GetVectorsFromAngle(double baseAngle, double angle, double distance)
        {
            double newAngle = baseAngle + angle;
            var measures = new Triangle(distance, newAngle);

            return (new Vector2d(measures.SideA, measures.SideB), newAngle);
        }

        [CommandMethod("Footprint", CommandFlags.Modal | CommandFlags.NoPaperSpace)]
        public static void EntryCommand()
        {

            Document AcDocument = AcApplication.DocumentManager.MdiActiveDocument;
            Editor AcEditor = AcDocument.Editor;
            Point3d startPoint = UserInput.SelectPointInDoc("Select a start point: ");
            double baseAngle = UserInput.SelectAngleInDoc("Select a start angle: ", startPoint);

            (Polyline line, Point2d currentPoint) = EstablishLine(startPoint, baseAngle);
            while (true)
            {
                (Point2d newPoint, double newAngle) = AddSide(line, baseAngle, currentPoint);
                if (newAngle == -1)
                {
                    break;
                }
                currentPoint = newPoint;
                baseAngle = newAngle;
            }
            Triangle triangle = new Triangle(new Point2d(startPoint.X, startPoint.Y), currentPoint);
            AcEditor.WriteMessage($"\nClosure: {triangle.SideC}\tDeltaX: {triangle.SideA}\tDeltaY: {triangle.SideB}\n");
        }
    }
}
