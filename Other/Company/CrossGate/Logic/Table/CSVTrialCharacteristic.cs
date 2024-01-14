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

	sealed public partial class CSVTrialCharacteristic : Framework.Table.TableBase<CSVTrialCharacteristic.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint levelGrade_id;
			public readonly uint characteristic1_name;
			public readonly uint characteristic1_description;
			public readonly uint characteristic2_name;
			public readonly uint characteristic2_description;
			public readonly uint rank_sub;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				levelGrade_id = ReadHelper.ReadUInt(binaryReader);
				characteristic1_name = ReadHelper.ReadUInt(binaryReader);
				characteristic1_description = ReadHelper.ReadUInt(binaryReader);
				characteristic2_name = ReadHelper.ReadUInt(binaryReader);
				characteristic2_description = ReadHelper.ReadUInt(binaryReader);
				rank_sub = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTrialCharacteristic.bytes";
		}

		private static CSVTrialCharacteristic instance = null;			
		public static CSVTrialCharacteristic Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTrialCharacteristic 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTrialCharacteristic forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTrialCharacteristic();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTrialCharacteristic");

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

    sealed public partial class CSVTrialCharacteristic : FCSVTrialCharacteristic
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTrialCharacteristic.bytes";
		}

		private static CSVTrialCharacteristic instance = null;			
		public static CSVTrialCharacteristic Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTrialCharacteristic 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTrialCharacteristic forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTrialCharacteristic();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTrialCharacteristic");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}