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

	sealed public partial class CSVTreasureRaider : Framework.Table.TableBase<CSVTreasureRaider.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint group;
			public readonly uint startTime;
			public readonly uint endTime;
			public readonly uint prizeTime;
			public readonly uint type;
			public readonly uint gameReward;
			public readonly uint inKindReward;
			public readonly uint priceType;
			public readonly uint price;
			public readonly uint priceID;
			public readonly uint refundType;
			public readonly uint refund;
			public readonly uint winNumber;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				group = ReadHelper.ReadUInt(binaryReader);
				startTime = ReadHelper.ReadUInt(binaryReader);
				endTime = ReadHelper.ReadUInt(binaryReader);
				prizeTime = ReadHelper.ReadUInt(binaryReader);
				type = ReadHelper.ReadUInt(binaryReader);
				gameReward = ReadHelper.ReadUInt(binaryReader);
				inKindReward = ReadHelper.ReadUInt(binaryReader);
				priceType = ReadHelper.ReadUInt(binaryReader);
				price = ReadHelper.ReadUInt(binaryReader);
				priceID = ReadHelper.ReadUInt(binaryReader);
				refundType = ReadHelper.ReadUInt(binaryReader);
				refund = ReadHelper.ReadUInt(binaryReader);
				winNumber = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTreasureRaider.bytes";
		}

		private static CSVTreasureRaider instance = null;			
		public static CSVTreasureRaider Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTreasureRaider 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTreasureRaider forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTreasureRaider();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTreasureRaider");

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

    sealed public partial class CSVTreasureRaider : FCSVTreasureRaider
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTreasureRaider.bytes";
		}

		private static CSVTreasureRaider instance = null;			
		public static CSVTreasureRaider Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTreasureRaider 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTreasureRaider forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTreasureRaider();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTreasureRaider");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}