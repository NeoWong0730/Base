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

	sealed public partial class CSVFamilyProsperity : Framework.Table.TableBase<CSVFamilyProsperity.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint agriculture_exp;
			public readonly uint business_exp;
			public readonly uint security_exp;
			public readonly uint religion_exp;
			public readonly uint technology_exp;
			public readonly string familyPicture;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				agriculture_exp = ReadHelper.ReadUInt(binaryReader);
				business_exp = ReadHelper.ReadUInt(binaryReader);
				security_exp = ReadHelper.ReadUInt(binaryReader);
				religion_exp = ReadHelper.ReadUInt(binaryReader);
				technology_exp = ReadHelper.ReadUInt(binaryReader);
				familyPicture = shareData.GetShareData<string>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyProsperity.bytes";
		}

		private static CSVFamilyProsperity instance = null;			
		public static CSVFamilyProsperity Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyProsperity 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyProsperity forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyProsperity();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyProsperity");

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

    sealed public partial class CSVFamilyProsperity : FCSVFamilyProsperity
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyProsperity.bytes";
		}

		private static CSVFamilyProsperity instance = null;			
		public static CSVFamilyProsperity Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyProsperity 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyProsperity forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyProsperity();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyProsperity");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}