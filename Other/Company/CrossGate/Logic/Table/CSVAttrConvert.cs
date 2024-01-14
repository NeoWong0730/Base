//
//#define USE_HOTFIX_LOGIC

using Lib.AssetLoader;
using Lib.Core;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Framework.Table;

namespace Table
{
#if USE_HOTFIX_LOGIC

	sealed public partial class CSVAttrConvert : Framework.Table.TableBase<CSVAttrConvert.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly List<List<int>> vtl_convert;
			public readonly List<List<int>> str_convert;
			public readonly List<List<int>> tgh_convert;
			public readonly List<List<int>> qui_convert;
			public readonly List<List<int>> mgc_convert;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				vtl_convert = shareData.GetShareData<List<List<int>>>(binaryReader, 1);
				str_convert = shareData.GetShareData<List<List<int>>>(binaryReader, 1);
				tgh_convert = shareData.GetShareData<List<List<int>>>(binaryReader, 1);
				qui_convert = shareData.GetShareData<List<List<int>>>(binaryReader, 1);
				mgc_convert = shareData.GetShareData<List<List<int>>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVAttrConvert.bytes";
		}

		private static CSVAttrConvert instance = null;			
		public static CSVAttrConvert Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAttrConvert 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAttrConvert forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAttrConvert();
                instance.ReadByFilePath(ConfigPath(), OnCreat, OnReadShareData);
            }
            else if (forceReload)
            {
                instance.Clear();
                instance.ReadByFilePath(ConfigPath(), OnCreat, OnReadShareData);
            }
        }

        public static void Unload()
        {
            DebugUtil.Log(ELogType.eTable, "卸载CSVAttrConvert");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }

        private static Data OnCreat(uint id, BinaryReader binaryReader, TableShareData shareData)
        {
            Data data = new Data(id, binaryReader, shareData);
            return data;
        }

        private static TableShareData OnReadShareData(BinaryReader binaryReader)
		{
			TableShareData shareData = new TableShareData(2);
			shareData.ReadArrays<int>(binaryReader, 0, ReadHelper.ReadArray_ReadInt);
			shareData.ReadArray2s<int>(binaryReader, 1, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVAttrConvert : FCSVAttrConvert
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVAttrConvert.bytes";
		}

		private static CSVAttrConvert instance = null;			
		public static CSVAttrConvert Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAttrConvert 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAttrConvert forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAttrConvert();
                instance.ReadByFilePath(ConfigPath());
            }
            else if (forceReload)
            {
                instance.Clear();
                instance.ReadByFilePath(ConfigPath());
            }
        }

        public static void Unload()
        {
            DebugUtil.Log(ELogType.eTable, "卸载CSVAttrConvert");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}