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

	sealed public partial class CSVBaseAction : Framework.Table.TableBase<CSVBaseAction.Data>
	{
	    sealed public partial class Data
	    {
			public readonly uint id; // 动作id
			public readonly uint action_id; // 动作id
			public readonly uint weapon_type; // 武器类型
			public readonly uint weapon_action_id; // 武器动作id
			public readonly string path; // 文件目录
			public readonly string idle; // 待机动作
			public readonly string walk; // 走路动作
			public readonly string sprit; // 疾跑动作
			public readonly string die; // 死亡动作


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				action_id = ReadHelper.ReadUInt(binaryReader);
				weapon_type = ReadHelper.ReadUInt(binaryReader);
				weapon_action_id = ReadHelper.ReadUInt(binaryReader);
				path = shareData.GetShareData<string>(binaryReader, 0);
				idle = shareData.GetShareData<string>(binaryReader, 0);
				walk = shareData.GetShareData<string>(binaryReader, 0);
				sprit = shareData.GetShareData<string>(binaryReader, 0);
				die = shareData.GetShareData<string>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVBaseAction.bytes";
		}

		private static CSVBaseAction instance = null;			
		public static CSVBaseAction Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBaseAction 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBaseAction forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBaseAction();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBaseAction");

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

    sealed public partial class CSVBaseAction : FCSVBaseAction
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVBaseAction.bytes";
		}

		private static CSVBaseAction instance = null;			
		public static CSVBaseAction Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBaseAction 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBaseAction forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBaseAction();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBaseAction");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}