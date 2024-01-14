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

	sealed public partial class CSVShopItem : Framework.Table.TableBase<CSVShopItem.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint shop_id;
			public readonly uint item_id;
			public readonly uint produce_id;
			public readonly uint weight;
			public readonly uint need_func_id;
			public readonly uint level_require;
			public readonly List<List<uint>> likability;
			public readonly uint need_card;
			public bool need_senior_BP { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly uint need_BP_lv;
			public readonly uint tutor_level;
			public readonly uint need_rank;
			public readonly List<uint> need_task;
			public readonly uint price_type;
			public readonly uint price_now;
			public readonly uint price_before;
			public readonly uint min_price;
			public readonly uint max_price;
			public readonly uint limit_type;
			public readonly uint server_limit;
			public readonly uint personal_limit;
			public readonly int frozen_time;
			public readonly uint random_amount;
			public readonly uint perPurchase_limit_count;
			public bool red_dot { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
			public readonly List<uint> world_level;
			public readonly uint system_notice;
			public readonly uint need_race_lv;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				shop_id = ReadHelper.ReadUInt(binaryReader);
				item_id = ReadHelper.ReadUInt(binaryReader);
				produce_id = ReadHelper.ReadUInt(binaryReader);
				weight = ReadHelper.ReadUInt(binaryReader);
				need_func_id = ReadHelper.ReadUInt(binaryReader);
				level_require = ReadHelper.ReadUInt(binaryReader);
				likability = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				need_card = ReadHelper.ReadUInt(binaryReader);
				need_BP_lv = ReadHelper.ReadUInt(binaryReader);
				tutor_level = ReadHelper.ReadUInt(binaryReader);
				need_rank = ReadHelper.ReadUInt(binaryReader);
				need_task = shareData.GetShareData<List<uint>>(binaryReader, 0);
				price_type = ReadHelper.ReadUInt(binaryReader);
				price_now = ReadHelper.ReadUInt(binaryReader);
				price_before = ReadHelper.ReadUInt(binaryReader);
				min_price = ReadHelper.ReadUInt(binaryReader);
				max_price = ReadHelper.ReadUInt(binaryReader);
				limit_type = ReadHelper.ReadUInt(binaryReader);
				server_limit = ReadHelper.ReadUInt(binaryReader);
				personal_limit = ReadHelper.ReadUInt(binaryReader);
				frozen_time = ReadHelper.ReadInt(binaryReader);
				random_amount = ReadHelper.ReadUInt(binaryReader);
				perPurchase_limit_count = ReadHelper.ReadUInt(binaryReader);
				world_level = shareData.GetShareData<List<uint>>(binaryReader, 0);
				system_notice = ReadHelper.ReadUInt(binaryReader);
				need_race_lv = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVShopItem.bytes";
		}

		private static CSVShopItem instance = null;			
		public static CSVShopItem Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVShopItem 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVShopItem forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVShopItem();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVShopItem");

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
			TableShareData shareData = new TableShareData(2);
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<uint>(binaryReader, 1, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVShopItem : FCSVShopItem
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVShopItem.bytes";
		}

		private static CSVShopItem instance = null;			
		public static CSVShopItem Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVShopItem 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVShopItem forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVShopItem();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVShopItem");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}