//CSVWordStyle
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

	sealed public partial class CSVAnnouncement : Framework.Table.TableBase<CSVAnnouncement.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Type;
			public readonly uint Priority;
			public readonly uint wordStyle;
			public readonly uint LvShow;
			public readonly uint MissionShow;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Type = ReadHelper.ReadUInt(binaryReader);
				Priority = ReadHelper.ReadUInt(binaryReader);
				wordStyle = ReadHelper.ReadUInt(binaryReader);
				LvShow = ReadHelper.ReadUInt(binaryReader);
				MissionShow = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVAnnouncement.bytes";
		}

		private static CSVAnnouncement instance = null;			
		public static CSVAnnouncement Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAnnouncement 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAnnouncement forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAnnouncement();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAnnouncement");

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

    sealed public partial class CSVAnnouncement : FCSVAnnouncement
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVAnnouncement.bytes";
		}

		private static CSVAnnouncement instance = null;			
		public static CSVAnnouncement Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAnnouncement 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAnnouncement forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAnnouncement();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAnnouncement");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}