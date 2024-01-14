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

	sealed public partial class CSVTrialPreSkill : Framework.Table.TableBase<CSVTrialPreSkill.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint skillBar_id;
			public readonly uint badge_type;
			public readonly uint badge_number;
			public readonly uint sort_id;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				skillBar_id = ReadHelper.ReadUInt(binaryReader);
				badge_type = ReadHelper.ReadUInt(binaryReader);
				badge_number = ReadHelper.ReadUInt(binaryReader);
				sort_id = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTrialPreSkill.bytes";
		}

		private static CSVTrialPreSkill instance = null;			
		public static CSVTrialPreSkill Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTrialPreSkill 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTrialPreSkill forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTrialPreSkill();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTrialPreSkill");

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

    sealed public partial class CSVTrialPreSkill : FCSVTrialPreSkill
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTrialPreSkill.bytes";
		}

		private static CSVTrialPreSkill instance = null;			
		public static CSVTrialPreSkill Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTrialPreSkill 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTrialPreSkill forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTrialPreSkill();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTrialPreSkill");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}