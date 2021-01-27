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

        static public string GetAppConfigSetting(string category, string title)
        {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(iniFile);
            string valueRaw = data[category][title];
            MatchCollection collection = Regex.Matches(valueRaw, @"\${\w+.\w+}");
            foreach (Match match in collection)
            {
                string[] item = match.Value.Trim('$', '{', '}').Split('.');
                string value = GetAppConfigSetting(item[0], item[1]);
                valueRaw = valueRaw.Replace(match.Value, value);
            }
            valueRaw = valueRaw.Replace("\"", "");
            return valueRaw;
        }
    }
}
