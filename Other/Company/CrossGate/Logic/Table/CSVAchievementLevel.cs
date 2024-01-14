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

	sealed public partial class CSVAchievementLevel : Framework.Table.TableBase<CSVAchievementLevel.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Level;
			public readonly uint Level_Test;
			public readonly uint Need_Points;
			public readonly uint Drop_Id;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Level = ReadHelper.ReadUInt(binaryReader);
				Level_Test = ReadHelper.ReadUInt(binaryReader);
				Need_Points = ReadHelper.ReadUInt(binaryReader);
				Drop_Id = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVAchievementLevel.bytes";
		}

		private static CSVAchievementLevel instance = null;			
		public static CSVAchievementLevel Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAchievementLevel 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAchievementLevel forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAchievementLevel();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAchievementLevel");

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

    sealed public partial class CSVAchievementLevel : FCSVAchievementLevel
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVAchievementLevel.bytes";
		}

		private static CSVAchievementLevel instance = null;			
		public static CSVAchievementLevel Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAchievementLevel 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAchievementLevel forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAchievementLevel();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAchievementLevel");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}