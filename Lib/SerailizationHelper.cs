using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;
using BinaryFormatter = System.Runtime.Serialization.Formatters.Binary.BinaryFormatter;
using DebuggerHidden = System.Diagnostics.DebuggerHiddenAttribute;
using StringBuilder = System.Text.StringBuilder;

namespace Lib
{
    public static class SerializationHelper
    {
        #region Lock Object
        private static object LockObject
        {
            get;
            set;
        }
        #endregion

        #region Constructors
        static SerializationHelper()
        {
            LockObject = new object();
        }
        #endregion

        #region CSV Serialization/Deserialization

        public static string CsvSerialize(List<string> value)
        {
            return value != null ? string.Join(",", value.ToArray()) : null;
        }

        public static string CsvSerialize<T>(List<T> value, Converter<T, string> converter)
        {
            return value != null ? CsvSerialize(value.ConvertAll<string>(converter)) : null;
        }

        public static List<string> CsvDeserialize(string value)
        {
            return CsvDeserialize(value, new char[] { ',' }, StringSplitOptions.None);
        }

        public static List<string> CsvDeserialize(string value, char[] separator)
        {
            return CsvDeserialize(value, separator, StringSplitOptions.None);
        }

        /// <summary>
        /// Parses a comma (',') delimited line of text
        /// </summary>
        /// <param name="value">A single line of text</param>
        /// <param name="options">Include/exclude empty entries in returned list</param>
        /// <returns>A CSV parsed list of white space trimmed strings</returns>
        public static List<string> CsvDeserialize(string value, char[] separator, StringSplitOptions options)
        {
            List<string> list;

            if (!string.IsNullOrEmpty(value))
            {
                // Trim all values before splitting
                value = Regex.Replace(value, @"(\s+,\s+)|(\s+,)|(,\s+)", ",",
                  RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

                list = new List<string>(value.Split(separator, options));
            }
            else
            {
                list = new List<string>();
            }

            return list;
        }

        public static List<T> CsvDeserialize<T>(string value, Converter<string, T> converter)
        {
            return !string.IsNullOrEmpty(value)
              ? CsvDeserialize(value, new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ConvertAll<T>(converter)
              : new List<T>();
        }

        #endregion

        #region JSON Serialization/Deserialization

        public static T JsonDeserialize<T>(string input, params JavaScriptConverter[] converters)
        {
            JavaScriptSerializer serializer = CreateJavaScriptSerializer(converters);

            return serializer.Deserialize<T>(input);
        }

        public static string JsonSerialize(object value, params JavaScriptConverter[] converters)
        {
            JavaScriptSerializer serializer = CreateJavaScriptSerializer(converters);

            return serializer.Serialize(value);
        }

        public static void JsonSerialize(object value, StringBuilder output, params JavaScriptConverter[] converters)
        {
            JavaScriptSerializer serializer = CreateJavaScriptSerializer(converters);

            serializer.Serialize(value, output);
        }

        #endregion

        #region XML Serialization/Deserialization

        public static T XmlDeserialize<T>(string input)
        {
            T value = default(T);

            if (!string.IsNullOrEmpty(input))
            {
                using (StringReader reader = new StringReader(input))
                {
                    value = XmlDeserialize<T>(new XmlSerializer(typeof(T)), XmlReader.Create(reader));
                }
            }

            return value;
        }

        //public static T XmlDeserialize<T>(string input, string xmlroot)
        //{
        //  T value = default(T);

        //  if(!string.IsNullOrEmpty(input))
        //  {
        //    using(StringReader reader = new StringReader(input))
        //    {
        //      value = XmlDeserialize<T>(new XmlSerializer(typeof(T), new XmlRootAttribute(xmlroot)), XmlReader.Create(reader));
        //    }
        //  }

        //  return value;
        //}

        private static T XmlDeserialize<T>(XmlSerializer serializer, XmlReader input)
        {
            return (T)serializer.Deserialize(input);
        }

        [DebuggerHidden]
        private static string XmlSerialize(object value, string xmlroot)
        {
            return XmlSerialize(value, xmlroot, false, false);
        }

        private static string XmlSerialize(object value, string xmlroot, bool omitXmlDeclaration, bool excludeNamespaces)
        {
            StringBuilder output = new StringBuilder();

            XmlSerialize(value, output, xmlroot, omitXmlDeclaration, excludeNamespaces);

            return output.ToString();
        }

        [DebuggerHidden]
        private static void XmlSerialize(object value, StringBuilder output, string xmlroot)
        {
            XmlSerialize(value, output, xmlroot, false, false);
        }

        private static void XmlSerialize(object value, StringBuilder output, string xmlroot, bool omitXmlDeclaration, bool excludeNamespaces)
        {
            XmlWriterSettings settings = new XmlWriterSettings();

            settings.OmitXmlDeclaration = omitXmlDeclaration;

            XmlSerialize(value, XmlWriter.Create(output, settings), xmlroot, excludeNamespaces);
        }

        [DebuggerHidden]
        private static void XmlSerialize(object value, XmlWriter output, string xmlroot, bool excludeNamespaces)
        {
            XmlSerialize(new XmlSerializer(value.GetType(), new XmlRootAttribute(xmlroot)), value, output, excludeNamespaces);
        }

        public static string XmlSerialize(object value)
        {
            //Default is not to indent the elements
            return XmlSerialize(value, false);
        }

        public static string XmlSerialize(object value, bool indentElements)
        {
            StringBuilder output = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = indentElements;

            XmlSerialize(new XmlSerializer(value.GetType()), value, XmlWriter.Create(output, settings), true);

            return output.ToString();
        }

        /// <summary>
        /// Typically used when an object needs to be serialized to an XmlWriter,
        /// along with other XML
        /// - or -
        /// Used when you want to control how the XML is written
        /// </summary>
        /// <param name="value"></param>
        /// <param name="output">Writes serialized output to this writer</param>
        /// <example>(see Notification.cs)</example>
        [DebuggerHidden]
        public static void XmlSerialize(object value, XmlWriter output)
        {
            XmlSerialize(new XmlSerializer(value.GetType()), value, output, true);
        }

        /// <summary>
        /// Does the actual serialization.
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="value"></param>
        /// <param name="output"></param>
        /// <param name="excludeNamespaces"></param>
        private static void XmlSerialize(XmlSerializer serializer, object value, XmlWriter output, bool excludeNamespaces)
        {
            try
            {
                if (excludeNamespaces)
                {
                    XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                    namespaces.Add("", "");

                    serializer.Serialize(output, value, namespaces);
                }
                else
                {
                    serializer.Serialize(output, value);
                }
            }
            catch (Exception ex)
            {
                throw GetInnerMostException(ex);
            }
        }

        #endregion

        #region DataSet Serialization/Deserialization
        #region Binary Array (DB Image)
        public static byte[] DataSetSerialize(DataSet ds)
        {
            ds.RemotingFormat = System.Data.SerializationFormat.Binary;
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, ds);
                stream.Position = 0;
                return stream.ToArray();
            }
        }
        public static DataSet DataSetDeserialize(byte[] byteArray)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(byteArray, 0, byteArray.Length);
                stream.Position = 0;
                object o = formatter.Deserialize(stream);
                return (DataSet)o;
            }
        }
        #endregion
        #region XSD Based DataSet to/from Weak XSD Generated "Strongly Typed Objects" Ha!!!...
        //Note: This methodology should not be used for an extensive list of reasons
        //      This is only here to remind everyone of the hurdles that we had to jump...
        public static T GetStronglyTypedObjects<T>(DataSet ds)
        {
            //Warning: Do not use this on large DataSets (performance hit)
            return XmlDeserialize<T>(ds.GetXml());
        }
        public static void GetDataSet<T>(T sto, ref DataSet ds)
        {
            //Warning: Be sure to do a ReadXmlSchema on the ds before reading the XML
            //         Otherwise, if a column for all rows has null values, then 
            //         that column won't be in the resultant DataSet, which can cause a lot of grief later.
            //         Not good...  Not at all intuitive.  Another fine MS "convenience" feature.
            //Warning: Do not use this on large DataSets (performance hit)
            string xml = XmlSerialize(sto);
            StringReader sr = new StringReader(xml);
            ds.ReadXml(sr);
        }
        #endregion
        #endregion

        #region Binary Serialization/Deserialization
        public static byte[] BinarySerialize<T>(T thing)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, thing);
                stream.Position = 0;
                return stream.ToArray();
            }
        }
        public static T BinaryDeserialize<T>(byte[] byteArray)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(byteArray, 0, byteArray.Length);
                stream.Position = 0;
                object o = formatter.Deserialize(stream);
                return (T)o;
            }
        }
        #endregion

        #region Helper Methods

        /// <summary>
        /// Performs a deep copy of any object marked as "Serializable", and whose
        /// containing types in the object graph are also marked as "Serializable".
        /// It does this by serializing the object's fields into a binary stream,
        /// then deserializing a full copy of the object from the same stream.
        /// </summary>
        /// <typeparam name="T">
        /// This never has to be explicitly set.  The purpose of this is to avoid a
        /// type-cast by the caller.
        /// </typeparam>
        /// <param name="value">The object to be copied</param>
        /// <returns>A deep copy of the object passed in</returns>
        public static T Copy<T>(T value)
        {
            lock (LockObject)
            {
                T copy = default(T);

                if (value != null)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(stream, value);
                        stream.Position = 0;
                        copy = (T)formatter.Deserialize(stream);
                    }
                }

                return copy;
            }
        }

        private static JavaScriptSerializer CreateJavaScriptSerializer(params JavaScriptConverter[] converters)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            if (converters != null && converters.Length > 0)
            {
                serializer.RegisterConverters(converters);
            }

            return serializer;
        }

        private static Exception GetInnerMostException(Exception ex)
        {
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }

            return ex;
        }

        #endregion
    }
}
