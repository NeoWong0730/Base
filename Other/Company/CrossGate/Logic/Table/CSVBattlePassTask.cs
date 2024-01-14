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

	sealed public partial class CSVBattlePassTask : Framework.Table.TableBase<CSVBattlePassTask.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly List<uint> Level;
			public readonly List<uint> Task_Quantity;
			public readonly List<uint> Fixed_Task;
			public readonly List<uint> Fixed_Task_week;
			public readonly List<uint> Fixed_Task_Season;
			public readonly List<List<uint>> Daily_Task_GroupID;
			public readonly List<List<uint>> Weekly_Task_GroupID;
			public readonly List<List<uint>> Season_Task_GroupID;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Level = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Task_Quantity = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Fixed_Task = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Fixed_Task_week = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Fixed_Task_Season = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Daily_Task_GroupID = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				Weekly_Task_GroupID = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				Season_Task_GroupID = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVBattlePassTask.bytes";
		}

		private static CSVBattlePassTask instance = null;			
		public static CSVBattlePassTask Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBattlePassTask 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBattlePassTask forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBattlePassTask();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBattlePassTask");

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

    sealed public partial class CSVBattlePassTask : FCSVBattlePassTask
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVBattlePassTask.bytes";
		}

		private static CSVBattlePassTask instance = null;			
		public static CSVBattlePassTask Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBattlePassTask 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBattlePassTask forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBattlePassTask();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBattlePassTask");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}