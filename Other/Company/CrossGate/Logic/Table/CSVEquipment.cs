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

	sealed public partial class CSVEquipment : Framework.Table.TableBase<CSVEquipment.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint equipment_category;
			public readonly uint equipment_type;
			public readonly uint special_action;
			public readonly List<uint> career_condition;
			public readonly uint equipment_level;
			public readonly uint attr_id;
			public readonly uint forge_id;
			public readonly uint active_skillid;
			public readonly uint icon;
			public readonly string model;
			public readonly List<string> additional_model;
			public readonly string show_model;
			public readonly List<string> additional_show_model;
			public readonly string equip_pos;
			public readonly List<string> additional_equip_pos;
			public readonly List<uint> slot_id;
			public readonly List<uint> jewel_type;
			public readonly uint jewel_number;
			public readonly uint jewel_level;
			public readonly List<List<uint>> attr;
			public readonly uint green_id;
			public readonly uint special_id;
			public readonly List<uint> special_range;
			public readonly List<List<uint>> common_forge;
			public readonly List<List<uint>> intensify_forge;
			public readonly List<List<uint>> smelt;
			public readonly List<List<uint>> quenching;
			public readonly uint dur;
			public readonly uint distance;
			public readonly List<uint> suit_id;
			public readonly List<List<uint>> common_repair;
			public readonly List<List<uint>> intensify_repair;
			public readonly uint score_coe;
			public readonly List<uint> score_sec;
			public readonly List<List<uint>> re_smelt;
			public readonly uint quen_item;
			public bool doublehand { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly List<uint> decompose;
			public readonly uint sale_least;
			public readonly List<List<uint>> suit_item_base;
			public readonly List<List<uint>> suit_pro_base;
			public readonly List<List<uint>> suit_item_special;
			public readonly List<List<uint>> jew_lev_score;
			public readonly List<List<uint>> jew_lev_attr;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				equipment_category = ReadHelper.ReadUInt(binaryReader);
				equipment_type = ReadHelper.ReadUInt(binaryReader);
				special_action = ReadHelper.ReadUInt(binaryReader);
				career_condition = shareData.GetShareData<List<uint>>(binaryReader, 1);
				equipment_level = ReadHelper.ReadUInt(binaryReader);
				attr_id = ReadHelper.ReadUInt(binaryReader);
				forge_id = ReadHelper.ReadUInt(binaryReader);
				active_skillid = ReadHelper.ReadUInt(binaryReader);
				icon = ReadHelper.ReadUInt(binaryReader);
				model = shareData.GetShareData<string>(binaryReader, 0);
				additional_model = shareData.GetShareData<List<string>>(binaryReader, 2);
				show_model = shareData.GetShareData<string>(binaryReader, 0);
				additional_show_model = shareData.GetShareData<List<string>>(binaryReader, 2);
				equip_pos = shareData.GetShareData<string>(binaryReader, 0);
				additional_equip_pos = shareData.GetShareData<List<string>>(binaryReader, 2);
				slot_id = shareData.GetShareData<List<uint>>(binaryReader, 1);
				jewel_type = shareData.GetShareData<List<uint>>(binaryReader, 1);
				jewel_number = ReadHelper.ReadUInt(binaryReader);
				jewel_level = ReadHelper.ReadUInt(binaryReader);
				attr = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				green_id = ReadHelper.ReadUInt(binaryReader);
				special_id = ReadHelper.ReadUInt(binaryReader);
				special_range = shareData.GetShareData<List<uint>>(binaryReader, 1);
				common_forge = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				intensify_forge = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				smelt = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				quenching = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				dur = ReadHelper.ReadUInt(binaryReader);
				distance = ReadHelper.ReadUInt(binaryReader);
				suit_id = shareData.GetShareData<List<uint>>(binaryReader, 1);
				common_repair = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				intensify_repair = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				score_coe = ReadHelper.ReadUInt(binaryReader);
				score_sec = shareData.GetShareData<List<uint>>(binaryReader, 1);
				re_smelt = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				quen_item = ReadHelper.ReadUInt(binaryReader);
				decompose = shareData.GetShareData<List<uint>>(binaryReader, 1);
				sale_least = ReadHelper.ReadUInt(binaryReader);
				suit_item_base = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				suit_pro_base = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				suit_item_special = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				jew_lev_score = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				jew_lev_attr = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVEquipment.bytes";
		}

		private static CSVEquipment instance = null;			
		public static CSVEquipment Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVEquipment 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVEquipment forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVEquipment();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVEquipment");

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
			TableShareData shareData = new TableShareData(4);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadStringArrays(binaryReader, 2, 0);
			shareData.ReadArray2s<uint>(binaryReader, 3, 1);

			return shareData;
		}
	}

#else

    sealed public partial class CSVEquipment : FCSVEquipment
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVEquipment.bytes";
		}

		private static CSVEquipment instance = null;			
		public static CSVEquipment Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVEquipment 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVEquipment forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVEquipment();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVEquipment");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}