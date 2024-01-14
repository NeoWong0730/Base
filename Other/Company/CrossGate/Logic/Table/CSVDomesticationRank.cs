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

	sealed public partial class CSVDomesticationRank : Framework.Table.TableBase<CSVDomesticationRank.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint dan;
			public readonly uint name;
			public readonly uint level;
			public readonly uint lvup_cost;
			public readonly List<List<uint>> lv_attr;
			public readonly uint lv_score;
			public readonly uint exp_reduce;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				dan = ReadHelper.ReadUInt(binaryReader);
				name = ReadHelper.ReadUInt(binaryReader);
				level = ReadHelper.ReadUInt(binaryReader);
				lvup_cost = ReadHelper.ReadUInt(binaryReader);
				lv_attr = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				lv_score = ReadHelper.ReadUInt(binaryReader);
				exp_reduce = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVDomesticationRank.bytes";
		}

		private static CSVDomesticationRank instance = null;			
		public static CSVDomesticationRank Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVDomesticationRank 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVDomesticationRank forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVDomesticationRank();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVDomesticationRank");

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

    sealed public partial class CSVDomesticationRank : FCSVDomesticationRank
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVDomesticationRank.bytes";
		}

		private static CSVDomesticationRank instance = null;			
		public static CSVDomesticationRank Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVDomesticationRank 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVDomesticationRank forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVDomesticationRank();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVDomesticationRank");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}