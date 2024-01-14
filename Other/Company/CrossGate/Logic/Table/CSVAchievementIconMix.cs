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

	sealed public partial class CSVAchievementIconMix : Framework.Table.TableBase<CSVAchievementIconMix.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint IconId1;
			public readonly uint IconId2;
			public readonly uint IconId3;
			public readonly uint IconId4;
			public readonly uint IconId5;
			public readonly uint IconId6;
			public readonly uint IconId7;
			public readonly uint IconId8;
			public readonly uint IconId9;
			public readonly uint IconId10;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				IconId1 = ReadHelper.ReadUInt(binaryReader);
				IconId2 = ReadHelper.ReadUInt(binaryReader);
				IconId3 = ReadHelper.ReadUInt(binaryReader);
				IconId4 = ReadHelper.ReadUInt(binaryReader);
				IconId5 = ReadHelper.ReadUInt(binaryReader);
				IconId6 = ReadHelper.ReadUInt(binaryReader);
				IconId7 = ReadHelper.ReadUInt(binaryReader);
				IconId8 = ReadHelper.ReadUInt(binaryReader);
				IconId9 = ReadHelper.ReadUInt(binaryReader);
				IconId10 = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVAchievementIconMix.bytes";
		}

		private static CSVAchievementIconMix instance = null;			
		public static CSVAchievementIconMix Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAchievementIconMix 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAchievementIconMix forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAchievementIconMix();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAchievementIconMix");

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

    sealed public partial class CSVAchievementIconMix : FCSVAchievementIconMix
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVAchievementIconMix.bytes";
		}

		private static CSVAchievementIconMix instance = null;			
		public static CSVAchievementIconMix Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAchievementIconMix 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAchievementIconMix forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAchievementIconMix();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAchievementIconMix");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}