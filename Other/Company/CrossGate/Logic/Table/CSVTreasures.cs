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

	sealed public partial class CSVTreasures : Framework.Table.TableBase<CSVTreasures.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint name_id;
			public readonly uint sort_id;
			public readonly uint unlock_des_id;
			public readonly uint des_id;
			public readonly uint icon_id;
			public readonly uint level;
			public readonly uint unlock_exp;
			public readonly List<List<uint>> display_attr;
			public readonly uint score;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				name_id = ReadHelper.ReadUInt(binaryReader);
				sort_id = ReadHelper.ReadUInt(binaryReader);
				unlock_des_id = ReadHelper.ReadUInt(binaryReader);
				des_id = ReadHelper.ReadUInt(binaryReader);
				icon_id = ReadHelper.ReadUInt(binaryReader);
				level = ReadHelper.ReadUInt(binaryReader);
				unlock_exp = ReadHelper.ReadUInt(binaryReader);
				display_attr = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				score = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTreasures.bytes";
		}

		private static CSVTreasures instance = null;			
		public static CSVTreasures Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTreasures 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTreasures forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTreasures();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTreasures");

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

    sealed public partial class CSVTreasures : FCSVTreasures
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTreasures.bytes";
		}

		private static CSVTreasures instance = null;			
		public static CSVTreasures Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTreasures 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTreasures forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTreasures();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTreasures");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}