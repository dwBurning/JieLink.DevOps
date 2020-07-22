using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface.Utils
{
    public class ConfigHelper
    {
        public static T GetValue<T>(string key, T defaultVal)
        {
            object val = ConfigurationManager.AppSettings[key];
            if (val == null)
            {
                return defaultVal;
            }
            val = Convert.ChangeType(val, typeof(T));
            if (val == null)
            {
                return defaultVal;
            }
            return (T)val;
        }
    }
}
