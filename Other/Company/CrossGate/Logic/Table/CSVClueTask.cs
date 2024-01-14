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

	sealed public partial class CSVClueTask : Framework.Table.TableBase<CSVClueTask.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint TaskStar;
			public readonly uint TaskName;
			public readonly uint TaskDes;
			public readonly uint TaskType;
			public readonly uint TriggerCondition_PlayerLevel;
			public readonly uint TriggerCondition_DetectiveLevel;
			public readonly uint TriggerCondition_AdventureLevel;
			public readonly List<uint> TriggerCondition_FinishTasks;
			public readonly List<uint> TriggerCondition_UnlockMaps;
			public readonly uint TriggerCondition_Special;
			public readonly List<uint> PhasedTasksGroup;
			public readonly uint TaskCompleteDes;
			public readonly uint Reward;
			public readonly string BG;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				TaskStar = ReadHelper.ReadUInt(binaryReader);
				TaskName = ReadHelper.ReadUInt(binaryReader);
				TaskDes = ReadHelper.ReadUInt(binaryReader);
				TaskType = ReadHelper.ReadUInt(binaryReader);
				TriggerCondition_PlayerLevel = ReadHelper.ReadUInt(binaryReader);
				TriggerCondition_DetectiveLevel = ReadHelper.ReadUInt(binaryReader);
				TriggerCondition_AdventureLevel = ReadHelper.ReadUInt(binaryReader);
				TriggerCondition_FinishTasks = shareData.GetShareData<List<uint>>(binaryReader, 1);
				TriggerCondition_UnlockMaps = shareData.GetShareData<List<uint>>(binaryReader, 1);
				TriggerCondition_Special = ReadHelper.ReadUInt(binaryReader);
				PhasedTasksGroup = shareData.GetShareData<List<uint>>(binaryReader, 1);
				TaskCompleteDes = ReadHelper.ReadUInt(binaryReader);
				Reward = ReadHelper.ReadUInt(binaryReader);
				BG = shareData.GetShareData<string>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVClueTask.bytes";
		}

		private static CSVClueTask instance = null;			
		public static CSVClueTask Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVClueTask 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVClueTask forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVClueTask();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVClueTask");

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
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVClueTask : FCSVClueTask
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVClueTask.bytes";
		}

		private static CSVClueTask instance = null;			
		public static CSVClueTask Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVClueTask 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVClueTask forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVClueTask();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVClueTask");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}