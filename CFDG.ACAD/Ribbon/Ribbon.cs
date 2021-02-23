using System;
using Autodesk.Windows;
using CFDG.ACAD.Functions;
using System.Windows.Controls;

namespace CFDG.ACAD
{
    /// <summary>
    /// Methods for creating AutoCAD ribbon items
    /// </summary>
    public class Ribbon
    {
        /// <summary>
        /// Create a large vertical button
        /// </summary>
        /// <param name="text">Display text</param>
        /// <param name="command">Command to execute</param>
        /// <returns>Large Vertical RibbonButton</returns>
        public static RibbonButton CreateLargeButton(string text, string command)
        {
            return CreateLargeButton(text, command, Properties.Resources.placehold_32);
        }

        /// <summary>
        /// Create a large vertical button
        /// </summary>
        /// <param name="text">Display text</param>
        /// <param name="command">Command to execute</param>
        /// <param name="images">List of Bitmap Images</param>
        /// <returns>Large Vertical RibbonButton</returns>
        public static RibbonButton CreateLargeButton(string text, string command, params System.Drawing.Bitmap[] images)
        {
            RibbonButton btn = new RibbonButton()
            {
                Text = text,
                ShowImage = true,
                ShowText = true,
                Orientation = Orientation.Vertical,
                Size = RibbonItemSize.Large,
                CommandHandler = new RibbonButtonHandler(),
                CommandParameter = $"._{ command.ToUpper() } ",
                LargeImage = Imaging.BitmapToImageSource(images)
            };
            return btn;
        }

        /// <summary>
        /// Create a small horizontal button
        /// </summary>
        /// <param name="text">Display text</param>
        /// <param name="command">Command to execute</param>
        /// <returns>small horizontal RibbonButton</returns>
        public static RibbonButton CreateSmallButton(string text, string command)
        {
            return CreateSmallButton(text, command, Properties.Resources.placehold_16);
        }

        /// <summary>
        /// Create a small horizontal button
        /// </summary>
        /// <param name="text">Display text</param>
        /// <param name="command">Command to execute</param>
        /// <param name="images">List of Bitmap Images</param>
        /// <returns>small horizontal RibbonButton</returns>
        public static RibbonButton CreateSmallButton(string text, string command, params System.Drawing.Bitmap[] images)
        {
            RibbonButton btn = new RibbonButton()
            {
                Text = text,
                ShowImage = true,
                ShowText = true,
                Orientation = Orientation.Horizontal,
                Size = RibbonItemSize.Standard,
                CommandHandler = new RibbonButtonHandler(),
                CommandParameter = $"._{ command.ToUpper() } ",
                Image = Imaging.BitmapToImageSource(images)
            };
            return btn;
        }

        /// <summary>
        /// Create a large vertical split button
        /// </summary>
        /// <param name="buttons">List of RibbonButtons, the first being default</param>
        /// <returns>Large vertical RibbonSplitButton</returns>
        public static RibbonSplitButton CreateLargeSplitButton(params RibbonButton[] buttons)
        {
            var btn = new RibbonSplitButton
            {
                Text = "splitbutton",
                CommandHandler = new RibbonButtonHandler(),
                ShowImage = true,
                ShowText = true,
                Image = Imaging.BitmapToImageSource(CFDG.ACAD.Properties.Resources.placehold_16),
                LargeImage = Imaging.BitmapToImageSource(CFDG.ACAD.Properties.Resources.placehold_32),
                IsSplit = true,
                Size = RibbonItemSize.Large,
                Orientation = Orientation.Vertical
            };
            foreach (var commandBtn in buttons)
            {
                btn.Items.Add(commandBtn);
            }
            return btn;
        }

        //TODO: Add feature for multiple buttons per line.
        /// <summary>
        /// Create a row set of buttons
        /// </summary>
        /// <param name="rowType">Row display type</param>
        /// <param name="button1">First button</param>
        /// <returns>small horizontal RibbonRowPanel</returns>
        public static RibbonRowPanel CreateRibbonRow(RibbonRowType rowType, RibbonButton button1)
        {
            RibbonRowPanel rrp = new RibbonRowPanel();
            if (rowType == RibbonRowType.ImageOnly)
            {
                button1.ShowText = false;
            }
            if (rowType == RibbonRowType.TextOnly)
            {
                button1.ShowImage = false;
            }
            rrp.Items.Add(button1);

            return rrp;
        }

        /// <summary>
        /// Create a row set of buttons
        /// </summary>
        /// <param name="rowType">Row display type</param>
        /// <param name="button1">First button</param>
        /// <param name="button2">Second button</param>
        /// <returns>small horizontal RibbonRowPanel</returns>
        public static RibbonRowPanel CreateRibbonRow(RibbonRowType rowType, RibbonButton button1, RibbonButton button2)
        {
            RibbonRowPanel rrp = new RibbonRowPanel();
            if (rowType == RibbonRowType.ImageOnly)
            {
                button1.ShowText = false;
                button2.ShowText = false;
            }
            if (rowType == RibbonRowType.TextOnly)
            {
                button1.ShowImage = false;
                button2.ShowImage = false;
            }
            rrp.Items.Add(button1);
            rrp.Items.Add(new RibbonRowBreak());
            rrp.Items.Add(button2);

            return rrp;
        }


        /// <summary>
        /// Create a row set of buttons
        /// </summary>
        /// <param name="rowType">Row display type</param>
        /// <param name="button1">First button</param>
        /// <param name="button2">Second button</param>
        /// <param name="button3">Third button</param>
        /// <returns>small horizontal RibbonRowPanel</returns>
        public static RibbonRowPanel CreateRibbonRow(RibbonRowType rowType, RibbonButton button1, RibbonButton button2, RibbonButton button3)
        {
            RibbonRowPanel rrp = new RibbonRowPanel();
            if (rowType == RibbonRowType.ImageOnly)
            {
                button1.ShowText = false;
                button2.ShowText = false;
                button3.ShowText = false;
            }
            if (rowType == RibbonRowType.TextOnly)
            {
                button1.ShowImage = false;
                button2.ShowImage = false;
                button3.ShowImage = false;
            }
            rrp.Items.Add(button1);
            rrp.Items.Add(new RibbonRowBreak());
            rrp.Items.Add(button2);
            rrp.Items.Add(new RibbonRowBreak());
            rrp.Items.Add(button3);

            return rrp;
        }

        /// <summary>
        /// Create a panel group of Buttons
        /// </summary>
        /// <param name="title">Group title</param>
        /// <param name="id">Group reference id</param>
        /// <param name="ribbonItems">List of RibbonItems</param>
        /// <returns>Ribbon Panel</returns>
        public static RibbonPanel CreatePanel(string title, string id, params RibbonItem[] ribbonItems)
        {
            RibbonPanelSource rps = new RibbonPanelSource()
            {
                Title = title,
                Name = id
            };

            RibbonPanel rp = new RibbonPanel()
            {
                Source = rps
            };

            foreach (var rItem in ribbonItems)
            {
                rps.Items.Add(rItem);
            }
            return rp;
        }

        /// <summary>
        /// Small whitespace in between ribbon columns
        /// </summary>
        public static RibbonSeparator RibbonSpacer
        {
            get
            {
                return new RibbonSeparator
                {
                    SeparatorStyle = RibbonSeparatorStyle.Spacer
                };
            }
        }

        /// <summary>
        /// Vertical line in between ribbon columns
        /// </summary>
        public static RibbonSeparator RibbonSeparator
        {
            get
            {
                return new RibbonSeparator
                {
                    SeparatorStyle = RibbonSeparatorStyle.Line
                };
            }
        }

        /// <summary>
        /// Defines RibbonRow display
        /// </summary>
        public enum RibbonRowType
        {
            /// <summary>
            /// Shows both the small image and text
            /// </summary>
            ImageAndText,
            /// <summary>
            /// Shows only the text (hides images)
            /// </summary>
            TextOnly,
            /// <summary>
            /// Show only the image (hides text)
            /// </summary>
            ImageOnly
        }
    }
}