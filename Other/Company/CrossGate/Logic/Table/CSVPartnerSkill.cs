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

	sealed public partial class CSVPartnerSkill : Framework.Table.TableBase<CSVPartnerSkill.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly List<List<uint>> Battle_PassiveSkill;
			public readonly List<List<uint>> Overall_PassiveSkill;
			public readonly List<uint> Active_Skill;
			public readonly List<uint> extra_skill_unlock;
			public readonly uint extra_skill_library;
			public readonly List<uint> extra_skill_random_cost;
			public readonly List<uint> BattleTree;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Battle_PassiveSkill = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				Overall_PassiveSkill = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				Active_Skill = shareData.GetShareData<List<uint>>(binaryReader, 0);
				extra_skill_unlock = shareData.GetShareData<List<uint>>(binaryReader, 0);
				extra_skill_library = ReadHelper.ReadUInt(binaryReader);
				extra_skill_random_cost = shareData.GetShareData<List<uint>>(binaryReader, 0);
				BattleTree = shareData.GetShareData<List<uint>>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVPartnerSkill.bytes";
		}

		private static CSVPartnerSkill instance = null;			
		public static CSVPartnerSkill Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPartnerSkill 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPartnerSkill forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPartnerSkill();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPartnerSkill");

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

    sealed public partial class CSVPartnerSkill : FCSVPartnerSkill
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVPartnerSkill.bytes";
		}

		private static CSVPartnerSkill instance = null;			
		public static CSVPartnerSkill Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPartnerSkill 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPartnerSkill forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPartnerSkill();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPartnerSkill");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}