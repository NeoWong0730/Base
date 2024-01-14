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

	sealed public partial class CSVMapExplorationMark : Framework.Table.TableBase<CSVMapExplorationMark.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint function_id;
			public readonly uint resource_icon1;
			public readonly uint resource_icon2;
			public readonly uint type_lan;
			public readonly uint List_lan;
			public readonly uint Des_lan;
			public readonly uint List_Icon;
			public readonly uint main_icon;
			public readonly byte tab_type;
			public bool allow_click { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				function_id = ReadHelper.ReadUInt(binaryReader);
				resource_icon1 = ReadHelper.ReadUInt(binaryReader);
				resource_icon2 = ReadHelper.ReadUInt(binaryReader);
				type_lan = ReadHelper.ReadUInt(binaryReader);
				List_lan = ReadHelper.ReadUInt(binaryReader);
				Des_lan = ReadHelper.ReadUInt(binaryReader);
				List_Icon = ReadHelper.ReadUInt(binaryReader);
				main_icon = ReadHelper.ReadUInt(binaryReader);
				tab_type = ReadHelper.ReadByte(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVMapExplorationMark.bytes";
		}

		private static CSVMapExplorationMark instance = null;			
		public static CSVMapExplorationMark Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVMapExplorationMark 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVMapExplorationMark forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVMapExplorationMark();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVMapExplorationMark");

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
			TableShareData shareData = null;

			return shareData;
		}
	}

#else

    sealed public partial class CSVMapExplorationMark : FCSVMapExplorationMark
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVMapExplorationMark.bytes";
		}

		private static CSVMapExplorationMark instance = null;			
		public static CSVMapExplorationMark Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVMapExplorationMark 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVMapExplorationMark forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVMapExplorationMark();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVMapExplorationMark");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}