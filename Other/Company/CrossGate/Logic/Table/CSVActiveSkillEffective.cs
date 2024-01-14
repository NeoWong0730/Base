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

	sealed public partial class CSVActiveSkillEffective : Framework.Table.TableBase<CSVActiveSkillEffective.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly List<int> weapon_type;
			public readonly List<uint> behavior_id;
			public readonly uint min_magic_attack;
			public readonly uint base_magic_attack;
			public readonly uint wind_attribute;
			public readonly uint earth_attribute;
			public readonly uint water_attribute;
			public readonly uint fire_attribute;
			public readonly uint effect_type;
			public readonly uint skill_effect_type;
			public readonly uint damage_type;
			public readonly uint effect_to_target;
			public readonly uint effect_to_target2;
			public readonly uint fixed_damage;
			public readonly List<List<int>> condition_coe;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				weapon_type = shareData.GetShareData<List<int>>(binaryReader, 0);
				behavior_id = shareData.GetShareData<List<uint>>(binaryReader, 1);
				min_magic_attack = ReadHelper.ReadUInt(binaryReader);
				base_magic_attack = ReadHelper.ReadUInt(binaryReader);
				wind_attribute = ReadHelper.ReadUInt(binaryReader);
				earth_attribute = ReadHelper.ReadUInt(binaryReader);
				water_attribute = ReadHelper.ReadUInt(binaryReader);
				fire_attribute = ReadHelper.ReadUInt(binaryReader);
				effect_type = ReadHelper.ReadUInt(binaryReader);
				skill_effect_type = ReadHelper.ReadUInt(binaryReader);
				damage_type = ReadHelper.ReadUInt(binaryReader);
				effect_to_target = ReadHelper.ReadUInt(binaryReader);
				effect_to_target2 = ReadHelper.ReadUInt(binaryReader);
				fixed_damage = ReadHelper.ReadUInt(binaryReader);
				condition_coe = shareData.GetShareData<List<List<int>>>(binaryReader, 2);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVActiveSkillEffective.bytes";
		}

		private static CSVActiveSkillEffective instance = null;			
		public static CSVActiveSkillEffective Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVActiveSkillEffective 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVActiveSkillEffective forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVActiveSkillEffective();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVActiveSkillEffective");

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
			shareData.ReadArrays<int>(binaryReader, 0, ReadHelper.ReadArray_ReadInt);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<int>(binaryReader, 2, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVActiveSkillEffective : FCSVActiveSkillEffective
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVActiveSkillEffective.bytes";
		}

		private static CSVActiveSkillEffective instance = null;			
		public static CSVActiveSkillEffective Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVActiveSkillEffective 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVActiveSkillEffective forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVActiveSkillEffective();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVActiveSkillEffective");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}