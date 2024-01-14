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

	sealed public partial class CSVChapterSysTask : Framework.Table.TableBase<CSVChapterSysTask.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint TaskName;
			public readonly uint TaskNameLag;
			public readonly uint Chapter;
			public readonly uint ChapterSys;
			public readonly uint Type;
			public readonly uint Reach;
			public readonly uint GhidanceGroup;
			public readonly List<uint> SideQuest;
			public readonly uint TypeAchievement;
			public readonly List<uint> ReachTypeAchievement;
			public readonly uint MainDisplay;
			public readonly uint DropWatchId;
			public readonly uint TryAgain;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				TaskName = ReadHelper.ReadUInt(binaryReader);
				TaskNameLag = ReadHelper.ReadUInt(binaryReader);
				Chapter = ReadHelper.ReadUInt(binaryReader);
				ChapterSys = ReadHelper.ReadUInt(binaryReader);
				Type = ReadHelper.ReadUInt(binaryReader);
				Reach = ReadHelper.ReadUInt(binaryReader);
				GhidanceGroup = ReadHelper.ReadUInt(binaryReader);
				SideQuest = shareData.GetShareData<List<uint>>(binaryReader, 0);
				TypeAchievement = ReadHelper.ReadUInt(binaryReader);
				ReachTypeAchievement = shareData.GetShareData<List<uint>>(binaryReader, 0);
				MainDisplay = ReadHelper.ReadUInt(binaryReader);
				DropWatchId = ReadHelper.ReadUInt(binaryReader);
				TryAgain = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVChapterSysTask.bytes";
		}

		private static CSVChapterSysTask instance = null;			
		public static CSVChapterSysTask Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVChapterSysTask 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVChapterSysTask forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVChapterSysTask();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVChapterSysTask");

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

    sealed public partial class CSVChapterSysTask : FCSVChapterSysTask
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVChapterSysTask.bytes";
		}

		private static CSVChapterSysTask instance = null;			
		public static CSVChapterSysTask Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVChapterSysTask 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVChapterSysTask forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVChapterSysTask();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVChapterSysTask");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}