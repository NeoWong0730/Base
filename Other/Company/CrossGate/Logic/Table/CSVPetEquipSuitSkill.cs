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

	sealed public partial class CSVPetEquipSuitSkill : Framework.Table.TableBase<CSVPetEquipSuitSkill.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint group_id;
			public readonly uint base_skill;
			public readonly uint upgrade_skill;
			public readonly uint weight;
			public readonly uint score;
			public readonly uint appearance_id;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				group_id = ReadHelper.ReadUInt(binaryReader);
				base_skill = ReadHelper.ReadUInt(binaryReader);
				upgrade_skill = ReadHelper.ReadUInt(binaryReader);
				weight = ReadHelper.ReadUInt(binaryReader);
				score = ReadHelper.ReadUInt(binaryReader);
				appearance_id = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVPetEquipSuitSkill.bytes";
		}

		private static CSVPetEquipSuitSkill instance = null;			
		public static CSVPetEquipSuitSkill Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetEquipSuitSkill 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetEquipSuitSkill forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetEquipSuitSkill();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetEquipSuitSkill");

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
			TableShareData shareData = null;

			return shareData;
		}
	}

#else

    sealed public partial class CSVPetEquipSuitSkill : FCSVPetEquipSuitSkill
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVPetEquipSuitSkill.bytes";
		}

		private static CSVPetEquipSuitSkill instance = null;			
		public static CSVPetEquipSuitSkill Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetEquipSuitSkill 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetEquipSuitSkill forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetEquipSuitSkill();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetEquipSuitSkill");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}