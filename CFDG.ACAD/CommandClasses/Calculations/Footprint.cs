using System;
using System.Linq;
using System.Text.RegularExpressions;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using CFDG.API;
using AcApplication = Autodesk.AutoCAD.ApplicationServices.Application;

namespace CFDG.ACAD.CommandClasses.Calculations
{
    //TODO!: Change to create a bunch of lines.
    public class Footprint : ICommandMethod
    {
        #region Internal Properties

        public static Point3d CurrentPoint { get; set; }
        public static double CurrentAngle { get; set; }


        #endregion


        [CommandMethod("Footprint", CommandFlags.Modal | CommandFlags.NoPaperSpace)]
        public void InitialCommand()
        {

            try
            {
                Document AcDocument = AcApplication.DocumentManager.MdiActiveDocument;
                Editor AcEditor = AcDocument.Editor;
                CurrentPoint = UserInput.SelectPointInDoc("Select a start point: ");
                CurrentAngle = UserInput.SelectAngleInDoc("Select a start angle: ", CurrentPoint);
                if (!EstablishLine(CurrentPoint, CurrentAngle))
                {
                    return;
                }
                while (true)
                {
                    if (!AddSide()) //If I don't add a side, consider the command to want to exit.
                    {
                        break;
                    }
                }
                //TODO: Add misclosure calculations;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        private static bool EstablishLine(Point3d start, double angle)
        {
            (var acDocument, var acEditor) = UserInput.GetCurrentDocSpace();
            var distanceStr = UserInput.GetStringFromUser("Enter the distance: ");
            if (!double.TryParse(distanceStr, out double distance))
            {
                if (!(distance > 0))
                {
                    return false;
                }
            }
            Triangle triangle = new Triangle(distance, angle);
            Point3d endPoint = new Point3d(start.X + triangle.SideA, start.Y + triangle.SideB, start.Z);
            CreateLine(start, endPoint);
            CurrentPoint = endPoint;
            return true;
        }

        private static bool AddSide()
        {
            double angle, distance;

            string distanceStr = UserInput.GetStringFromUser("Enter a distance for the side: ");
            if (string.IsNullOrEmpty(distanceStr))
            {
                return false;
            }
            Match match = Regex.Match(distanceStr, @"^(-?)(\+?)\d+(\.\d+)?(@\d+)?$");
            if (!match.Success)
            {
                return false;
            }
            if (distanceStr.Contains('@'))
            {
                string[] parts = distanceStr.Split('@');
                angle = double.Parse(parts[0]) > 0 ? double.Parse(parts[1]) * -1 : double.Parse(parts[1]);
                distance = Math.Abs(double.Parse(parts[0]));
            }
            else
            {
                angle = double.Parse(distanceStr) > 0 ? -90 : 90;
                distance = Math.Abs(double.Parse(distanceStr));
            }
            Point3d endPoint = GetPointFromAngle(angle, distance);
            CreateLine(CurrentPoint, endPoint);
            CurrentPoint = endPoint;
            CurrentAngle += angle;
            return true;
        }



        private static Point3d GetPointFromAngle(double angle, double distance)
        {
            double newAngle = CurrentAngle + angle;
            var measures = new Triangle(distance, newAngle);

            return new Point3d(CurrentPoint.X + measures.SideA, CurrentPoint.Y + measures.SideA, CurrentPoint.Z);
        }

        private static void CreateLine(Point3d start, Point3d end)
        {
            Line line = new Line(start,end);
            UserInput.AddObjectToDrawing(line);
        }
    }
}
