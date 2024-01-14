using System;
using System.IO;
using System.Json;
using System.Reflection;
using UnityEngine;

namespace Logic
{
    public static class JsonHeler
    {
        public static void DeserializeObject(JsonObject jsonObject, object obj)
        {
            Type type = obj.GetType();
            foreach (var jo in jsonObject)
            {
                var param = type.GetField(jo.Key, BindingFlags.Public | BindingFlags.Instance);
                if (param != null)
                {
                    if (jo.Value is JsonObject)
                    {
                        param.SetValue(obj, jo.Value);
                    }
                    else if (jo.Value is JsonPrimitive)
                    {
                        var value = ((JsonPrimitive)jo.Value).Value;

                        try
                        {
                            if (value != null)
                            {
                                if (!value.GetType().Equals(param.FieldType))
                                {
                                    if (param.FieldType.IsPrimitive)
                                    {
                                        value = Convert.ChangeType(value, param.FieldType);
                                    }
                                }
                            }
                            param.SetValue(obj, value);
                        }
                        catch (Exception e)
                        {
                            param.SetValue(obj, value);
                            Debug.LogError($"{jo.Key} {param.FieldType} {value.GetType().Name}   ----  {e.Message}{e.StackTrace}");
                        }
                    }
                }
            }
        }

        public static void SerializeToJsonFile(object o, string filePath)
        {
            string json = LitJson.JsonMapper.ToJson(o);

            if (!File.Exists(filePath)) { using (File.Create(filePath)) {/* let using close filestream! */ } }
            using (StreamWriter sw = new StreamWriter(filePath, false)) { sw.Write(json); }
        }

        public static bool TryDeSerializeFromJsonFile<T>(string filePath, out T o) where T : class, new()
        {
            bool ret = true;
            if (!File.Exists(filePath)) { ret = false; o = new T(); return ret; }
            string json = null;

            using (StreamReader sr = new StreamReader(filePath, System.Text.Encoding.UTF8)) { json = sr.ReadToEnd(); }
            o = LitJson.JsonMapper.ToObject<T>(json);
            return ret;
        }

        public static string GetJsonStr(string filePath)
        {
            if (!File.Exists(filePath))
                return string.Empty;

            string json = null;
            using (StreamReader sr = new StreamReader(filePath, System.Text.Encoding.UTF8)) { json = sr.ReadToEnd(); }
            return json;
        }
    }
}
