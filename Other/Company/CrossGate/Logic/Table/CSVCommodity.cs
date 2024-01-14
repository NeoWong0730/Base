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

	sealed public partial class CSVCommodity : Framework.Table.TableBase<CSVCommodity.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint type;
			public readonly List<uint> quality_range;
			public readonly uint world_level;
			public bool treasure { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly uint pricing_type;
			public readonly uint recommend_price;
			public readonly uint bulk_sale;
			public bool bargain { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
			public bool publicity { get { return ReadHelper.GetBoolByIndex(boolArray0, 2); } }
			public bool assignation { get { return ReadHelper.GetBoolByIndex(boolArray0, 3); } }
			public bool check { get { return ReadHelper.GetBoolByIndex(boolArray0, 4); } }
			public readonly uint category;
			public readonly uint subclass;
			public bool cross_server { get { return ReadHelper.GetBoolByIndex(boolArray0, 5); } }
			public readonly uint cross_category;
			public readonly uint cross_subclass;
			public bool attention { get { return ReadHelper.GetBoolByIndex(boolArray0, 6); } }
			public bool share { get { return ReadHelper.GetBoolByIndex(boolArray0, 7); } }
			public bool contact_seller { get { return ReadHelper.GetBoolByIndex(boolArray1, 0); } }
			public bool buy_cheap { get { return ReadHelper.GetBoolByIndex(boolArray1, 1); } }
			public bool transaction_record { get { return ReadHelper.GetBoolByIndex(boolArray1, 2); } }
		private readonly byte boolArray0;
		private readonly byte boolArray1;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				type = ReadHelper.ReadUInt(binaryReader);
				quality_range = shareData.GetShareData<List<uint>>(binaryReader, 0);
				world_level = ReadHelper.ReadUInt(binaryReader);
				pricing_type = ReadHelper.ReadUInt(binaryReader);
				recommend_price = ReadHelper.ReadUInt(binaryReader);
				bulk_sale = ReadHelper.ReadUInt(binaryReader);
				category = ReadHelper.ReadUInt(binaryReader);
				subclass = ReadHelper.ReadUInt(binaryReader);
				cross_category = ReadHelper.ReadUInt(binaryReader);
				cross_subclass = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
			boolArray1 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVCommodity.bytes";
		}

		private static CSVCommodity instance = null;			
		public static CSVCommodity Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCommodity 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCommodity forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCommodity();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCommodity");

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
			TableShareData shareData = new TableShareData(1);
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVCommodity : FCSVCommodity
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVCommodity.bytes";
		}

		private static CSVCommodity instance = null;			
		public static CSVCommodity Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCommodity 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCommodity forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCommodity();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCommodity");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}