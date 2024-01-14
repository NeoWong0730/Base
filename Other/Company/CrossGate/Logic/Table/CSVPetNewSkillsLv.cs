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

	sealed public partial class CSVPetNewSkillsLv : Framework.Table.TableBase<CSVPetNewSkillsLv.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint exp;
			public readonly uint total_exp;
			public readonly uint pet_level;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				exp = ReadHelper.ReadUInt(binaryReader);
				total_exp = ReadHelper.ReadUInt(binaryReader);
				pet_level = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVPetNewSkillsLv.bytes";
		}

		private static CSVPetNewSkillsLv instance = null;			
		public static CSVPetNewSkillsLv Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetNewSkillsLv 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetNewSkillsLv forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetNewSkillsLv();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetNewSkillsLv");

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

    sealed public partial class CSVPetNewSkillsLv : FCSVPetNewSkillsLv
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVPetNewSkillsLv.bytes";
		}

		private static CSVPetNewSkillsLv instance = null;			
		public static CSVPetNewSkillsLv Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetNewSkillsLv 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetNewSkillsLv forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetNewSkillsLv();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetNewSkillsLv");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}