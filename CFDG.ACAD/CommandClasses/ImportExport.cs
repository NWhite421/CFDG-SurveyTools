﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.PlottingServices;
using Autodesk.AutoCAD.Publishing;
using Autodesk.AutoCAD.Runtime;
using ACApp = Autodesk.AutoCAD.ApplicationServices.Application;


namespace CFDG.ACAD
{
    // Base class for the different configurations
    public abstract class PlotToFileConfig
    {
        // Private fields
        private string dsdFile, dwgFile, outputDir, outputFile, plotType;
        private int sheetNum;
        private IEnumerable<Layout> layouts;
        private const string LOG = "publish.log";

        // Base constructor
        public PlotToFileConfig(string outputDir, IEnumerable<Layout> layouts, string plotType)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            this.dwgFile = db.Filename;
            this.outputDir = outputDir;
            this.dsdFile = Path.ChangeExtension(this.dwgFile, "dsd");
            this.layouts = layouts;
            this.plotType = plotType;
            string ext = plotType == "0" || plotType == "1" ? "dwf" : "pdf";
            this.outputFile = Path.Combine(
                this.outputDir,
                Path.ChangeExtension(Path.GetFileName(this.dwgFile), ext));
        }

        // Plot the layouts
        public void Publish()
        {
            if (TryCreateDSD())
            {
                object bgp = Application.GetSystemVariable("BACKGROUNDPLOT");
                object ctab = Application.GetSystemVariable("CTAB");
                try
                {
                    Application.SetSystemVariable("BACKGROUNDPLOT", 0);

                    Publisher publisher = Application.Publisher;
                    var plotDlg = new PlotProgressDialog(false, this.sheetNum, true);
                    publisher.PublishDsd(this.dsdFile, plotDlg);
                    plotDlg.Destroy();
                    File.Delete(this.dsdFile);
                }
                catch (System.Exception exn)
                {
                    Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
                    ed.WriteMessage("\nError: {0}\n{1}", exn.Message, exn.StackTrace);
                    throw;
                }
                finally
                {
                    Application.SetSystemVariable("BACKGROUNDPLOT", bgp);
                    Application.SetSystemVariable("CTAB", ctab);
                }
            }
        }

        // Creates the DSD file from a template (default options)
        private bool TryCreateDSD()
        {
            using (var dsd = new DsdData())
            using (DsdEntryCollection dsdEntries = CreateDsdEntryCollection(this.layouts))
            {
                if (dsdEntries == null || dsdEntries.Count <= 0)
                {
                    return false;
                }

                if (!Directory.Exists(this.outputDir))
                {
                    Directory.CreateDirectory(this.outputDir);
                }

                this.sheetNum = dsdEntries.Count;

                dsd.SetDsdEntryCollection(dsdEntries);

                dsd.SetUnrecognizedData("PwdProtectPublishedDWF", "FALSE");
                dsd.SetUnrecognizedData("PromptForPwd", "FALSE");
                dsd.NoOfCopies = 1;
                dsd.DestinationName = this.outputFile;
                dsd.IsHomogeneous = false;
                dsd.LogFilePath = Path.Combine(this.outputDir, LOG);

                PostProcessDSD(dsd);

                return true;
            }
        }

        // Creates an entry collection (one per layout) for the DSD file
        private DsdEntryCollection CreateDsdEntryCollection(IEnumerable<Layout> layouts)
        {
            var entries = new DsdEntryCollection();

            foreach (Layout layout in layouts)
            {
                var dsdEntry = new DsdEntry();
                dsdEntry.DwgName = this.dwgFile;
                dsdEntry.Layout = layout.LayoutName;
                dsdEntry.Title = Path.GetFileNameWithoutExtension(this.dwgFile) + "-" + layout.LayoutName;
                dsdEntry.Nps = layout.TabOrder.ToString();
                entries.Add(dsdEntry);
            }
            return entries;
        }

        // Writes the definitive DSD file from the templates and additional informations
        private void PostProcessDSD(DsdData dsd)
        {
            string str, newStr;
            string tmpFile = Path.Combine(this.outputDir, "temp.dsd");

            dsd.WriteDsd(tmpFile);

            using (var reader = new StreamReader(tmpFile, Encoding.Default))
            using (var writer = new StreamWriter(this.dsdFile, false, Encoding.Default))
            {
                while (!reader.EndOfStream)
                {
                    str = reader.ReadLine();
                    if (str.Contains("Has3DDWF"))
                    {
                        newStr = "Has3DDWF=0";
                    }
                    else if (str.Contains("OriginalSheetPath"))
                    {
                        newStr = "OriginalSheetPath=" + this.dwgFile;
                    }
                    else if (str.Contains("Type"))
                    {
                        newStr = "Type=" + this.plotType;
                    }
                    else if (str.Contains("OUT"))
                    {
                        newStr = "OUT=" + this.outputDir;
                    }
                    else if (str.Contains("IncludeLayer"))
                    {
                        newStr = "IncludeLayer=TRUE";
                    }
                    else if (str.Contains("PromptForDwfName"))
                    {
                        newStr = "PromptForDwfName=FALSE";
                    }
                    else if (str.Contains("LogFilePath"))
                    {
                        newStr = "LogFilePath=" + Path.Combine(this.outputDir, LOG);
                    }
                    else
                    {
                        newStr = str;
                    }
                    writer.WriteLine(newStr);
                }
            }
            File.Delete(tmpFile);
        }
    }

    // Class to plot one DWF file per sheet
    public class SingleSheetDwf : PlotToFileConfig
    {
        public SingleSheetDwf(string outputDir, IEnumerable<Layout> layouts)
            : base(outputDir, layouts, "0") { }
    }

    // Class to plot a multi-sheet DWF file
    public class MultiSheetsDwf : PlotToFileConfig
    {
        public MultiSheetsDwf(string outputDir, IEnumerable<Layout> layouts)
            : base(outputDir, layouts, "1") { }
    }

    // Class to plot one PDF file per sheet
    public class SingleSheetPdf : PlotToFileConfig
    {
        public SingleSheetPdf(string outputDir, IEnumerable<Layout> layouts)
            : base(outputDir, layouts, "5") { }
    }

    // Class to plot a multi-sheet PDF file
    public class MultiSheetsPdf : PlotToFileConfig
    {
        public MultiSheetsPdf(string outputDir, IEnumerable<Layout> layouts)
            : base(outputDir, layouts, "6") { }
    }

    public class PlotService
    {
        [CommandMethod("PlotPages")]
        public static void PlotPages()
        {
            Document doc = ACApp.DocumentManager.MdiActiveDocument;
            Editor ACEditor = doc.Editor;
            Database ACDatabase = doc.Database;
            var layouts = new List<Layout> { };
            LayoutManager layoutMngr = LayoutManager.Current;

            using (Transaction trans = ACDatabase.TransactionManager.StartTransaction())
            {
                var layoutDic = trans.GetObject(ACDatabase.LayoutDictionaryId, OpenMode.ForRead, false) as DBDictionary;
                foreach (DBDictionaryEntry entry in layoutDic)
                {
                    ObjectId layoutID = entry.Value;
                    var layout = trans.GetObject(layoutID, OpenMode.ForRead) as Layout;
                    layouts.Add(layout);
                }

                trans.Commit();
            }

            var dialog = new UI.PlotPageDialog(layouts);
            bool? result = Application.ShowModalWindow(dialog);
            if (!result.Value)
            {
                ACEditor.WriteMessage($"{Environment.NewLine}The Plot Pages dialog was cancelled by the user.");
                return;
            }
        }
    }
}
