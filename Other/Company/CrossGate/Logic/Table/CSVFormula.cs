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

	sealed public partial class CSVFormula : Framework.Table.TableBase<CSVFormula.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint view_item;
			public readonly uint extra_desc;
			public readonly List<uint> formula_animation;
			public readonly uint animation_name;
			public readonly uint type;
			public readonly uint level_formula;
			public readonly uint level_skill;
			public bool unlock { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly uint cost;
			public readonly uint proficiency;
			public readonly uint forge_type;
			public readonly List<uint> item_type_id;
			public readonly uint item_lv_min;
			public readonly uint forge_num_min;
			public readonly uint forge_num;
			public readonly List<List<uint>> normal_forge;
			public bool can_intensify { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
			public readonly List<List<uint>> intensify_forge;
			public bool can_harden { get { return ReadHelper.GetBoolByIndex(boolArray0, 2); } }
			public readonly uint harden_item_id;
			public readonly uint harden_item_source;
			public bool isequipment { get { return ReadHelper.GetBoolByIndex(boolArray0, 3); } }
			public readonly List<uint> career_condition;
			public readonly List<uint> forge_success_equip;
			public readonly List<uint> intensify_forge_equip;
			public readonly uint lucky_value;
			public readonly List<uint> lucky_forge_equip;
			public readonly List<uint> orange_forge_equip;
			public readonly uint forge_success_item;
			public readonly uint extra_item;
			public readonly uint forge_fail_item;
			public readonly uint entrust_currency;
			public readonly uint entrust_pay;
			public readonly uint assist_currency;
			public readonly uint assist_reward;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				view_item = ReadHelper.ReadUInt(binaryReader);
				extra_desc = ReadHelper.ReadUInt(binaryReader);
				formula_animation = shareData.GetShareData<List<uint>>(binaryReader, 0);
				animation_name = ReadHelper.ReadUInt(binaryReader);
				type = ReadHelper.ReadUInt(binaryReader);
				level_formula = ReadHelper.ReadUInt(binaryReader);
				level_skill = ReadHelper.ReadUInt(binaryReader);
				cost = ReadHelper.ReadUInt(binaryReader);
				proficiency = ReadHelper.ReadUInt(binaryReader);
				forge_type = ReadHelper.ReadUInt(binaryReader);
				item_type_id = shareData.GetShareData<List<uint>>(binaryReader, 0);
				item_lv_min = ReadHelper.ReadUInt(binaryReader);
				forge_num_min = ReadHelper.ReadUInt(binaryReader);
				forge_num = ReadHelper.ReadUInt(binaryReader);
				normal_forge = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				intensify_forge = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				harden_item_id = ReadHelper.ReadUInt(binaryReader);
				harden_item_source = ReadHelper.ReadUInt(binaryReader);
				career_condition = shareData.GetShareData<List<uint>>(binaryReader, 0);
				forge_success_equip = shareData.GetShareData<List<uint>>(binaryReader, 0);
				intensify_forge_equip = shareData.GetShareData<List<uint>>(binaryReader, 0);
				lucky_value = ReadHelper.ReadUInt(binaryReader);
				lucky_forge_equip = shareData.GetShareData<List<uint>>(binaryReader, 0);
				orange_forge_equip = shareData.GetShareData<List<uint>>(binaryReader, 0);
				forge_success_item = ReadHelper.ReadUInt(binaryReader);
				extra_item = ReadHelper.ReadUInt(binaryReader);
				forge_fail_item = ReadHelper.ReadUInt(binaryReader);
				entrust_currency = ReadHelper.ReadUInt(binaryReader);
				entrust_pay = ReadHelper.ReadUInt(binaryReader);
				assist_currency = ReadHelper.ReadUInt(binaryReader);
				assist_reward = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFormula.bytes";
		}

		private static CSVFormula instance = null;			
		public static CSVFormula Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFormula 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFormula forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFormula();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFormula");

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

    sealed public partial class CSVFormula : FCSVFormula
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFormula.bytes";
		}

		private static CSVFormula instance = null;			
		public static CSVFormula Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFormula 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFormula forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFormula();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFormula");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}