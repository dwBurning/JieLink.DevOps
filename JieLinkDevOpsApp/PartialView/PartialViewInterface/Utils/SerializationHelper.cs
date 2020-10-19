using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PartialViewInterface.Utils
{
    public class SerializationHelper
    {
		public static void SerializeToXMLFile<T>(string Path, T t)
		{
			try
			{
				string path = Path.Substring(0, Path.LastIndexOf("\\"));
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				if (File.Exists(Path))
				{
					File.Delete(Path);
				}
				using (FileStream fileStream = new FileStream(Path, FileMode.Create, FileAccess.Write))
				{
					XmlSerializer xmlSerializer = new XmlSerializer(t.GetType());
					xmlSerializer.Serialize(fileStream, t);
					fileStream.Close();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static T DeserializeFromXMLFile<T>(string Path) where T : class
		{
			T result = default(T);
			try
			{
				using (XmlReader xmlReader = XmlReader.Create(Path))
				{
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
					result = (T)((object)xmlSerializer.Deserialize(xmlReader));
				}
			}
			catch (Exception ex)
			{
				result = default(T);
				throw ex;
			}
			return result;
		}
	}
}
