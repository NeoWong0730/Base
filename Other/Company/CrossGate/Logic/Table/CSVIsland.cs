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

	sealed public partial class CSVIsland : Framework.Table.TableBase<CSVIsland.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly int island_type;
			public readonly uint name;
			public readonly uint mapid;
			public readonly List<int> map_lv;
			public readonly string map_ui;
			public readonly List<uint> subIds;
			public readonly List<uint> islandid;
			public readonly uint icon;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				island_type = ReadHelper.ReadInt(binaryReader);
				name = ReadHelper.ReadUInt(binaryReader);
				mapid = ReadHelper.ReadUInt(binaryReader);
				map_lv = shareData.GetShareData<List<int>>(binaryReader, 1);
				map_ui = shareData.GetShareData<string>(binaryReader, 0);
				subIds = shareData.GetShareData<List<uint>>(binaryReader, 2);
				islandid = shareData.GetShareData<List<uint>>(binaryReader, 2);
				icon = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVIsland.bytes";
		}

		private static CSVIsland instance = null;			
		public static CSVIsland Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVIsland 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVIsland forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVIsland();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVIsland");

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
			TableShareData shareData = new TableShareData(3);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<int>(binaryReader, 1, ReadHelper.ReadArray_ReadInt);
			shareData.ReadArrays<uint>(binaryReader, 2, ReadHelper.ReadArray_ReadUInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVIsland : FCSVIsland
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVIsland.bytes";
		}

		private static CSVIsland instance = null;			
		public static CSVIsland Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVIsland 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVIsland forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVIsland();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVIsland");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}