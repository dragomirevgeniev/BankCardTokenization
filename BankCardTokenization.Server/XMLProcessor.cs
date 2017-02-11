using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace BankCardTokenization.Server
{
    public class XMLProcessor
    { 
        public void LoadXml<T>(Type type, string filePath, ref T data) where T : new()
        {
            XmlSerializer serializer = new XmlSerializer(type);
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    data = (T)serializer.Deserialize(stream);
                }
            }
            catch (FileNotFoundException)
            {
                // if file does not exist then create new collection
                data = new T();
            }
        }

        public void SaveXml<T>(Type type, string filePath, List<T> data)
        {
            XmlSerializer serializer = new XmlSerializer(type);
            using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(stream, data);
            }
        }
    }
}
