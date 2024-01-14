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

	sealed public partial class CSVRaceChangeCard : Framework.Table.TableBase<CSVRaceChangeCard.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint attr_skill_id;
			public readonly uint type;
			public readonly uint career;
			public readonly uint need_race_lv;
			public readonly uint change_id;
			public readonly uint last_time;
			public readonly List<List<int>> base_attr;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				attr_skill_id = ReadHelper.ReadUInt(binaryReader);
				type = ReadHelper.ReadUInt(binaryReader);
				career = ReadHelper.ReadUInt(binaryReader);
				need_race_lv = ReadHelper.ReadUInt(binaryReader);
				change_id = ReadHelper.ReadUInt(binaryReader);
				last_time = ReadHelper.ReadUInt(binaryReader);
				base_attr = shareData.GetShareData<List<List<int>>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVRaceChangeCard.bytes";
		}

		private static CSVRaceChangeCard instance = null;			
		public static CSVRaceChangeCard Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVRaceChangeCard 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVRaceChangeCard forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVRaceChangeCard();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVRaceChangeCard");

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

    sealed public partial class CSVRaceChangeCard : FCSVRaceChangeCard
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVRaceChangeCard.bytes";
		}

		private static CSVRaceChangeCard instance = null;			
		public static CSVRaceChangeCard Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVRaceChangeCard 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVRaceChangeCard forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVRaceChangeCard();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVRaceChangeCard");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}