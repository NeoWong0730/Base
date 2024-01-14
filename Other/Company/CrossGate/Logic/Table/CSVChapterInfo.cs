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

	sealed public partial class CSVChapterInfo : Framework.Table.TableBase<CSVChapterInfo.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint category;
			public readonly uint ChapterNum;
			public readonly uint ChapterOpenlv;
			public readonly uint ChapterDescribe;
			public readonly uint ChapterTaskNum;
			public readonly uint DropWatchId;
			public readonly uint DropDescribe;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				category = ReadHelper.ReadUInt(binaryReader);
				ChapterNum = ReadHelper.ReadUInt(binaryReader);
				ChapterOpenlv = ReadHelper.ReadUInt(binaryReader);
				ChapterDescribe = ReadHelper.ReadUInt(binaryReader);
				ChapterTaskNum = ReadHelper.ReadUInt(binaryReader);
				DropWatchId = ReadHelper.ReadUInt(binaryReader);
				DropDescribe = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVChapterInfo.bytes";
		}

		private static CSVChapterInfo instance = null;			
		public static CSVChapterInfo Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVChapterInfo 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVChapterInfo forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVChapterInfo();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVChapterInfo");

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
			TableShareData shareData = null;

			return shareData;
		}
	}

#else

    sealed public partial class CSVChapterInfo : FCSVChapterInfo
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVChapterInfo.bytes";
		}

		private static CSVChapterInfo instance = null;			
		public static CSVChapterInfo Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVChapterInfo 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVChapterInfo forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVChapterInfo();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVChapterInfo");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}