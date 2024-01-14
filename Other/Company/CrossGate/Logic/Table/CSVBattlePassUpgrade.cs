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

	sealed public partial class CSVBattlePassUpgrade : Framework.Table.TableBase<CSVBattlePassUpgrade.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Branch_ID;
			public readonly uint Base_Reward;
			public readonly uint Advanced_reward;
			public readonly uint Reward_Type;
			public readonly uint Level;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Branch_ID = ReadHelper.ReadUInt(binaryReader);
				Base_Reward = ReadHelper.ReadUInt(binaryReader);
				Advanced_reward = ReadHelper.ReadUInt(binaryReader);
				Reward_Type = ReadHelper.ReadUInt(binaryReader);
				Level = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVBattlePassUpgrade.bytes";
		}

		private static CSVBattlePassUpgrade instance = null;			
		public static CSVBattlePassUpgrade Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBattlePassUpgrade 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBattlePassUpgrade forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBattlePassUpgrade();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBattlePassUpgrade");

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

    sealed public partial class CSVBattlePassUpgrade : FCSVBattlePassUpgrade
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVBattlePassUpgrade.bytes";
		}

		private static CSVBattlePassUpgrade instance = null;			
		public static CSVBattlePassUpgrade Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBattlePassUpgrade 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBattlePassUpgrade forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBattlePassUpgrade();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBattlePassUpgrade");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}