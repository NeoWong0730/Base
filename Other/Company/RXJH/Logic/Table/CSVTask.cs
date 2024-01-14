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

	sealed public partial class CSVTask : Framework.Table.TableBase<CSVTask.Data>
	{
	    sealed public partial class Data
	    {
			public readonly uint id; // 任务
			public readonly uint taskType; // 任务类型
			public readonly uint taskName; // 任务名称
			public readonly List<List<uint>> taskTarget; // 任务目标
			public readonly uint acceptLv; // 接受等级
			public readonly uint acceptTransfer; // 接受转职
			public readonly uint acceptFactions; // 接受正邪
			public readonly uint acceptCondition; // 接取条件
			public readonly uint taskReward; // 任务奖励
			public readonly uint itemId; // 特殊奖励


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				taskType = ReadHelper.ReadUInt(binaryReader);
				taskName = ReadHelper.ReadUInt(binaryReader);
				taskTarget = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				acceptLv = ReadHelper.ReadUInt(binaryReader);
				acceptTransfer = ReadHelper.ReadUInt(binaryReader);
				acceptFactions = ReadHelper.ReadUInt(binaryReader);
				acceptCondition = ReadHelper.ReadUInt(binaryReader);
				taskReward = ReadHelper.ReadUInt(binaryReader);
				itemId = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTask.bytes";
		}

		private static CSVTask instance = null;			
		public static CSVTask Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTask 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTask forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTask();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTask");

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

    sealed public partial class CSVTask : FCSVTask
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTask.bytes";
		}

		private static CSVTask instance = null;			
		public static CSVTask Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTask 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTask forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTask();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTask");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}