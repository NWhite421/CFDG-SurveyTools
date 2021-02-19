using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

namespace CFDG.API
{
    /// <summary>
    /// Internal XML Handling including reading and writing
    /// </summary>
    public class XML
    {
        #region Properties
        private static readonly string xmlFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\AppSettings.xml";

        private static XElement xmlElement { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Read the value of <paramref name="key"/> in the <paramref name="category"/>
        /// </summary>
        /// <param name="category">Setting category</param>
        /// <param name="key">Key name</param>
        /// <returns>Value or null if not found.</returns>
        public static string ReadValue(string category, string key)
        {
            if (xmlElement == null)
            {
                xmlElement = XDocument.Load(xmlFile).Root;
            }
            category = category.ToLower();
            key = key.ToLower();

            if (string.IsNullOrEmpty(category) || string.IsNullOrEmpty(key))
            {
                return null;
            }

            string value = RetreiveValue(category, key);
            if (string.IsNullOrEmpty(value))
            {
            }
            return value;
        }
        #endregion

        #region Private Methods

        private static string RetreiveValue(string category, string key)
        {
            try
            {
                var element = xmlElement
                    .Element(category)
                    .Element(key)
                    .Attribute("value")
                    .Value;
                if (string.IsNullOrEmpty(element))
                {
                    return null;
                }
                return element;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
