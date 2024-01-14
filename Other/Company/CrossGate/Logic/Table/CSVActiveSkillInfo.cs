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

	sealed public partial class CSVActiveSkillInfo : Framework.Table.TableBase<CSVActiveSkillInfo.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint skill_id;
			public readonly uint level;
			public readonly uint rank;
			public readonly uint icon;
			public readonly uint name;
			public readonly uint desc;
			public readonly uint effect_id;
			public readonly uint typeicon;
			public readonly uint need_lv;
			public readonly List<List<uint>> need_career_lv;
			public readonly uint pre_task;
			public readonly uint score;
			public readonly uint learn_npc;
			public readonly List<List<uint>> upgrade_cost;
			public readonly uint need_proficiency;
			public readonly uint add_proficiency;
			public readonly uint target_select;
			public readonly uint anti_weapon_type;
			public readonly List<uint> require_weapon_type;
			public readonly uint career_condition;
			public readonly uint active_skillid;
			public readonly uint skill_show_id;
			public readonly uint seqencing;
			public readonly uint quality;
			public readonly uint can_lock;
			public readonly List<uint> lock_consumption;
			public bool quick_show { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly int skill_category;
			public readonly List<List<uint>> restitution;
			public readonly int skill_type;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				skill_id = ReadHelper.ReadUInt(binaryReader);
				level = ReadHelper.ReadUInt(binaryReader);
				rank = ReadHelper.ReadUInt(binaryReader);
				icon = ReadHelper.ReadUInt(binaryReader);
				name = ReadHelper.ReadUInt(binaryReader);
				desc = ReadHelper.ReadUInt(binaryReader);
				effect_id = ReadHelper.ReadUInt(binaryReader);
				typeicon = ReadHelper.ReadUInt(binaryReader);
				need_lv = ReadHelper.ReadUInt(binaryReader);
				need_career_lv = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				pre_task = ReadHelper.ReadUInt(binaryReader);
				score = ReadHelper.ReadUInt(binaryReader);
				learn_npc = ReadHelper.ReadUInt(binaryReader);
				upgrade_cost = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				need_proficiency = ReadHelper.ReadUInt(binaryReader);
				add_proficiency = ReadHelper.ReadUInt(binaryReader);
				target_select = ReadHelper.ReadUInt(binaryReader);
				anti_weapon_type = ReadHelper.ReadUInt(binaryReader);
				require_weapon_type = shareData.GetShareData<List<uint>>(binaryReader, 0);
				career_condition = ReadHelper.ReadUInt(binaryReader);
				active_skillid = ReadHelper.ReadUInt(binaryReader);
				skill_show_id = ReadHelper.ReadUInt(binaryReader);
				seqencing = ReadHelper.ReadUInt(binaryReader);
				quality = ReadHelper.ReadUInt(binaryReader);
				can_lock = ReadHelper.ReadUInt(binaryReader);
				lock_consumption = shareData.GetShareData<List<uint>>(binaryReader, 0);
				skill_category = ReadHelper.ReadInt(binaryReader);
				restitution = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				skill_type = ReadHelper.ReadInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVActiveSkillInfo.bytes";
		}

		private static CSVActiveSkillInfo instance = null;			
		public static CSVActiveSkillInfo Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVActiveSkillInfo 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVActiveSkillInfo forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVActiveSkillInfo();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVActiveSkillInfo");

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

    sealed public partial class CSVActiveSkillInfo : FCSVActiveSkillInfo
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVActiveSkillInfo.bytes";
		}

		private static CSVActiveSkillInfo instance = null;			
		public static CSVActiveSkillInfo Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVActiveSkillInfo 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVActiveSkillInfo forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVActiveSkillInfo();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVActiveSkillInfo");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}