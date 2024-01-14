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

	sealed public partial class CSVIndustryTask : Framework.Table.TableBase<CSVIndustryTask.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint industryType;
			public readonly List<uint> task_array;
			public readonly List<uint> taskConsumeNum_array;
			public readonly List<uint> taskDrop_array;
			public readonly uint minConsumeNum;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				industryType = ReadHelper.ReadUInt(binaryReader);
				task_array = shareData.GetShareData<List<uint>>(binaryReader, 0);
				taskConsumeNum_array = shareData.GetShareData<List<uint>>(binaryReader, 0);
				taskDrop_array = shareData.GetShareData<List<uint>>(binaryReader, 0);
				minConsumeNum = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVIndustryTask.bytes";
		}

		private static CSVIndustryTask instance = null;			
		public static CSVIndustryTask Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVIndustryTask 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVIndustryTask forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVIndustryTask();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVIndustryTask");

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

    sealed public partial class CSVIndustryTask : FCSVIndustryTask
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVIndustryTask.bytes";
		}

		private static CSVIndustryTask instance = null;			
		public static CSVIndustryTask Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVIndustryTask 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVIndustryTask forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVIndustryTask();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVIndustryTask");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}