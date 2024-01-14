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

	sealed public partial class CSVSkillAction : Framework.Table.TableBase<CSVSkillAction.Data>
	{
	    sealed public partial class Data
	    {
			public readonly uint id; // 动作id
			public readonly uint action_id; // 动作id
			public readonly uint weapon_type; // 武器类型
			public readonly uint weapon_action_id; // 武器动作id
			public readonly string path; // 文件目录
			public readonly string skill001; // 技能1
			public readonly string skill002; // 技能2
			public readonly string skill003; // 技能3
			public readonly string skill004; // 技能4
			public readonly string skill005; // 技能5
			public readonly string skill006; // 技能6
			public readonly string skill007; // 技能7
			public readonly string skill008; // 技能8
			public readonly string skill009; // 技能9
			public readonly string skill010; // 技能10


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				action_id = ReadHelper.ReadUInt(binaryReader);
				weapon_type = ReadHelper.ReadUInt(binaryReader);
				weapon_action_id = ReadHelper.ReadUInt(binaryReader);
				path = shareData.GetShareData<string>(binaryReader, 0);
				skill001 = shareData.GetShareData<string>(binaryReader, 0);
				skill002 = shareData.GetShareData<string>(binaryReader, 0);
				skill003 = shareData.GetShareData<string>(binaryReader, 0);
				skill004 = shareData.GetShareData<string>(binaryReader, 0);
				skill005 = shareData.GetShareData<string>(binaryReader, 0);
				skill006 = shareData.GetShareData<string>(binaryReader, 0);
				skill007 = shareData.GetShareData<string>(binaryReader, 0);
				skill008 = shareData.GetShareData<string>(binaryReader, 0);
				skill009 = shareData.GetShareData<string>(binaryReader, 0);
				skill010 = shareData.GetShareData<string>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVSkillAction.bytes";
		}

		private static CSVSkillAction instance = null;			
		public static CSVSkillAction Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSkillAction 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSkillAction forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSkillAction();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSkillAction");

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
			TableShareData shareData = new TableShareData(1);
			shareData.ReadStrings(binaryReader, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVSkillAction : FCSVSkillAction
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVSkillAction.bytes";
		}

		private static CSVSkillAction instance = null;			
		public static CSVSkillAction Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSkillAction 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSkillAction forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSkillAction();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSkillAction");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}