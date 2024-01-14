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

	sealed public partial class CSVActivityShopGoods : Framework.Table.TableBase<CSVActivityShopGoods.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Item_Number;
			public readonly uint Activity_Id;
			public readonly uint item_id;
			public readonly uint produce_id;
			public readonly uint need_func_id;
			public readonly uint level_require;
			public readonly uint price_type;
			public readonly uint price_now;
			public readonly uint price_before;
			public readonly uint limit_type;
			public readonly uint server_limit;
			public readonly uint personal_limit;
			public readonly uint perPurchase_limit_count;
			public readonly List<List<uint>> Activity_Type;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Item_Number = ReadHelper.ReadUInt(binaryReader);
				Activity_Id = ReadHelper.ReadUInt(binaryReader);
				item_id = ReadHelper.ReadUInt(binaryReader);
				produce_id = ReadHelper.ReadUInt(binaryReader);
				need_func_id = ReadHelper.ReadUInt(binaryReader);
				level_require = ReadHelper.ReadUInt(binaryReader);
				price_type = ReadHelper.ReadUInt(binaryReader);
				price_now = ReadHelper.ReadUInt(binaryReader);
				price_before = ReadHelper.ReadUInt(binaryReader);
				limit_type = ReadHelper.ReadUInt(binaryReader);
				server_limit = ReadHelper.ReadUInt(binaryReader);
				personal_limit = ReadHelper.ReadUInt(binaryReader);
				perPurchase_limit_count = ReadHelper.ReadUInt(binaryReader);
				Activity_Type = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVActivityShopGoods.bytes";
		}

		private static CSVActivityShopGoods instance = null;			
		public static CSVActivityShopGoods Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVActivityShopGoods 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVActivityShopGoods forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVActivityShopGoods();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVActivityShopGoods");

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

    sealed public partial class CSVActivityShopGoods : FCSVActivityShopGoods
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVActivityShopGoods.bytes";
		}

		private static CSVActivityShopGoods instance = null;			
		public static CSVActivityShopGoods Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVActivityShopGoods 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVActivityShopGoods forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVActivityShopGoods();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVActivityShopGoods");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}