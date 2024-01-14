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

	sealed public partial class CSVPassiveSkillInfo : Framework.Table.TableBase<CSVPassiveSkillInfo.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint name;
			public readonly uint desc;
			public readonly uint skill_id;
			public readonly uint level;
			public readonly uint order;
			public readonly uint need_lv;
			public readonly List<List<uint>> need_career_lv;
			public readonly uint pre_task;
			public readonly uint score;
			public readonly uint learn_npc;
			public readonly List<List<uint>> upgrade_cost;
			public readonly uint max_adept;
			public readonly uint icon;
			public readonly List<List<int>> attr;
			public readonly List<uint> passive_skill_id;
			public readonly uint quality;
			public readonly uint typeicon;
			public readonly List<List<uint>> restitution;
			public readonly uint cost_energy;
			public readonly uint cost_energy_limit;
			public readonly uint mutex_id;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				name = ReadHelper.ReadUInt(binaryReader);
				desc = ReadHelper.ReadUInt(binaryReader);
				skill_id = ReadHelper.ReadUInt(binaryReader);
				level = ReadHelper.ReadUInt(binaryReader);
				order = ReadHelper.ReadUInt(binaryReader);
				need_lv = ReadHelper.ReadUInt(binaryReader);
				need_career_lv = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				pre_task = ReadHelper.ReadUInt(binaryReader);
				score = ReadHelper.ReadUInt(binaryReader);
				learn_npc = ReadHelper.ReadUInt(binaryReader);
				upgrade_cost = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				max_adept = ReadHelper.ReadUInt(binaryReader);
				icon = ReadHelper.ReadUInt(binaryReader);
				attr = shareData.GetShareData<List<List<int>>>(binaryReader, 3);
				passive_skill_id = shareData.GetShareData<List<uint>>(binaryReader, 0);
				quality = ReadHelper.ReadUInt(binaryReader);
				typeicon = ReadHelper.ReadUInt(binaryReader);
				restitution = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				cost_energy = ReadHelper.ReadUInt(binaryReader);
				cost_energy_limit = ReadHelper.ReadUInt(binaryReader);
				mutex_id = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVPassiveSkillInfo.bytes";
		}

		private static CSVPassiveSkillInfo instance = null;			
		public static CSVPassiveSkillInfo Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPassiveSkillInfo 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPassiveSkillInfo forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPassiveSkillInfo();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPassiveSkillInfo");

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
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArrays<int>(binaryReader, 1, ReadHelper.ReadArray_ReadInt);
			shareData.ReadArray2s<uint>(binaryReader, 2, 0);
			shareData.ReadArray2s<int>(binaryReader, 3, 1);

			return shareData;
		}
	}

#else

    sealed public partial class CSVPassiveSkillInfo : FCSVPassiveSkillInfo
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVPassiveSkillInfo.bytes";
		}

		private static CSVPassiveSkillInfo instance = null;			
		public static CSVPassiveSkillInfo Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPassiveSkillInfo 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPassiveSkillInfo forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPassiveSkillInfo();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPassiveSkillInfo");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}