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

	sealed public partial class CSVTrialSkillBar : Framework.Table.TableBase<CSVTrialSkillBar.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint superSkill_id;
			public readonly uint pet_type;
			public readonly List<uint> characteristic_id;
			public readonly uint sort_id;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				superSkill_id = ReadHelper.ReadUInt(binaryReader);
				pet_type = ReadHelper.ReadUInt(binaryReader);
				characteristic_id = shareData.GetShareData<List<uint>>(binaryReader, 0);
				sort_id = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTrialSkillBar.bytes";
		}

		private static CSVTrialSkillBar instance = null;			
		public static CSVTrialSkillBar Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTrialSkillBar 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTrialSkillBar forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTrialSkillBar();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTrialSkillBar");

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
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVTrialSkillBar : FCSVTrialSkillBar
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTrialSkillBar.bytes";
		}

		private static CSVTrialSkillBar instance = null;			
		public static CSVTrialSkillBar Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTrialSkillBar 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTrialSkillBar forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTrialSkillBar();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTrialSkillBar");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}