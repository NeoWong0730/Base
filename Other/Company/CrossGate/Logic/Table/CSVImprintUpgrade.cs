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

	sealed public partial class CSVImprintUpgrade : Framework.Table.TableBase<CSVImprintUpgrade.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Node_ID;
			public readonly List<List<uint>> Attribute_Bonus;
			public readonly List<uint> Target_Type;
			public readonly List<List<uint>> Consume_Item;
			public readonly uint Consume_Coin;
			public readonly uint score;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Node_ID = ReadHelper.ReadUInt(binaryReader);
				Attribute_Bonus = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				Target_Type = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Consume_Item = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				Consume_Coin = ReadHelper.ReadUInt(binaryReader);
				score = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVImprintUpgrade.bytes";
		}

		private static CSVImprintUpgrade instance = null;			
		public static CSVImprintUpgrade Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVImprintUpgrade 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVImprintUpgrade forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVImprintUpgrade();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVImprintUpgrade");

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

    sealed public partial class CSVImprintUpgrade : FCSVImprintUpgrade
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVImprintUpgrade.bytes";
		}

		private static CSVImprintUpgrade instance = null;			
		public static CSVImprintUpgrade Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVImprintUpgrade 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVImprintUpgrade forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVImprintUpgrade();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVImprintUpgrade");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}