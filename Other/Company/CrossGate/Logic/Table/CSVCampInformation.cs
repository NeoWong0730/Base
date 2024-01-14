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

	sealed public partial class CSVCampInformation : Framework.Table.TableBase<CSVCampInformation.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint camp_name;
			public readonly uint camp_description;
			public readonly uint campUnlocked_drop;
			public readonly uint camp_icon;
			public readonly uint camp_background;
			public readonly uint coreBoss_id;
			public readonly Color32 FontColor;
			public readonly Color32 ImageColor;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				camp_name = ReadHelper.ReadUInt(binaryReader);
				camp_description = ReadHelper.ReadUInt(binaryReader);
				campUnlocked_drop = ReadHelper.ReadUInt(binaryReader);
				camp_icon = ReadHelper.ReadUInt(binaryReader);
				camp_background = ReadHelper.ReadUInt(binaryReader);
				coreBoss_id = ReadHelper.ReadUInt(binaryReader);
				FontColor = ReadHelper.ReadColor32(binaryReader);
				ImageColor = ReadHelper.ReadColor32(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVCampInformation.bytes";
		}

		private static CSVCampInformation instance = null;			
		public static CSVCampInformation Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCampInformation 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCampInformation forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCampInformation();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCampInformation");

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

    sealed public partial class CSVCampInformation : FCSVCampInformation
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVCampInformation.bytes";
		}

		private static CSVCampInformation instance = null;			
		public static CSVCampInformation Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCampInformation 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCampInformation forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCampInformation();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCampInformation");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}