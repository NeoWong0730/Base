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

	sealed public partial class CSVBattlePassPurchase : Framework.Table.TableBase<CSVBattlePassPurchase.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Title_Des;
			public readonly uint Activation_Des;
			public readonly uint Icon_ID;
			public readonly uint Drop_Reward;
			public readonly uint Price;
			public readonly uint Extra_Level;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Title_Des = ReadHelper.ReadUInt(binaryReader);
				Activation_Des = ReadHelper.ReadUInt(binaryReader);
				Icon_ID = ReadHelper.ReadUInt(binaryReader);
				Drop_Reward = ReadHelper.ReadUInt(binaryReader);
				Price = ReadHelper.ReadUInt(binaryReader);
				Extra_Level = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVBattlePassPurchase.bytes";
		}

		private static CSVBattlePassPurchase instance = null;			
		public static CSVBattlePassPurchase Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBattlePassPurchase 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBattlePassPurchase forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBattlePassPurchase();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBattlePassPurchase");

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

    sealed public partial class CSVBattlePassPurchase : FCSVBattlePassPurchase
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVBattlePassPurchase.bytes";
		}

		private static CSVBattlePassPurchase instance = null;			
		public static CSVBattlePassPurchase Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBattlePassPurchase 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBattlePassPurchase forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBattlePassPurchase();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBattlePassPurchase");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}