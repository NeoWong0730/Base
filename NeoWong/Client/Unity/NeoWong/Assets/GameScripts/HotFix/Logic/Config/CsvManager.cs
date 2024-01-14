using cfg;
using NWFramework;
using System.Collections.Generic;
using UnityEngine;
using Luban;

namespace HotFix
{
    public class CsvManager : Singleton<CsvManager>
    {
        readonly Dictionary<string, object> tableDict = new Dictionary<string, object>();

        Tables tables = new Tables();

        TextAsset ReadData(string fileName)
        {
            //AsyncOperationHandle<TextAsset> csvHandle = Addressables.LoadAssetAsync<TextAsset>(fileName);
            //TextAsset res = csvHandle.WaitForCompletion();
            //Addressables.Release(csvHandle);
            //return res;


            return GameModule.Resource.LoadAsset<TextAsset>(fileName, false, true);
        }

        public T GetVOData<T>(string fileName) where T : IVOFun, new()
        {
            if (tableDict.ContainsKey(fileName))
            {
                return (T)tableDict[fileName];
            }
            else
            {
                var data = new T();
                TextAsset text = ReadData(fileName);
                data.LoadData(tables, new ByteBuf(text.bytes));
                tableDict.Add(fileName, data);

                return data;
            }
        }
    }
}
