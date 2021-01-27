using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32.SafeHandles;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using ACApplication = Autodesk.AutoCAD.ApplicationServices.Application;
using CFDG.API;

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
            ACApplication.DocumentManager.DocumentCreated += new DocumentCollectionEventHandler(LoadDWG);

            // Add event handler for every drawing closed.
            ACApplication.DocumentManager.DocumentDestroyed += new DocumentDestroyedEventHandler(UnLoadDWG);

            // Add event handler when AutoCAD goes idle (removes itself after first run to bypass bullshit).
            Autodesk.AutoCAD.ApplicationServices.Application.Idle += new EventHandler(OnAppLoad);
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

            OnEachDocLoad();
        }

        /// <summary>
        /// Runs when a new drawing is opened.
        /// </summary>
        public void LoadDWG(object s, DocumentCollectionEventArgs e)
        {
            //TODO: Impliment logging features and code cleanup.
            OnEachDocLoad();
        }

        /// <summary>
        /// Shared method between LoadDWG and OnAppLoad
        /// </summary>
        internal void OnEachDocLoad()
        {
            ACApplication.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"CFDG Survey plugin version {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version} has been loaded successfully");
        }

        /// <summary>
        /// Runs when a drawing is closed.
        /// </summary>
        public static void UnLoadDWG(object s, DocumentDestroyedEventArgs e)
        {
            //TODO: Impliment logging features and code cleanup.
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
        private readonly RibbonButton ButtonLarge = new RibbonButton
        {
            Text = "PLACEHOLDER",
            ShowImage = true,
            ShowText = true,
            //Image = IntFunctions.BitmapToImageSource(Properties.Resources.placeholder_small),
            //LargeImage = IntFunctions.BitmapToImageSource(Properties.Resources.placeholder_large),
            Orientation = System.Windows.Controls.Orientation.Vertical,
            Size = RibbonItemSize.Large,
            CommandHandler = new RibbonButtonHandler(),
            CommandParameter = "._PLACEHOLDER ",
        };

        /// <summary>
        /// Small placeholder button (horizontal)
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "Generic definition for future use")]
        private readonly RibbonButton ButtonSmall = new RibbonButton
        {
            Text = "PLACEHOLDER",
            ShowImage = true,
            ShowText = true,
            //Image = IntFunctions.BitmapToImageSource(Properties.Resources.placeholder_small),
            //LargeImage = IntFunctions.BitmapToImageSource(Properties.Resources.placeholder_large),
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
        [CommandMethod("EstablishTabs")]
        public void EstablishTab()
        {
            //Get tab name
            string tabName = INI.GetAppConfigSetting("ACAD", "TabName");

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
                //rtab.Panels.Add(AddProMgmtPanel()); // Project Management Group
                //rtab.Panels.Add(AddCompPanel());    // Computations Group
                //rtab.Panels.Add(HelpPanel());       // Help Group

                // Display tab in the RibbonControl for the user.
                ribbon.Tabs.Add(rtab);

                // Let the user know this succeeded.
                ACApplication.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Panel loaded successfully..." + Environment.NewLine);
            }

        }
        #endregion

        #endregion

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
