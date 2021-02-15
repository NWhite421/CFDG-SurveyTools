using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;
using System.Text.RegularExpressions;

namespace CFDG.API
{

    public static class INI
    {
        static private readonly string iniFile = "appsettings.ini";

        /// <summary>
        /// Gets value of setting from appsettings.ini.
        /// </summary>
        /// <param name="section">Section name</param>
        /// <param name="title">Key title</param>
        /// <returns>value of key in section (with translations).</returns>
        static public string GetAppConfigSetting(string section, string title)
        {
            return GetAppConfigSetting(section, title, true);
        }

        /// <summary>
        /// Gets value of setting from appsettings.ini.
        /// </summary>
        /// <param name="section">Section name</param>
        /// <param name="title">Key title</param>
        /// <param name="replaceText">True to translate text, false to keep raw.</param>
        /// <returns>value of key in section.</returns>
        static public string GetAppConfigSetting(string section, string title, bool replaceText)
        {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(iniFile);
            data.SectionKeySeparator = '.';
            if (data.TryGetKey(section + "." + title, out string valueRaw))
            {
                if (replaceText)
                {
                    MatchCollection collection = Regex.Matches(valueRaw, @"\${\w+.\w+}");
                    foreach (Match match in collection)
                    {
                        string[] item = match.Value.Trim('$', '{', '}').Split('.');
                        string value = GetAppConfigSetting(item[0], item[1]);
                        valueRaw = valueRaw.Replace(match.Value, value);
                    }
                    valueRaw = valueRaw.Replace("\"", "");
                }
                return valueRaw;
            }
            return "";
        }
    }
}
