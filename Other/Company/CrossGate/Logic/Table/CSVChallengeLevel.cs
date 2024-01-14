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

	sealed public partial class CSVChallengeLevel : Framework.Table.TableBase<CSVChallengeLevel.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint rule_name;
			public readonly uint difficulty_level;
			public readonly List<uint> challengeLevelLimit;
			public readonly List<uint> BOSS_id;
			public readonly uint island_id;
			public readonly uint rulePage_leve;
			public readonly uint rulePage_description;
			public readonly uint rulePage_time;
			public readonly uint rulePage_rewardTimes;
			public bool challengeLevelIsRanking { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				rule_name = ReadHelper.ReadUInt(binaryReader);
				difficulty_level = ReadHelper.ReadUInt(binaryReader);
				challengeLevelLimit = shareData.GetShareData<List<uint>>(binaryReader, 0);
				BOSS_id = shareData.GetShareData<List<uint>>(binaryReader, 0);
				island_id = ReadHelper.ReadUInt(binaryReader);
				rulePage_leve = ReadHelper.ReadUInt(binaryReader);
				rulePage_description = ReadHelper.ReadUInt(binaryReader);
				rulePage_time = ReadHelper.ReadUInt(binaryReader);
				rulePage_rewardTimes = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVChallengeLevel.bytes";
		}

		private static CSVChallengeLevel instance = null;			
		public static CSVChallengeLevel Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVChallengeLevel 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVChallengeLevel forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVChallengeLevel();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVChallengeLevel");

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

    sealed public partial class CSVChallengeLevel : FCSVChallengeLevel
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVChallengeLevel.bytes";
		}

		private static CSVChallengeLevel instance = null;			
		public static CSVChallengeLevel Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVChallengeLevel 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVChallengeLevel forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVChallengeLevel();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVChallengeLevel");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}