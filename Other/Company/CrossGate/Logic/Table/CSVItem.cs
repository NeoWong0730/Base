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

	sealed public partial class CSVItem : Framework.Table.TableBase<CSVItem.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint name_id;
			public readonly uint icon_id;
			public readonly uint small_icon_id;
			public readonly uint lv;
			public readonly uint quality;
			public readonly uint stack_num;
			public readonly uint sell_price;
			public readonly uint Recommend_Sale;
			public readonly List<uint> undo_item;
			public readonly List<uint> undo_num;
			public readonly uint Recommend_Undo;
			public readonly uint del_item;
			public readonly uint type_id;
			public readonly uint type_name;
			public readonly uint box_id;
			public readonly uint battle_use;
			public readonly uint battle_target;
			public readonly uint scene_use;
			public readonly uint die_target;
			public readonly uint bank_use;
			public readonly uint batch_use;
			public readonly uint describe_id;
			public readonly uint world_view;
			public readonly uint show_above;
			public readonly uint use_lv;
			public readonly uint FunctionOpenId;
			public readonly List<uint> banMap;
			public readonly uint useMap;
			public readonly uint active_skillid;
			public readonly string fun_parameter;
			public readonly List<uint> fun_value;
			public readonly List<List<uint>> Source;
			public readonly List<uint> Source_ui;
			public bool on_sale { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly uint titleGet;
			public readonly uint quick_use;
			public readonly uint Daily_limit;
			public bool isSubmit { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
			public readonly List<List<uint>> rewardPaid;
			public readonly uint receptionValue;
			public readonly uint PresentType;
			public readonly uint PresentIntimacy;
			public readonly uint DoubleCheck;
			public readonly uint DoubleCheckID;
			public readonly uint ItemSource;
			public readonly uint compose;
			public readonly uint composed;
			public readonly uint OverDue;
			public readonly uint ExpirationTime;
			public readonly List<uint> appraisal;
			public readonly List<List<uint>> appraisal_drop;
			public readonly uint battle_show_type;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				name_id = ReadHelper.ReadUInt(binaryReader);
				icon_id = ReadHelper.ReadUInt(binaryReader);
				small_icon_id = ReadHelper.ReadUInt(binaryReader);
				lv = ReadHelper.ReadUInt(binaryReader);
				quality = ReadHelper.ReadUInt(binaryReader);
				stack_num = ReadHelper.ReadUInt(binaryReader);
				sell_price = ReadHelper.ReadUInt(binaryReader);
				Recommend_Sale = ReadHelper.ReadUInt(binaryReader);
				undo_item = shareData.GetShareData<List<uint>>(binaryReader, 1);
				undo_num = shareData.GetShareData<List<uint>>(binaryReader, 1);
				Recommend_Undo = ReadHelper.ReadUInt(binaryReader);
				del_item = ReadHelper.ReadUInt(binaryReader);
				type_id = ReadHelper.ReadUInt(binaryReader);
				type_name = ReadHelper.ReadUInt(binaryReader);
				box_id = ReadHelper.ReadUInt(binaryReader);
				battle_use = ReadHelper.ReadUInt(binaryReader);
				battle_target = ReadHelper.ReadUInt(binaryReader);
				scene_use = ReadHelper.ReadUInt(binaryReader);
				die_target = ReadHelper.ReadUInt(binaryReader);
				bank_use = ReadHelper.ReadUInt(binaryReader);
				batch_use = ReadHelper.ReadUInt(binaryReader);
				describe_id = ReadHelper.ReadUInt(binaryReader);
				world_view = ReadHelper.ReadUInt(binaryReader);
				show_above = ReadHelper.ReadUInt(binaryReader);
				use_lv = ReadHelper.ReadUInt(binaryReader);
				FunctionOpenId = ReadHelper.ReadUInt(binaryReader);
				banMap = shareData.GetShareData<List<uint>>(binaryReader, 1);
				useMap = ReadHelper.ReadUInt(binaryReader);
				active_skillid = ReadHelper.ReadUInt(binaryReader);
				fun_parameter = shareData.GetShareData<string>(binaryReader, 0);
				fun_value = shareData.GetShareData<List<uint>>(binaryReader, 1);
				Source = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				Source_ui = shareData.GetShareData<List<uint>>(binaryReader, 1);
				titleGet = ReadHelper.ReadUInt(binaryReader);
				quick_use = ReadHelper.ReadUInt(binaryReader);
				Daily_limit = ReadHelper.ReadUInt(binaryReader);
				rewardPaid = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				receptionValue = ReadHelper.ReadUInt(binaryReader);
				PresentType = ReadHelper.ReadUInt(binaryReader);
				PresentIntimacy = ReadHelper.ReadUInt(binaryReader);
				DoubleCheck = ReadHelper.ReadUInt(binaryReader);
				DoubleCheckID = ReadHelper.ReadUInt(binaryReader);
				ItemSource = ReadHelper.ReadUInt(binaryReader);
				compose = ReadHelper.ReadUInt(binaryReader);
				composed = ReadHelper.ReadUInt(binaryReader);
				OverDue = ReadHelper.ReadUInt(binaryReader);
				ExpirationTime = ReadHelper.ReadUInt(binaryReader);
				appraisal = shareData.GetShareData<List<uint>>(binaryReader, 1);
				appraisal_drop = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				battle_show_type = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVItem.bytes";
		}

		private static CSVItem instance = null;			
		public static CSVItem Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVItem 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVItem forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVItem();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVItem");

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
			TableShareData shareData = new TableShareData(3);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<uint>(binaryReader, 2, 1);

			return shareData;
		}
	}

#else

    sealed public partial class CSVItem : FCSVItem
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVItem.bytes";
		}

		private static CSVItem instance = null;			
		public static CSVItem Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVItem 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVItem forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVItem();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVItem");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}