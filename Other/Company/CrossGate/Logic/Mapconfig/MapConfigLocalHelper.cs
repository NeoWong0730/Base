using System;
using System.Collections.Generic;
using Google.Protobuf;
using System.IO;
using Packet;
using System.Runtime.Serialization.Formatters.Binary;

namespace Logic
{
    public class MapConfigLocalHelper
    {
        public static void ListParse<T, T1>(List<T> value0, IList<T1> value1) where T : IMapInfoProtoParse<T1>,new() where T1 : class
        {
            for (int i = 0; i < value1.Count; i++)
            {
                if (value1[i] == null)
                    continue;
                var obj = new T();

                obj.ParseValue(value1[i]);

                value0.Add(obj);
            }
        }

        public static void Serialize(Stream stream, MapCfgInfo mapCfgInfo)
        {
            var obj =  MapConfigLocal.Parse(mapCfgInfo);

            // Serialize(stream, obj);

            OutputStream outputStream = new OutputStream() { mStream = stream};

            //outputStream.WriteISerialize(obj);
            obj.Serialize(outputStream);


        }

        //public static void Serialize(Stream stream, MapConfigLocal mapCfgInfo)
        //{
        //    BinaryFormatter formatter = new BinaryFormatter();

        //    formatter.Serialize(stream, mapCfgInfo);
        //}

        public static MapConfigLocal DeSerialize(Stream stream)
        {
            InputStream formatter = new InputStream(stream);

            MapConfigLocal mapConfigLocal = new MapConfigLocal();

            mapConfigLocal.DeSerialize(formatter);

            return mapConfigLocal;
        }
    }

   
}
