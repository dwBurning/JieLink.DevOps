using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
namespace PartialViewInterface.Utils
{
    public class JsonHelper
    {
        public static string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj, GetSetting());
        }
        public static T DeserializeObject<T>(string json)
        {
            
            return JsonConvert.DeserializeObject<T>(json, GetSetting());
        }
        private static JsonSerializerSettings GetSetting()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            settings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            settings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            return settings;
        }
        public static T GetFromFile<T>(string path)
        {
            if(!File.Exists(path))
            {
                return default(T);
            }
            string json = null;
            using (StreamReader sr = new StreamReader(path,Encoding.UTF8))
            {
                json = sr.ReadToEnd();
            }
            try
            {
                return DeserializeObject<T>(json);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return default(T);
            }

        }
    }
}
