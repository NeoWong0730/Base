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

	sealed public partial class CSVIndustryActivity : Framework.Table.TableBase<CSVIndustryActivity.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint industryIcon;
			public readonly uint findNPC;
			public readonly uint industryName;
			public readonly uint playName;
			public readonly uint playIntroduce;
			public readonly string teachingPicture;
			public readonly List<uint> industryTask_array;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				industryIcon = ReadHelper.ReadUInt(binaryReader);
				findNPC = ReadHelper.ReadUInt(binaryReader);
				industryName = ReadHelper.ReadUInt(binaryReader);
				playName = ReadHelper.ReadUInt(binaryReader);
				playIntroduce = ReadHelper.ReadUInt(binaryReader);
				teachingPicture = shareData.GetShareData<string>(binaryReader, 0);
				industryTask_array = shareData.GetShareData<List<uint>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVIndustryActivity.bytes";
		}

		private static CSVIndustryActivity instance = null;			
		public static CSVIndustryActivity Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVIndustryActivity 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVIndustryActivity forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVIndustryActivity();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVIndustryActivity");

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

    sealed public partial class CSVIndustryActivity : FCSVIndustryActivity
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVIndustryActivity.bytes";
		}

		private static CSVIndustryActivity instance = null;			
		public static CSVIndustryActivity Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVIndustryActivity 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVIndustryActivity forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVIndustryActivity();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVIndustryActivity");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}