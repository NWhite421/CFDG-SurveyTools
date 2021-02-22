using System;
using System.Windows.Input;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using ACApplication = Autodesk.AutoCAD.ApplicationServices.Application;
using CFDG.API;
using CFDG.ACAD.Functions;
using Autodesk.AutoCAD.StatusBar;

namespace HNH.ACAD
{
    public class Commands : IExtensionApplication
    {
        #region INIT AND DEINIT

        /// <summary>
        /// Establish the initial event handlers
        /// </summary>
        void IExtensionApplication.Initialize()
        {
            // Add event handler for every drawing opened.
            ACApplication.DocumentManager.DocumentCreated += LoadDWG;

            // Add event handler for every drawing closed.
            ACApplication.DocumentManager.DocumentDestroyed += UnLoadDWG;

            // Add event handler when AutoCAD goes idle (removes itself after first run to bypass bullshit).
            Autodesk.AutoCAD.ApplicationServices.Application.Idle += OnAppLoad;
        }

        /// <summary>
        /// Runs when the Autocad Application executes Idle event handler. Removes itself after first run.
        /// Litterally a copy paste of LoadDWG() with the added benefit of custom tab addition.
        /// </summary>
        public void OnAppLoad(object s, EventArgs e)
        {
            // Add custom ribbon to RibbonControl
            RibbonControl ribbon = ComponentManager.Ribbon;
            if (ribbon != null)
            {
                EstablishTab();
                // Remove this event handler as to not fire again.
                // Ensures that the tab is established on startup, but will not create additional.
                Autodesk.AutoCAD.ApplicationServices.Application.Idle -= OnAppLoad;
            }

            ACApplication.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"CFDG Survey plugin version {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version} has been loaded successfully");

            OnEachDocLoad();
        }

        /// <summary>
        /// Runs when a new drawing is opened.
        /// </summary>
        public void LoadDWG(object s, DocumentCollectionEventArgs e)
        {
            OnEachDocLoad();
        }

        //TODO: Add function to notify user if n amount of documents are open.
        /// <summary>
        /// Shared method between LoadDWG and OnAppLoad
        /// </summary>
        internal void OnEachDocLoad()
        {
            if ((bool)XML.ReadValue("Autocad", "EnableOsnapZ"))
            {
                ACApplication.SetSystemVariable("OSnapZ", 1);
            }
        }

        /// <summary>
        /// Runs when a drawing is closed.
        /// </summary>
        public static void UnLoadDWG(object s, DocumentDestroyedEventArgs e)
        {
        }

        /// <summary>
        /// Fires once when the plugin is unloaded(? assumes when either plugin crashes or application is closed)
        /// </summary>
        void IExtensionApplication.Terminate()
        {

        }

        #endregion

        #region RIBBON

        #region DEFAULT DEFINITIONS
        /// <summary>
        /// Large placeholder button (vertical)
        /// </summary>
        readonly RibbonButton ButtonLarge = new RibbonButton
        {
            Text = "PLACEHOLDER",
            ShowImage = true,
            ShowText = true,
            Image = Imaging.BitmapToImageSource(CFDG.ACAD.Properties.Resources.placehold_16),
            LargeImage = Imaging.BitmapToImageSource(CFDG.ACAD.Properties.Resources.placehold_32),
            Orientation = System.Windows.Controls.Orientation.Vertical,
            Size = RibbonItemSize.Large,
            CommandHandler = new RibbonButtonHandler(),
            CommandParameter = "._PLACEHOLDER ",
        };

        /// <summary>
        /// Small placeholder button (horizontal)
        /// </summary>
        readonly RibbonButton ButtonSmall = new RibbonButton
        {
            Text = "PLACEHOLDER",
            ShowImage = false,
            ShowText = true,
            Image = Imaging.BitmapToImageSource(CFDG.ACAD.Properties.Resources.placehold_16),
            LargeImage = Imaging.BitmapToImageSource(CFDG.ACAD.Properties.Resources.placehold_32),
            Orientation = System.Windows.Controls.Orientation.Horizontal,
            Size = RibbonItemSize.Standard,
            CommandHandler = new RibbonButtonHandler(),
            CommandParameter = "._PLACEHOLDER "
        };

        /// <summary>
        /// A command to fill space until an actual command is made.
        /// </summary>
        [CommandMethod("PLACEHOLDER", CommandFlags.Transparent)]
        public void PlaceholderCommand()
        {
            var doc = ACApplication.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            ed.WriteMessage("Command not implimented yet..." + Environment.NewLine);
        }
        #endregion

        #region RIBBON CREATION
        /// <summary>
        /// Create tab and add panels to tab.
        /// </summary>
        public void EstablishTab()
        {
            //Get tab name
            string tabName = (string)XML.ReadValue("General","CompanyAbbreviation");

            //Add Ribbon
            RibbonControl ribbon = ComponentManager.Ribbon;

            // If the RibbonControl is established, then initialize our tab.
            if (ribbon != null)
            {
                // If the Ribbon already exists, don't create another.
                RibbonTab rtab = ribbon.FindTab("CSurveyTab");
                if (rtab != null)
                {
                    return;
                }

                rtab = new RibbonTab
                {
                    Title = tabName,
                    Id = "CSurveyTab"
                };

                RibbonPanelSource rps;
                RibbonPanel rp;
                RibbonButton btn;

                #region Project Management

                    rps = new RibbonPanelSource
                    {
                        Title = "Project Management",
                        Name = "Project Management"
                    };

                    rp = new RibbonPanel
                    {
                        Source = rps
                    };

                    var OpenFolderSplit = new RibbonSplitButton
                    {
                        Text = "splitbutton",
                        CommandHandler = new RibbonButtonHandler(),
                        ShowImage = true,
                        ShowText = true,
                        Image = Imaging.BitmapToImageSource(CFDG.ACAD.Properties.Resources.placehold_16),
                        LargeImage = Imaging.BitmapToImageSource(CFDG.ACAD.Properties.Resources.placehold_32),
                        IsSplit = true,
                        Size = RibbonItemSize.Large,
                        Orientation = System.Windows.Controls.Orientation.Vertical
                    };

                    btn = ButtonLarge.Clone() as RibbonButton;
                    btn.Text = $"Open{Environment.NewLine}Folder";
                    btn.CommandParameter = "._OpenProjectFolder ";
                    btn.LargeImage = Imaging.BitmapToImageSource(CFDG.ACAD.Properties.Resources.folder);
                    OpenFolderSplit.Items.Add(btn);

                    btn = ButtonLarge.Clone() as RibbonButton;
                    btn.Text = $"Open{Environment.NewLine}Comp Folder";
                    btn.CommandParameter = "._OpenCompFolder ";
                    btn.LargeImage = Imaging.BitmapToImageSource(
                        CFDG.ACAD.Properties.Resources.folder,
                        CFDG.ACAD.Properties.Resources.overlay_edit
                    );
                    OpenFolderSplit.Items.Add(btn);

                    btn = ButtonLarge.Clone() as RibbonButton;
                    btn.Text = $"Open{Environment.NewLine}Field Data";
                    btn.CommandParameter = "._OpenFieldDataFolder ";
                    btn.LargeImage = Imaging.BitmapToImageSource(
                        CFDG.ACAD.Properties.Resources.folder,
                        CFDG.ACAD.Properties.Resources.overlay_field
                    );
                    OpenFolderSplit.Items.Add(btn);

                    btn = ButtonLarge.Clone() as RibbonButton;
                    btn.Text = $"Open{Environment.NewLine}Submittals";
                    btn.CommandParameter = "._OpenSubmittalFolder ";
                    btn.LargeImage = Imaging.BitmapToImageSource(
                        CFDG.ACAD.Properties.Resources.folder,
                        CFDG.ACAD.Properties.Resources.arrow_up
                    );
                    OpenFolderSplit.Items.Add(btn);

                    rps.Items.Add(OpenFolderSplit);

                        btn = ButtonLarge.Clone() as RibbonButton;
                        btn.Text = $"Project{Environment.NewLine}Information";
                        //CompFolderbtn.CommandParameter = "._OpenProjectFolder ";
                        //btn.LargeImage = Imaging.BitmapToImageSource(CFDG.ACAD.Properties.Resources.folder);
                        rps.Items.Add(btn);

                    if (rps.Items.Count != 0)
                        rtab.Panels.Add(rp);

                #endregion

                #region Computations

                rps = new RibbonPanelSource
                {
                    Title = "Computations",
                    Name = "Computations"
                };
                rp = new RibbonPanel
                {
                    Source = rps
                };

                RibbonButton GroupPoints = ButtonLarge.Clone() as RibbonButton;
                GroupPoints.Text = $"Group Comp{Environment.NewLine}Points";
                GroupPoints.CommandParameter = "_.CreateGroupOfCalcs ";
                GroupPoints.LargeImage = Imaging.BitmapToImageSource(CFDG.ACAD.Properties.Resources.Create_PG);
                rps.Items.Add(GroupPoints);

                RibbonButton ExportGroup = ButtonLarge.Clone() as RibbonButton;
                ExportGroup.Text = $"Export Point{Environment.NewLine}Groups";
                //ExportGroup.CommandParameter = "_.ExportPointGroups ";
                ExportGroup.LargeImage = Imaging.BitmapToImageSource(CFDG.ACAD.Properties.Resources.Export_PG);
                rps.Items.Add(ExportGroup);

               
                btn = ButtonSmall.Clone() as RibbonButton;
                btn.Text = "Slope From Points";
                btn.CommandParameter = "GETSLOPEFROMPOINTS ";
                rps.Items.Add(btn);


                if (rps.Items.Count != 0)
                    rtab.Panels.Add(rp);
                #endregion

                #region Import & Export

                #endregion

                #region Support
                rps = new RibbonPanelSource
                {
                    Title = "Support",
                    Name = "Support"
                };
                rp = new RibbonPanel
                {
                    Source = rps
                };

                btn = ButtonLarge.Clone() as RibbonButton;
                btn.Text = "Give\nFeedback";
                btn.CommandParameter = "_.OpenCFDGFeedbackWindow ";
                rps.Items.Add(btn);


                if (rps.Items.Count != 0)
                    rtab.Panels.Add(rp);
                #endregion

                // Display tab in the RibbonControl for the user.
                if (rtab.Panels.Count != 0)
                    ribbon.Tabs.Add(rtab);

                // Let the user know this succeeded.
                ACApplication.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Panel loaded successfully..." + Environment.NewLine);

            }
            #endregion

        #endregion

        }
    }

    public class HelpSection
    {
        [CommandMethod("OpenCFDGFeedbackWindow")]
        public void OpenFeedback()
        {
            var dialog = new CFDG.UI.FeedbackWindow(0, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
            dialog.Show();
        }
    }

    /// <summary>
    /// RibbonButton click handler.
    /// </summary>
    public class RibbonButtonHandler : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }
#pragma warning disable CS0067 //command is never used
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067

        public void Execute(object parameter)

        {
            // Grab the command associated with the button
            RibbonButton cmd = parameter as RibbonButton;

            Document dwg = ACApplication.DocumentManager.MdiActiveDocument;

            // Send the command to the application in the current document
            dwg.SendStringToExecute(cmd.CommandParameter as string, true, false, true);

        }
    }
}
