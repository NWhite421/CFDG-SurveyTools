using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CFDG.API
{
    public enum JobNumberFormats
    {
        ShortNoHyphan,
        ShortHyphan,
        LongNoHyphan,
        LongHyphan
    }

    public class JobNumber
    {
        #region CONSTANTS

        /// <summary>
        /// Regex formats of job numbers.
        /// </summary>
        public class Formats
        {
            /// <summary>
            /// XXXXXXX (7 Numbers)
            /// </summary>
            public static string ShortNoHyphan
            {
                get
                {
                    return @"^\d{7}$";
                }
            }

            /// <summary>
            /// XX-XX-XXX (7 Numbers with hyphans after the 2nd and 4th)
            /// </summary>
            public static string ShortHyphan
            {
                get
                {
                    return @"^\d{2}-\d{2}-\d{3}";
                }
            }

            /// <summary>
            /// XXXXXXXXX (9 Numbers)
            /// </summary>
            public static string LongNoHyphan
            {
                get
                {
                    return @"^\d{9}$";
                }
            }

            /// <summary>
            /// XXXX-XX-XXX (9 Numbers with hyphans after the 4th and 6th)
            /// </summary>
            public static string LongHyphan
            {
                get
                {
                    return @"^\d{4}-\d{2}-\d{3}";
                }
            }
        }
        #endregion

        /// <summary>
        /// Check the input to determine if it is a job number.
        /// </summary>
        /// <param name="Input">The string to check.</param>
        /// <returns>True if a job number format, false if not.</returns>
        public static bool TryParse(string Input)
        {
            return TryParse(Input, out _, JobNumberFormats.LongHyphan);
        }

        /// <summary>
        /// Check the input to determine if it is a job number.
        /// </summary>
        /// <param name="Input">String to check.</param>
        /// <param name="Format">The format you wish to output in <paramref name="FormatNumber"/>.</param>
        /// <param name="FormatNumber">The formatted string if it can be transformed.</param>
        /// <returns>True if a job number format, false if not.</returns>
        public static bool TryParse(string Input, out string FormatNumber, JobNumberFormats Format)
        {
            if (string.IsNullOrEmpty(Input))
            {
                FormatNumber = null;
                return false;
            }

            //If the input string is not a correctly formatted number, return false
            if (
                !Regex.IsMatch(Input, Formats.ShortHyphan) &&
                !Regex.IsMatch(Input, Formats.ShortNoHyphan) &&
                !Regex.IsMatch(Input, Formats.LongHyphan) &&
                !Regex.IsMatch(Input, Formats.LongNoHyphan)
            )
            {
                FormatNumber = null;
                return false;
            }

            //make the string a long no hyphan format by default to easily format into 
            string formatted = Input.Replace("-", "");
            if (formatted.Length != 9)
            {
                formatted = formatted.Insert(0, "20");
            }

            //Format number and output.
            switch (Format)
            {
                case JobNumberFormats.ShortHyphan:
                    {
                        FormatNumber = formatted.Remove(0, 2).Insert(2, "-").Insert(5, "-");
                        return true;
                    }
                case JobNumberFormats.LongNoHyphan:
                    {
                        FormatNumber = formatted;
                        return true;
                    }
                case JobNumberFormats.LongHyphan:
                    {
                        FormatNumber = formatted.Insert(4, "-").Insert(7, "-");
                        return true;
                    }
                case JobNumberFormats.ShortNoHyphan:
                    {
                        FormatNumber = formatted.Remove(0, 2);
                        return true;
                    }
                default:
                    {
                        FormatNumber = null;
                        return false;
                    }
            }

        }

        /// <summary>
        /// Return the full path of the Job Number directory
        /// </summary>
        /// <param name="JobNumber">Job number to find.</param>
        /// <returns>string of path or null if not found.</returns>
        public static string GetPath(string JobNumber)
        {
            //Base variables setup
            string dir = INI.GetAppConfigSetting("General", "DefaultProjectPath");

            //Error checks
            if (!TryParse(JobNumber, out string fullNumber, JobNumberFormats.LongHyphan))
            {
                //Log.AddWarning($"Job number {JobNumber} failed to correctly parse.");
                return null;
            }

            if (!Directory.Exists(dir))
            {
                //Log.AddWarning($"base path \"{dir}\" does not exist.");
                return null;
            }


            string[] parts = fullNumber.Split('-');
            dir = Path.Combine(dir, $@"{parts[0]}\{parts[1]}-{parts[0]}\{parts[1]}-{parts[2]}");

            if (!Directory.Exists(dir))
            {
                //Log.AddWarning($"Path \"{dir}\" does not exist.");
                return null;
            }

            return dir;
        }

        /// <summary>
        /// Parse number to another format.
        /// </summary>
        /// <param name="Input">Source job number.</param>
        /// <param name="Format">New format.</param>
        /// <returns>Reformatted job number.</returns>
        public static string Parse(string Input, JobNumberFormats Format)
        {
            if (TryParse(Input, out string reformatted, Format))
            {
                return reformatted;
            }
            else
            {
                return "";
            }
        }
    }
}
