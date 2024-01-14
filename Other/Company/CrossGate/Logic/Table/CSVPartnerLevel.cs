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

	sealed public partial class CSVPartnerLevel : Framework.Table.TableBase<CSVPartnerLevel.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint attr_id;
			public readonly uint level;
			public readonly uint totol_exp;
			public readonly uint upgrade_exp;
			public readonly List<List<uint>> attribute;
			public readonly List<uint> Active_Skill;
			public readonly List<uint> Passive_Skill;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				attr_id = ReadHelper.ReadUInt(binaryReader);
				level = ReadHelper.ReadUInt(binaryReader);
				totol_exp = ReadHelper.ReadUInt(binaryReader);
				upgrade_exp = ReadHelper.ReadUInt(binaryReader);
				attribute = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				Active_Skill = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Passive_Skill = shareData.GetShareData<List<uint>>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVPartnerLevel.bytes";
		}

		private static CSVPartnerLevel instance = null;			
		public static CSVPartnerLevel Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPartnerLevel 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPartnerLevel forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPartnerLevel();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPartnerLevel");

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

    sealed public partial class CSVPartnerLevel : FCSVPartnerLevel
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVPartnerLevel.bytes";
		}

		private static CSVPartnerLevel instance = null;			
		public static CSVPartnerLevel Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPartnerLevel 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPartnerLevel forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPartnerLevel();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPartnerLevel");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}