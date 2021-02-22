using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using AcApp = Autodesk.AutoCAD.ApplicationServices.Application;
using CFDG.API;

namespace CFDG.ACAD
{
    public class Calculations
    {
        #region Variables
        private static UI.SlopeDistance DistanceWindow;
        private static bool CheckForZeneth;
        #endregion

        [CommandMethod("GetSlopeFromPoints")]
        public static void GetSlopeFromPoints()
        {
            CheckForZeneth = true;
            Document doc = AcApp.DocumentManager.MdiActiveDocument;
            Point3d startPnt = SelectPointInDoc(doc, "Select your start point: ");
            Point3d endPnt = SelectPointInDoc(doc, "Select your end point: ", startPnt);

            if (startPnt == new Point3d(-1,-1,-1) || endPnt == new Point3d(-1, -1, -1))
            {
                return;
            }

            if (DistanceWindow == null)
            {
                UI.SlopeDistance distanceWin = new UI.SlopeDistance(startPnt, endPnt);
                DistanceWindow = distanceWin;
                AcApp.ShowModelessWindow(distanceWin);
                DistanceWindow.Closed += DistanceWindow_Closed;
            }
            DistanceWindow.Calculate(startPnt, endPnt);
        }

        private static Point3d SelectPointInDoc(Document doc, string message)
        {
            return SelectPointInDoc(doc, message, new Point3d(-1,-1,-1));
        }

        private static Point3d SelectPointInDoc(Document doc, string message, Point3d basePoint)
        {
            Editor AcEditor = doc.Editor;

            if (VerifyZenthValues(false))
            {
                PromptPointOptions ppo = new PromptPointOptions($"\n{message}")
                {
                    AllowArbitraryInput = true,
                    AllowNone = false
                };
                if (basePoint != new Point3d(-1,-1,-1))
                {
                    ppo.BasePoint = basePoint;
                    ppo.UseBasePoint = true;
                    ppo.UseDashedLine = true;
                }

                var pr = AcEditor.GetPoint(ppo);
                if (pr.Status != PromptStatus.Cancel)
                {
                    return pr.Value;
                }
                return new Point3d(-1,-1,-1);
            }
            return new Point3d(-1,-1,-1);
        }

        private static bool VerifyZenthValues(bool preferredValue)
        {
            Document doc = AcApp.DocumentManager.MdiActiveDocument;
            Editor AcEditor = doc.Editor;
            Database AcDatabase = doc.Database;
            bool OSnapZ = Convert.ToBoolean(AcApp.TryGetSystemVariable("OSnapZ"));

            if (preferredValue == OSnapZ || !CheckForZeneth)
            {
                CheckForZeneth = false;
                return true;
            }
            else
            {
                //FEATURE: Enable temporary disablement of AutoCAD variables.
                PromptKeywordOptions pkwo = new PromptKeywordOptions($"OSnapZ is {(preferredValue ? "disabled" : "enabled")}, Do you want to continue?");
                pkwo.Keywords.Add("Yes");
                pkwo.Keywords.Add("No");
                pkwo.Keywords.Default = "Yes";
                var KeywordResult = AcEditor.GetKeywords(pkwo);
                if (KeywordResult.StringResult == "No")
                {
                    CheckForZeneth = false;
                    return false;
                }
                else
                {
                    CheckForZeneth = false;
                    return true;
                }
            }
        }


        [CommandMethod("CreateMeasureDownPoints")]
        public static void CreateMeasureDownPoints()
        {

        }

        private static Point3d GetMeasureDownCoordinates(Point3d top, double distance, double angle)
        {
            Editor AcEditor = AcApp.DocumentManager.MdiActiveDocument.Editor;

            double drop = Math.Cos((Math.PI / 180) * angle) * distance;
            double offset = Math.Sin((Math.PI / 189) * angle) * distance;
            
            PromptAngleOptions pao = new PromptAngleOptions("Select a point for the measuredown: ")
            {
                AllowArbitraryInput = true,
                AllowNone = true,
                AllowZero = true,
                BasePoint = top,
                UseBasePoint = true,
                UseDashedLine = true,
                DefaultValue = 0,
                UseDefaultValue = true        
            };

            var ar = AcEditor.GetAngle(pao);
            if (ar.Status == PromptStatus.Cancel)
            {
                return new Point3d(-1, -1, -1);
            }
            if (ar.Status == PromptStatus.Keyword)
            {
                return new Point3d(-2, -2, -2);
            }

            double deltaXRaw = Math.Cos((Math.PI / 180) * angle) * distance;
            double DeltaYRaw = Math.Sin((Math.PI / 189) * angle) * distance;
            double deltaX = (angle > 90 && angle < 270) ? deltaXRaw*-1 : deltaXRaw;
            double deltaY = (angle > 180 && angle < 360) ? DeltaYRaw*-1 : DeltaYRaw;
            return new Point3d(top.X + deltaX, top.Y + deltaY, top.Z - drop);
        }

        private static void DistanceWindow_Closed(object sender, EventArgs e)
        {
            DistanceWindow = null;
        }

        internal static List<string> controlCodes = new List<string>
        {
            "IRC=",
            "IR=",
            "NL=",
            "NLD=",
            "CM=",
            "PIP=",
            "OM="
        };

        public static string BeautifyPointList(List<string> points)
        {
            string outp = "";
            int curPoint = 0;
            foreach (string point in points)
            {
                int pNumber = int.Parse(point);
                if (curPoint == 0)
                {
                    outp += pNumber;
                    curPoint = pNumber;
                }
                else if (int.Parse(points.Last()) == pNumber)
                {
                    outp += pNumber == curPoint + 1 && outp.Last() != '-' ? "-" : ", ";

                    outp += pNumber;
                }
                else if (pNumber == curPoint + 1)
                {
                    curPoint = pNumber;
                }
                else
                {
                    outp += "-" + curPoint + ", " + pNumber;
                    curPoint = pNumber;
                }
            }
            return outp;
        }

        /// <summary>
        /// Creates a point file from a selected point group.
        /// </summary>
        [CommandMethod("ExportPointGroups")]
        /*public static void ExportPointGroups()
        {
            Document acDoc = AcApp.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;
            Editor acEd = acDoc.Editor;
            CivilDocument cApp = Autodesk.Civil.ApplicationServices.CivilApplication.ActiveDocument;

            PointGroupCollection pointGroups;
            ExportPoints form;

            #region Job Number
            var jobNumber = Path.GetFileNameWithoutExtension(acDoc.Name);
            if (jobNumber.Length > 9) jobNumber = jobNumber.Remove(9);
            if (!API.JobNumber.TryParse(jobNumber))
            {
                acEd.WriteMessage("Job number could not be determined." + Environment.NewLine);
                return;
            }
            string path = API.JobNumber.GetPath(jobNumber);
            if (!Directory.Exists(path))
            {
                acEd.WriteMessage("Project folder could not be determined." + Environment.NewLine);
            }
            #endregion

            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {
                pointGroups = cApp.PointGroups;
                form = new ExportPoints(pointGroups, path);
                var fResult = AcApp.ShowModalDialog(form);

                if (fResult == DialogResult.Cancel) { acEd.WriteMessage("\nExport operation cancelled."); return; }
                acEd.WriteMessage("\nExport operation continued.");

                if (!Directory.Exists(form.FolderPath)) { Directory.CreateDirectory(form.FolderPath); }

                string file = form.FolderPath + "\\" + form.FileName.Trim();
                acEd.WriteMessage($"\nWriting {form.PointGroup} points to {file}");

                switch (form.PointGroup.ToLower())
                {
                    case "!all points":
                        {
                            foreach (ObjectId pointId in cApp.CogoPoints)
                            {
                                CogoPoint point = (CogoPoint)pointId.GetObject(OpenMode.ForRead);
                                string pointTxt = $"{point.PointNumber},{Math.Round(point.Northing, 4)},{Math.Round(point.Easting, 4)},{Math.Round(point.Elevation, 4)},{point.RawDescription}{Environment.NewLine}";
                                File.AppendAllText(file, pointTxt);
                            }
                            break;
                        }
                    case "!comp points":
                        {
                            foreach (ObjectId pointId in cApp.CogoPoints)
                            {
                                CogoPoint point = (CogoPoint)pointId.GetObject(OpenMode.ForRead);
                                int pnumber = Convert.ToInt32(point.PointNumber);
                                if (pnumber >= 1000 && pnumber < 10000)
                                {
                                    string pointTxt = $"{point.PointNumber},{Math.Round(point.Northing, 4)},{Math.Round(point.Easting, 4)},{Math.Round(point.Elevation, 4)},{point.RawDescription}{Environment.NewLine}";
                                    File.AppendAllText(file, pointTxt);
                                }
                            }
                            break;
                        }
                    case "!control points":
                        {
                            foreach (ObjectId pointId in cApp.CogoPoints)
                            {
                                CogoPoint point = (CogoPoint)pointId.GetObject(OpenMode.ForRead);
                                string description = point.RawDescription;
                                if (controlCodes.Any(s => description.Contains(s)))
                                {
                                    string pointTxt = $"{point.PointNumber},{Math.Round(point.Northing, 4)},{Math.Round(point.Easting, 4)},{Math.Round(point.Elevation, 4)},{point.RawDescription}{Environment.NewLine}";
                                    File.AppendAllText(file, pointTxt);
                                }
                            }
                            break;
                        }
                    default:
                        {
                            foreach (ObjectId pgId in pointGroups)
                            {
                                PointGroup group = (PointGroup)pgId.GetObject(OpenMode.ForRead);
                                if (group.Name.ToLower() == form.PointGroup.ToLower())
                                {
                                    var cogoPoints = group.GetCogoPoints();
                                    acEd.WriteMessage($"\nPoint Group {form.PointGroup} contains {cogoPoints.Count} points");
                                    foreach (CogoPoint point in cogoPoints)
                                    {
                                        string pointTxt = $"{point.PointNumber},{Math.Round(point.Northing, 4)},{Math.Round(point.Easting, 4)},{Math.Round(point.Elevation, 4)},{point.RawDescription}{Environment.NewLine}";
                                        File.AppendAllText(file, pointTxt);
                                    }
                                }
                            }

                            break;
                        }
                }
                acEd.WriteMessage($"\nPoints exported successfully.");
                form.Dispose();
                tr.Commit();
            };
        }*/

        /// <summary>
        /// Creates a point group with the proper name that 
        /// </summary>
        [CommandMethod("CreateGroupOfCalcs")]
        public static void GroupCalcPoints()
        {
            Document acDoc = AcApp.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;
            Editor adEd = acDoc.Editor;
            CivilDocument cApp = Autodesk.Civil.ApplicationServices.CivilApplication.ActiveDocument;

            List<string> points = new List<string> { };
            string pointStr = "";
            string descriptionStr = "";

            // Get the purpose of the points
            PromptStringOptions pStrOpts = new PromptStringOptions("\nEnter the purpose: ")
            {
                AllowSpaces = true
            };
            PromptResult pRlt = adEd.GetString(pStrOpts);

            // If the string was empty (or null somehow)
            if (string.IsNullOrEmpty(pRlt.StringResult)) { adEd.WriteMessage("\nThe string entered was empty, please try again."); return; }
            string purpose = pRlt.StringResult;

            //Selection method
            PromptKeywordOptions pKeyOpts = new PromptKeywordOptions("\nPlease select a method of point selection: ");
            pKeyOpts.Keywords.Add("List");
            pKeyOpts.Keywords.Add("Selection");
            pKeyOpts.Keywords.Add("Descriptions");
            pKeyOpts.Keywords.Default = "List";
            pKeyOpts.AllowNone = true;

            pRlt = adEd.GetKeywords(pKeyOpts);
            string selType = pRlt.StringResult;
            switch (selType)
            {
                case "List":
                    {
                        pStrOpts = new PromptStringOptions("\nEnter the point range using dashes (-) and commas (,): ")
                        {
                            AllowSpaces = true
                        };
                        pRlt = adEd.GetString(pStrOpts);
                        if (string.IsNullOrEmpty(pRlt.StringResult)) { adEd.WriteMessage("\nThe string entered was empty, please try again."); return; }
                        pointStr = pRlt.StringResult;

                        break;
                    }
                case "Selection":
                    {
                        TypedValue[] tvs = new TypedValue[]
                        {
                            new TypedValue((int)DxfCode.Start, "AECC_COGO_POINT")
                        };
                        SelectionFilter selFltr = new SelectionFilter(tvs);
                        PromptSelectionResult acSSPrompt;
                        acSSPrompt = adEd.GetSelection(selFltr);
                        if (acSSPrompt.Status == PromptStatus.Cancel) { adEd.WriteMessage("\nAction aborted."); return; }
                        if (acSSPrompt.Value.Count < 1) { adEd.WriteMessage("\nThe selectiond was empty, please try again."); return; }
                        using (Transaction tr = acDb.TransactionManager.StartTransaction())
                        {
                            foreach (var obj in acSSPrompt.Value.GetObjectIds())
                            {
                                CogoPoint pnt = (CogoPoint)obj.GetObject(OpenMode.ForRead);
                                points.Add(pnt.PointNumber.ToString());
                            }
                            points.Sort();
                        }
                        pointStr = BeautifyPointList(points);
                        break;
                    }
                case "Descriptions":
                    {
                        pStrOpts = new PromptStringOptions("\nEnter the raw descriptions including wildcards (*) and commas (,): ")
                        {
                            AllowSpaces = true
                        };
                        pRlt = adEd.GetString(pStrOpts);
                        if (string.IsNullOrEmpty(pRlt.StringResult)) { adEd.WriteMessage("\nThe string entered was empty, please try again."); return; }
                        descriptionStr = pRlt.StringResult;

                        break;
                    }
                default:
                    {
                        MessageBox.Show($"The selected input for GroupCalc Command was not valid: {pRlt.StringResult}");
                        return;
                    }
            }
            if (string.IsNullOrEmpty(pointStr) && string.IsNullOrEmpty(descriptionStr)) { adEd.WriteMessage("\nNo points were detected"); return; }
            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {
                //Establish points
                string groupName = $"[{DateTime.Now:MM-dd-yyyy}] {purpose.ToUpper()}";

                StandardPointGroupQuery query = new StandardPointGroupQuery();
                if (selType == "Descriptions")
                {
                    query.IncludeRawDescriptions = descriptionStr;
                }
                else
                {
                    query.IncludeNumbers = pointStr;
                }

                PointGroupCollection pointGroups = cApp.PointGroups;
                if (pointGroups.Contains(groupName))
                {
                    adEd.WriteMessage("\nThe point group already exists, edit the existing group or come up with another name...");
                    return;
                }
                ObjectId groupId = pointGroups.Add(groupName);
                PointGroup group = (PointGroup)groupId.GetObject(OpenMode.ForRead);
                group.SetQuery(query);
                tr.Commit();
            }
            adEd.WriteMessage("\nPoint group created successfully!");
        }

        [CommandMethod("CreateACPoints")]
        public static void CreateACPoint()
        {
            var AcDoc = AcApp.DocumentManager.MdiActiveDocument;
            var AcEdit = AcDoc.Editor;
            var AcDb = AcDoc.Database;
            var OSnapZ = Convert.ToBoolean(AcApp.TryGetSystemVariable("OSnapZ"));

            if (OSnapZ)
            {
                var ret = System.Windows.Forms.MessageBox.Show("Objects will be placed at elevation 0. Continue?", "OSnapZ is on", MessageBoxButtons.YesNo);
                if (ret == DialogResult.No)
                {
                    AcEdit.WriteMessage("Command exited per user input.");
                    return;
                }
            }

            AcEdit.WriteMessage("\nPlease select a point " + (OSnapZ ? "[2D]: " : "[3D]: "));


            PromptPointOptions pointOptions = new PromptPointOptions("\nPlease select a point: ")
            {
                AllowNone = true,
                UseBasePoint = false
            };

            bool continueCommand = true;

            while (continueCommand)
            {
                var point = AcEdit.GetPoint(pointOptions);
                if (point.Status == PromptStatus.Cancel)
                {
                    break;
                }
                Point3d endPt = point.Value;
                if (OSnapZ)
                    endPt = new Point3d(endPt.X, endPt.Y, 0);

                using (Transaction tr = AcDb.TransactionManager.StartTransaction())
                {
                    // Open the Block table record for read
                    BlockTable acBlkTbl = tr.GetObject(AcDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                    // Open the Block table record Model space for write
                    BlockTableRecord acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    DBPoint acPoint = new DBPoint(endPt);
                    acPoint.SetDatabaseDefaults();
                    acBlkTblRec.AppendEntity(acPoint);
                    tr.AddNewlyCreatedDBObject(acPoint, true);
                    tr.Commit();
                }
            }
        }
    }

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
