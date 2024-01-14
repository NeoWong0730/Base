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

	sealed public partial class CSVTrialLevelGrade : Framework.Table.TableBase<CSVTrialLevelGrade.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint levelGrade;
			public readonly uint team_id;
			public readonly uint bossNPC_id;
			public readonly List<int> model_location;
			public readonly uint model_size;
			public readonly List<uint> action;
			public readonly uint dailyActivites;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				levelGrade = ReadHelper.ReadUInt(binaryReader);
				team_id = ReadHelper.ReadUInt(binaryReader);
				bossNPC_id = ReadHelper.ReadUInt(binaryReader);
				model_location = shareData.GetShareData<List<int>>(binaryReader, 0);
				model_size = ReadHelper.ReadUInt(binaryReader);
				action = shareData.GetShareData<List<uint>>(binaryReader, 1);
				dailyActivites = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTrialLevelGrade.bytes";
		}

		private static CSVTrialLevelGrade instance = null;			
		public static CSVTrialLevelGrade Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTrialLevelGrade 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTrialLevelGrade forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTrialLevelGrade();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTrialLevelGrade");

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
			shareData.ReadArrays<int>(binaryReader, 0, ReadHelper.ReadArray_ReadInt);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVTrialLevelGrade : FCSVTrialLevelGrade
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTrialLevelGrade.bytes";
		}

		private static CSVTrialLevelGrade instance = null;			
		public static CSVTrialLevelGrade Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTrialLevelGrade 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTrialLevelGrade forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTrialLevelGrade();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTrialLevelGrade");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}