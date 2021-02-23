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

namespace CFDG.ACAD
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

            ACApplication.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"\nCFDG Survey plugin version {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version} has been loaded successfully\n");

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
            //TODO: Fix tab name to include "Survey"
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

                //Project Management Tab
                rtab.Panels.Add(
                    Ribbon.CreatePanel("Project Management", "ProjectManagement",
                    Ribbon.CreateLargeSplitButton(
                        Ribbon.CreateLargeButton("Open\nFolder", "OpenProjectFolder", Properties.Resources.folder),
                        Ribbon.CreateLargeButton("Open Comp\nFolder", "OpenCompFolder", Properties.Resources.folder, Properties.Resources.overlay_edit),
                        Ribbon.CreateLargeButton("Open Field\nData Folder", "OpenFieldDataFold", Properties.Resources.folder, Properties.Resources.overlay_field),
                        Ribbon.CreateLargeButton("Open Submittal\nFolder", "OpenSubmittalFolder", Properties.Resources.folder, Properties.Resources.overlay_export)
                        )
                    )
                );

                //Computations Tab
                rtab.Panels.Add(
                    Ribbon.CreatePanel("Computations", "Computations",
                        Ribbon.CreateLargeButton("Group Comp\nPoints", "CreateGroupOfCalcs", Properties.Resources.Create_PG),
                        Ribbon.RibbonSpacer,
                        Ribbon.CreateRibbonRow(Ribbon.RibbonRowType.TextOnly,
                            Ribbon.CreateSmallButton("Slope From Points", "SlopeFromPoints"),
                            Ribbon.CreateSmallButton("Create Measure Down", "Measuredowns")
                        )
                    )
                );

                // Display tab in the RibbonControl for the user.
                if (rtab.Panels.Count != 0)
                    ribbon.Tabs.Add(rtab);
            }
            #endregion

        #endregion

        }
    }
}
