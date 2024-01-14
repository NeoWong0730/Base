//
#define USE_HOTFIX_LOGIC

using Lib.AssetLoader;
using Lib.Core;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Framework.Table;

namespace Table
{
#if USE_HOTFIX_LOGIC

	sealed public partial class CSVAttrBoost : Framework.Table.TableBase<CSVAttrBoost.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint attr_id;
			public readonly List<List<uint>> limit_boost_lv;
			public readonly uint limit_boost_max;
			public readonly List<List<uint>> boost_value_cost;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				attr_id = ReadHelper.ReadUInt(binaryReader);
				limit_boost_lv = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				limit_boost_max = ReadHelper.ReadUInt(binaryReader);
				boost_value_cost = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVAttrBoost.bytes";
		}

		private static CSVAttrBoost instance = null;			
		public static CSVAttrBoost Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAttrBoost 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAttrBoost forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAttrBoost();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAttrBoost");

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
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<uint>(binaryReader, 1, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVAttrBoost : FCSVAttrBoost
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVAttrBoost.bytes";
		}

		private static CSVAttrBoost instance = null;			
		public static CSVAttrBoost Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAttrBoost 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAttrBoost forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAttrBoost();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAttrBoost");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}