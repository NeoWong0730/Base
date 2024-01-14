//
//#define USE_HOTFIX_LOGIC

using Lib.AssetLoader;
using Lib.Core;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Framework.Table;

namespace Table
{
#if USE_HOTFIX_LOGIC

	sealed public partial class CSVExperienceAttr : Framework.Table.TableBase<CSVExperienceAttr.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint rank;
			public readonly uint attr_id;
			public readonly uint add_value;
			public readonly uint max_level;
			public readonly uint double_cost_level;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				rank = ReadHelper.ReadUInt(binaryReader);
				attr_id = ReadHelper.ReadUInt(binaryReader);
				add_value = ReadHelper.ReadUInt(binaryReader);
				max_level = ReadHelper.ReadUInt(binaryReader);
				double_cost_level = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVExperienceAttr.bytes";
		}

		private static CSVExperienceAttr instance = null;			
		public static CSVExperienceAttr Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVExperienceAttr 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVExperienceAttr forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVExperienceAttr();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVExperienceAttr");

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

    sealed public partial class CSVExperienceAttr : FCSVExperienceAttr
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVExperienceAttr.bytes";
		}

		private static CSVExperienceAttr instance = null;			
		public static CSVExperienceAttr Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVExperienceAttr 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVExperienceAttr forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVExperienceAttr();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVExperienceAttr");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}