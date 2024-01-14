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

	sealed public partial class CSVRuneSynthetise : Framework.Table.TableBase<CSVRuneSynthetise.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly List<uint> synthetise_expend;
			public readonly uint synthetise_production;
			public readonly uint synthetise_maxnum;
			public readonly string bg_path;
			public readonly uint baseIcon;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				synthetise_expend = shareData.GetShareData<List<uint>>(binaryReader, 1);
				synthetise_production = ReadHelper.ReadUInt(binaryReader);
				synthetise_maxnum = ReadHelper.ReadUInt(binaryReader);
				bg_path = shareData.GetShareData<string>(binaryReader, 0);
				baseIcon = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVRuneSynthetise.bytes";
		}

		private static CSVRuneSynthetise instance = null;			
		public static CSVRuneSynthetise Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVRuneSynthetise 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVRuneSynthetise forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVRuneSynthetise();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVRuneSynthetise");

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
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVRuneSynthetise : FCSVRuneSynthetise
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVRuneSynthetise.bytes";
		}

		private static CSVRuneSynthetise instance = null;			
		public static CSVRuneSynthetise Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVRuneSynthetise 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVRuneSynthetise forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVRuneSynthetise();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVRuneSynthetise");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}