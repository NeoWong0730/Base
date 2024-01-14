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

	sealed public partial class CSVRaceChangeGroup : Framework.Table.TableBase<CSVRaceChangeGroup.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint group_id;
			public readonly uint weight;
			public readonly uint skill_id;
			public readonly uint min_level;
			public readonly uint max_level;
			public readonly uint mutex;
			public readonly List<uint> career_restrict;
			public readonly List<uint> race_restrict;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				group_id = ReadHelper.ReadUInt(binaryReader);
				weight = ReadHelper.ReadUInt(binaryReader);
				skill_id = ReadHelper.ReadUInt(binaryReader);
				min_level = ReadHelper.ReadUInt(binaryReader);
				max_level = ReadHelper.ReadUInt(binaryReader);
				mutex = ReadHelper.ReadUInt(binaryReader);
				career_restrict = shareData.GetShareData<List<uint>>(binaryReader, 0);
				race_restrict = shareData.GetShareData<List<uint>>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVRaceChangeGroup.bytes";
		}

		private static CSVRaceChangeGroup instance = null;			
		public static CSVRaceChangeGroup Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVRaceChangeGroup 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVRaceChangeGroup forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVRaceChangeGroup();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVRaceChangeGroup");

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
			TableShareData shareData = new TableShareData(1);
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVRaceChangeGroup : FCSVRaceChangeGroup
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVRaceChangeGroup.bytes";
		}

		private static CSVRaceChangeGroup instance = null;			
		public static CSVRaceChangeGroup Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVRaceChangeGroup 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVRaceChangeGroup forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVRaceChangeGroup();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVRaceChangeGroup");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}