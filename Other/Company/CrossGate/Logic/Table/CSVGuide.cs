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

	sealed public partial class CSVGuide : Framework.Table.TableBase<CSVGuide.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint UI_id;
			public readonly string Animation_path;
			public readonly string Motion;
			public readonly uint force;
			public readonly float auto_time;
			public readonly string effect;
			public readonly uint effect_type;
			public readonly uint finish_type;
			public readonly uint prefab_type;
			public readonly List<uint> prefab_range;
			public readonly uint tippos_type;
			public readonly List<uint> Location;
			public readonly List<string> prefab_path;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				UI_id = ReadHelper.ReadUInt(binaryReader);
				Animation_path = shareData.GetShareData<string>(binaryReader, 0);
				Motion = shareData.GetShareData<string>(binaryReader, 0);
				force = ReadHelper.ReadUInt(binaryReader);
				auto_time = ReadHelper.ReadFloat(binaryReader);
				effect = shareData.GetShareData<string>(binaryReader, 0);
				effect_type = ReadHelper.ReadUInt(binaryReader);
				finish_type = ReadHelper.ReadUInt(binaryReader);
				prefab_type = ReadHelper.ReadUInt(binaryReader);
				prefab_range = shareData.GetShareData<List<uint>>(binaryReader, 1);
				tippos_type = ReadHelper.ReadUInt(binaryReader);
				Location = shareData.GetShareData<List<uint>>(binaryReader, 1);
				prefab_path = shareData.GetShareData<List<string>>(binaryReader, 2);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVGuide.bytes";
		}

		private static CSVGuide instance = null;			
		public static CSVGuide Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVGuide 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVGuide forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVGuide();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVGuide");

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
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadStringArrays(binaryReader, 2, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVGuide : FCSVGuide
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVGuide.bytes";
		}

		private static CSVGuide instance = null;			
		public static CSVGuide Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVGuide 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVGuide forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVGuide();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVGuide");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}