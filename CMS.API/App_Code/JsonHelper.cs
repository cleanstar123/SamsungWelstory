using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace CMS.API.App_Code
{
    public class JsonHelper
    {
        public static string GetJsonToXmlString(object req)
        {
            return GetXmlString((XmlDocument)JsonConvert.DeserializeXmlNode("{" + JsonConvert.SerializeObject(req) + "}", "BODY"));
        }

        public static string GetXmlString(XmlDocument xmlDoc)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            xw.Formatting = System.Xml.Formatting.None;
            xmlDoc.WriteTo(xw);

            return sw.ToString();
        }

        public static string GetJsonToXmlString<T>(List<T> req)
        {
            string json = string.Empty;

            if (req == null || req.Count <= 0)
                return GetXmlString((XmlDocument)JsonConvert.DeserializeXmlNode("{}", "BODY"));

            for (int i = 0; i < req.Count; i++)
            {
                if (i == 0)
                {
                    json += "{";
                }

                json += JsonWrapper("DATA", JsonConvert.SerializeObject(req[i]));

                if (i == req.Count - 1)
                {
                    json += "}";
                }
                else
                {
                    json += ",";
                }
            }
               
            return GetXmlString((XmlDocument)JsonConvert.DeserializeXmlNode(json, "BODY"));
        }

        public static string JsonWrapper(string wrapper, string json)
        {
            return string.Format("\"{0}\": {1}", wrapper, json);
        }
    }
}