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

	sealed public partial class CSVDailyActivityShow : Framework.Table.TableBase<CSVDailyActivityShow.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint ActiveTypeName;
			public readonly uint Activeicon;
			public readonly uint Customize;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				ActiveTypeName = ReadHelper.ReadUInt(binaryReader);
				Activeicon = ReadHelper.ReadUInt(binaryReader);
				Customize = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVDailyActivityShow.bytes";
		}

		private static CSVDailyActivityShow instance = null;			
		public static CSVDailyActivityShow Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVDailyActivityShow 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVDailyActivityShow forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVDailyActivityShow();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVDailyActivityShow");

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

    sealed public partial class CSVDailyActivityShow : FCSVDailyActivityShow
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVDailyActivityShow.bytes";
		}

		private static CSVDailyActivityShow instance = null;			
		public static CSVDailyActivityShow Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVDailyActivityShow 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVDailyActivityShow forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVDailyActivityShow();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVDailyActivityShow");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}