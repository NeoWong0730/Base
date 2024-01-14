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

	sealed public partial class CSVReturnChanllengeTask : Framework.Table.TableBase<CSVReturnChanllengeTask.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Activity_Group;
			public readonly uint RankType;
			public readonly uint Priority;
			public readonly uint TypeAchievement;
			public readonly List<uint> ReachTypeAchievement;
			public readonly List<uint> Change_UI;
			public readonly uint Dropid;
			public readonly uint Points;
			public readonly uint Task_Des;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Activity_Group = ReadHelper.ReadUInt(binaryReader);
				RankType = ReadHelper.ReadUInt(binaryReader);
				Priority = ReadHelper.ReadUInt(binaryReader);
				TypeAchievement = ReadHelper.ReadUInt(binaryReader);
				ReachTypeAchievement = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Change_UI = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Dropid = ReadHelper.ReadUInt(binaryReader);
				Points = ReadHelper.ReadUInt(binaryReader);
				Task_Des = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVReturnChanllengeTask.bytes";
		}

		private static CSVReturnChanllengeTask instance = null;			
		public static CSVReturnChanllengeTask Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVReturnChanllengeTask 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVReturnChanllengeTask forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVReturnChanllengeTask();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVReturnChanllengeTask");

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

    sealed public partial class CSVReturnChanllengeTask : FCSVReturnChanllengeTask
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVReturnChanllengeTask.bytes";
		}

		private static CSVReturnChanllengeTask instance = null;			
		public static CSVReturnChanllengeTask Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVReturnChanllengeTask 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVReturnChanllengeTask forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVReturnChanllengeTask();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVReturnChanllengeTask");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}