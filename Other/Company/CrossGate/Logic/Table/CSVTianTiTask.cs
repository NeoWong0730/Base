﻿//
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

	sealed public partial class CSVTianTiTask : Framework.Table.TableBase<CSVTianTiTask.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Task_Des;
			public readonly uint Reward;
			public readonly uint TypeAchievement;
			public readonly List<uint> ReachTypeAchievement;
			public readonly uint MissionType;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Task_Des = ReadHelper.ReadUInt(binaryReader);
				Reward = ReadHelper.ReadUInt(binaryReader);
				TypeAchievement = ReadHelper.ReadUInt(binaryReader);
				ReachTypeAchievement = shareData.GetShareData<List<uint>>(binaryReader, 0);
				MissionType = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTianTiTask.bytes";
		}

		private static CSVTianTiTask instance = null;			
		public static CSVTianTiTask Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTianTiTask 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTianTiTask forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTianTiTask();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTianTiTask");

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

    sealed public partial class CSVTianTiTask : FCSVTianTiTask
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTianTiTask.bytes";
		}

		private static CSVTianTiTask instance = null;			
		public static CSVTianTiTask Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTianTiTask 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTianTiTask forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTianTiTask();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTianTiTask");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}