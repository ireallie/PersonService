using Person.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Person.Api
{
    public class Serializer
    {
        /// <summary>
        /// Serialize a collection of objects by converting each element of that array into its textual representation.
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static string Serialize(IEnumerable<object> objects)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.Append("[");

            List<string> serializedObjects = new List<string>();

            foreach (var obj in objects)
            {
                string serializedObj = Serialize(obj);
                serializedObjects.Add(serializedObj);
            }

            string separatedObjects = string.Join(",", serializedObjects);
            sb.Append(separatedObjects);

            sb.AppendLine();
            sb.Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Serialize an object by converting its properties into textual representations.
        /// </summary>
        /// <param name="obj">The object that will be serialized.</param>
        /// <param name="tab"></param>
        /// <param name="space"></param>
        /// <returns></returns>    
        public static string Serialize(object obj, string tab = "\t", string space = "  ")
        {
            var sb = new StringBuilder();

            if(tab.Length < 2)
            {
                sb.AppendLine();
                sb.Append(space);
            }

            sb.Append("{");
            sb.AppendLine();

            var myType = obj.GetType();

            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

            List<string> records = new List<string>();

            foreach (var prop in props)
            {
                var propValue = prop.GetValue(obj, null);

                bool isCustomType = propValue.GetType().Namespace != "System";
                propValue = isCustomType ? Serialize(propValue, tab + "\t", space.PadRight(8)) : propValue;

                if (prop.PropertyType == typeof(string))
                {
                    records.Add(@"" + tab + prop.Name + ": " + "'" + propValue + "'");
                }
                else
                {
                    records.Add(@"" + tab + prop.Name + ": " + propValue);
                }
            }

            string separatedRecords = string.Join(",\n", records);
            sb.Append(separatedRecords);

            sb.AppendLine();
            sb.Append(space + "}");
            return sb.ToString();
        }
        /// <summary>
        /// Returns a list of the serialized objects.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startString"></param>
        /// <param name="endString"></param>
        /// <param name="raw">Enabled: Will return the raw representation without any filtering.</param>
        /// <returns></returns>
        public static List<string> ExtractData(
            string text, string startString = "{", string endString = "}", bool raw = false)
        {
            var matched = new List<string>();

            bool exit = false;

            while (!exit)
            {
                var indexStart = text.IndexOf(startString, StringComparison.Ordinal);
                var indexEnd = text.LastIndexOf(endString, StringComparison.Ordinal);
                if (indexStart != -1 && indexEnd != -1)
                {
                    if (raw)
                        matched.Add("{" + text.Substring(indexStart + startString.Length,
                                        indexEnd - indexStart - startString.Length) + "}");
                    else
                        matched.Add(text.Substring(indexStart + startString.Length,
                            indexEnd - indexStart - startString.Length));
                    text = text.Substring(indexEnd + endString.Length);
                }
                else
                {
                    exit = true;
                }
            }
            return matched;
        }

        /// <summary>
        /// Returns a list of all the serialized properties.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static List<Data> ExtractValuesFromData(string text, string nesteObjectName = "Address:")
        {
            var listOfData = new List<Data>();

            string data = text.Substring(0, text.IndexOf(nesteObjectName));

            string[] records = data.Split(',').Select(sValue => sValue.Trim()).ToArray();

            foreach (var record in records)
            {
                if (string.IsNullOrEmpty(record) || string.IsNullOrWhiteSpace(record)) continue; 

                var pName = record.Substring(0, record.IndexOf(":", StringComparison.Ordinal))
                    .FirstCharToUpper();
                var pValue = record.Substring(record.IndexOf(":", StringComparison.Ordinal) + 2)
                    .Replace("‘", "")
                    .Replace("’", "");

                listOfData.Add(new Data { PropertyName = pName, Value = pValue });
            }
            return listOfData;
        }

        private static List<Data> ExtractValuesFromNestedObject(string text, string nesteObjectName = "Address:")
        {
            var listOfData = new List<Data>();

            int index = text.IndexOf(nesteObjectName);
            string nestedObject = text.Substring(index + nesteObjectName.Length + 1);

            var props = ExtractData(nestedObject);

            string[] records = new string[] { };

            foreach (var prop in props)
            {
                records = prop.Split(',').Select(sValue => sValue.Trim()).ToArray();
            }

            foreach (var record in records)
            {
                if (string.IsNullOrEmpty(record) || string.IsNullOrWhiteSpace(record)) continue;

                var pName = record.Substring(0, record.IndexOf(":", StringComparison.Ordinal))
                    .FirstCharToUpper();
                var pValue = record.Substring(record.IndexOf(":", StringComparison.Ordinal) + 2)
                    .Replace("‘", "")
                    .Replace("’", "");

                listOfData.Add(new Data { PropertyName = pName, Value = pValue });
            }
            return listOfData;
        }


        /// <summary>
        /// Deserialize an object based on some serialized data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializeData"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static T DeSerialize<T>(string serializeData, T target) where T : new()
        {
            var deserializedObjects = ExtractData(serializeData);

            foreach (var obj in deserializedObjects)
            {
                var properties = ExtractValuesFromData(obj);
                var nestedProperties = ExtractValuesFromNestedObject(obj);

                foreach (var property in properties)
                {
                    var propInfo = target.GetType().GetProperty(property.PropertyName);

                    object propValue = ConvertValue(propInfo.PropertyType, property.Value);

                    propInfo?.SetValue(target, propValue, null);
                }

                var address = new Address();
               
                foreach (var property in nestedProperties)
                {
                    var propInfo = address.GetType().GetProperty(property.PropertyName);

                    object propValue = ConvertValue(propInfo.PropertyType, property.Value);

                    propInfo?.SetValue(address, propValue, null);
                }

                var addressInfo = target.GetType().GetProperty("Address");
                addressInfo?.SetValue(target,
                   Convert.ChangeType(address, addressInfo.PropertyType), null);

            }
            return target;
        }

        private static object ConvertValue(Type propType, string propValue)
        {
            object result = null;

            if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (!string.IsNullOrEmpty(propValue))
                {
                    result = Convert.ChangeType(propValue, propType.GetGenericArguments()[0]);
                }
            }
            else
            {
                result = Convert.ChangeType(propValue, propType);
            }

            return result;
        }

        /// <summary>
        /// Deserialize a collection of objects based on some serialized data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializeData"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static List<T> DeSerializeCollection<T>(string serializeData, T target) where T : new()
        {
            List<T> list = new List<T>();

            List<string> extractedObjects = ExtractData(serializeData, "[", "]");

            string[] objects = extractedObjects[0].Split("},");

            for(int i = 0; i < objects.Length; i++)
            {
                if(i != objects.Length - 1)
                {
                    objects[i] += "}";
                }

                list.Add(DeSerialize(objects[i], target));
            }

            return list;
        }
        public struct Data
        {
            public string PropertyName;
            public string Value;
        }
    }
}
