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

	sealed public partial class CSVPetNew : Framework.Table.TableBase<CSVPetNew.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint name;
			public readonly uint PetBooks;
			public readonly byte card_type;
			public readonly byte card_lv;
			public readonly byte race;
			public bool reborn { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public bool mount { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
			public readonly byte mountgrade;
			public readonly List<int> mountsignposition;
			public bool show_pet { get { return ReadHelper.GetBoolByIndex(boolArray0, 2); } }
			public bool lose_loyalty { get { return ReadHelper.GetBoolByIndex(boolArray0, 3); } }
			public readonly uint icon_id;
			public readonly uint action_id;
			public readonly uint action_id_show;
			public readonly uint action_id_mount;
			public readonly string model;
			public readonly string model_show;
			public readonly uint bust;
			public readonly uint weapon;
			public readonly byte participation_lv;
			public readonly List<List<uint>> init_attr;
			public readonly uint attr_id;
			public readonly uint init_score;
			public readonly List<uint> quality_score;
			public readonly byte max_lost_gear;
			public readonly List<uint> gear_param;
			public readonly byte endurance;
			public readonly byte strength;
			public readonly byte strong;
			public readonly byte speed;
			public readonly byte magic;
			public readonly byte skill_num;
			public readonly List<List<uint>> required_skills;
			public readonly List<uint> required_skills_up;
			public readonly List<uint> required_skills_count;
			public readonly List<List<uint>> required_skills_money;
			public readonly List<List<uint>> lv_back_money;
			public readonly List<List<uint>> unique_skills;
			public readonly List<uint> remake_skills;
			public readonly byte first_remake_num;
			public readonly byte max_remake_num;
			public readonly string action_idle;
			public readonly uint shadow;
			public readonly float translation;
			public readonly float angle1;
			public readonly float angle2;
			public readonly float angle3;
			public readonly float height;
			public readonly float size;
			public readonly uint haunted_area;
			public readonly List<uint> activity;
			public readonly uint jump_item;
			public readonly List<List<uint>> add_point_num;
			public readonly uint zooming;
			public readonly uint is_follow;
			public readonly float follow_distance;
			public readonly uint map_show;
			public bool search_pet { get { return ReadHelper.GetBoolByIndex(boolArray0, 4); } }
			public readonly List<string> SubPackageShow;
			public readonly uint is_defauit;
			public readonly uint PetBooks_is_act;
			public bool show_appearance { get { return ReadHelper.GetBoolByIndex(boolArray0, 5); } }
			public readonly uint is_seal;
			public bool is_gold_charge { get { return ReadHelper.GetBoolByIndex(boolArray0, 6); } }
			public bool is_gold_adv { get { return ReadHelper.GetBoolByIndex(boolArray0, 7); } }
			public readonly uint forward_adv_num;
			public readonly uint soul_skill_id;
			public readonly List<uint> soul_activate_cost;
			public readonly List<uint> extra1_remake_cost;
			public readonly List<uint> extra2_remake_cost;
			public readonly List<uint> pet_feel_lv;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				name = ReadHelper.ReadUInt(binaryReader);
				PetBooks = ReadHelper.ReadUInt(binaryReader);
				card_type = ReadHelper.ReadByte(binaryReader);
				card_lv = ReadHelper.ReadByte(binaryReader);
				race = ReadHelper.ReadByte(binaryReader);
				mountgrade = ReadHelper.ReadByte(binaryReader);
				mountsignposition = shareData.GetShareData<List<int>>(binaryReader, 1);
				icon_id = ReadHelper.ReadUInt(binaryReader);
				action_id = ReadHelper.ReadUInt(binaryReader);
				action_id_show = ReadHelper.ReadUInt(binaryReader);
				action_id_mount = ReadHelper.ReadUInt(binaryReader);
				model = shareData.GetShareData<string>(binaryReader, 0);
				model_show = shareData.GetShareData<string>(binaryReader, 0);
				bust = ReadHelper.ReadUInt(binaryReader);
				weapon = ReadHelper.ReadUInt(binaryReader);
				participation_lv = ReadHelper.ReadByte(binaryReader);
				init_attr = shareData.GetShareData<List<List<uint>>>(binaryReader, 4);
				attr_id = ReadHelper.ReadUInt(binaryReader);
				init_score = ReadHelper.ReadUInt(binaryReader);
				quality_score = shareData.GetShareData<List<uint>>(binaryReader, 2);
				max_lost_gear = ReadHelper.ReadByte(binaryReader);
				gear_param = shareData.GetShareData<List<uint>>(binaryReader, 2);
				endurance = ReadHelper.ReadByte(binaryReader);
				strength = ReadHelper.ReadByte(binaryReader);
				strong = ReadHelper.ReadByte(binaryReader);
				speed = ReadHelper.ReadByte(binaryReader);
				magic = ReadHelper.ReadByte(binaryReader);
				skill_num = ReadHelper.ReadByte(binaryReader);
				required_skills = shareData.GetShareData<List<List<uint>>>(binaryReader, 4);
				required_skills_up = shareData.GetShareData<List<uint>>(binaryReader, 2);
				required_skills_count = shareData.GetShareData<List<uint>>(binaryReader, 2);
				required_skills_money = shareData.GetShareData<List<List<uint>>>(binaryReader, 4);
				lv_back_money = shareData.GetShareData<List<List<uint>>>(binaryReader, 4);
				unique_skills = shareData.GetShareData<List<List<uint>>>(binaryReader, 4);
				remake_skills = shareData.GetShareData<List<uint>>(binaryReader, 2);
				first_remake_num = ReadHelper.ReadByte(binaryReader);
				max_remake_num = ReadHelper.ReadByte(binaryReader);
				action_idle = shareData.GetShareData<string>(binaryReader, 0);
				shadow = ReadHelper.ReadUInt(binaryReader);
				translation = ReadHelper.ReadFloat(binaryReader);
				angle1 = ReadHelper.ReadFloat(binaryReader);
				angle2 = ReadHelper.ReadFloat(binaryReader);
				angle3 = ReadHelper.ReadFloat(binaryReader);
				height = ReadHelper.ReadFloat(binaryReader);
				size = ReadHelper.ReadFloat(binaryReader);
				haunted_area = ReadHelper.ReadUInt(binaryReader);
				activity = shareData.GetShareData<List<uint>>(binaryReader, 2);
				jump_item = ReadHelper.ReadUInt(binaryReader);
				add_point_num = shareData.GetShareData<List<List<uint>>>(binaryReader, 4);
				zooming = ReadHelper.ReadUInt(binaryReader);
				is_follow = ReadHelper.ReadUInt(binaryReader);
				follow_distance = ReadHelper.ReadFloat(binaryReader);
				map_show = ReadHelper.ReadUInt(binaryReader);
				SubPackageShow = shareData.GetShareData<List<string>>(binaryReader, 3);
				is_defauit = ReadHelper.ReadUInt(binaryReader);
				PetBooks_is_act = ReadHelper.ReadUInt(binaryReader);
				is_seal = ReadHelper.ReadUInt(binaryReader);
				forward_adv_num = ReadHelper.ReadUInt(binaryReader);
				soul_skill_id = ReadHelper.ReadUInt(binaryReader);
				soul_activate_cost = shareData.GetShareData<List<uint>>(binaryReader, 2);
				extra1_remake_cost = shareData.GetShareData<List<uint>>(binaryReader, 2);
				extra2_remake_cost = shareData.GetShareData<List<uint>>(binaryReader, 2);
				pet_feel_lv = shareData.GetShareData<List<uint>>(binaryReader, 2);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVPetNew.bytes";
		}

		private static CSVPetNew instance = null;			
		public static CSVPetNew Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetNew 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetNew forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetNew();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetNew");

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
			TableShareData shareData = new TableShareData(5);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<int>(binaryReader, 1, ReadHelper.ReadArray_ReadInt);
			shareData.ReadArrays<uint>(binaryReader, 2, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadStringArrays(binaryReader, 3, 0);
			shareData.ReadArray2s<uint>(binaryReader, 4, 2);

			return shareData;
		}
	}

#else

    sealed public partial class CSVPetNew : FCSVPetNew
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVPetNew.bytes";
		}

		private static CSVPetNew instance = null;			
		public static CSVPetNew Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetNew 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetNew forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetNew();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetNew");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}