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

	sealed public partial class CSVBOOSFightPlayMode : Framework.Table.TableBase<CSVBOOSFightPlayMode.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint playMode;
			public readonly List<uint> difficulty_id;
			public readonly List<uint> playModeLevelLimit;
			public readonly uint dailyActivites;
			public readonly uint rewardLimit;
			public readonly uint rewardLimitDay;
			public readonly uint teamPlayModeId;
			public readonly uint headIcon_id;
			public readonly Color32 themeColor;
			public bool playModeIsRanking { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public bool isShowShop { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				playMode = ReadHelper.ReadUInt(binaryReader);
				difficulty_id = shareData.GetShareData<List<uint>>(binaryReader, 0);
				playModeLevelLimit = shareData.GetShareData<List<uint>>(binaryReader, 0);
				dailyActivites = ReadHelper.ReadUInt(binaryReader);
				rewardLimit = ReadHelper.ReadUInt(binaryReader);
				rewardLimitDay = ReadHelper.ReadUInt(binaryReader);
				teamPlayModeId = ReadHelper.ReadUInt(binaryReader);
				headIcon_id = ReadHelper.ReadUInt(binaryReader);
				themeColor = ReadHelper.ReadColor32(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVBOOSFightPlayMode.bytes";
		}

		private static CSVBOOSFightPlayMode instance = null;			
		public static CSVBOOSFightPlayMode Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBOOSFightPlayMode 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBOOSFightPlayMode forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBOOSFightPlayMode();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBOOSFightPlayMode");

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

    sealed public partial class CSVBOOSFightPlayMode : FCSVBOOSFightPlayMode
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVBOOSFightPlayMode.bytes";
		}

		private static CSVBOOSFightPlayMode instance = null;			
		public static CSVBOOSFightPlayMode Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBOOSFightPlayMode 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBOOSFightPlayMode forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBOOSFightPlayMode();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBOOSFightPlayMode");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}