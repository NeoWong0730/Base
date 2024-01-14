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

	sealed public partial class CSVTutorialFirstHeading : Framework.Table.TableBase<CSVTutorialFirstHeading.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint firstHeadingName;
			public readonly uint firstHeadingSrot;
			public readonly List<uint> secondHeading_array;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				firstHeadingName = ReadHelper.ReadUInt(binaryReader);
				firstHeadingSrot = ReadHelper.ReadUInt(binaryReader);
				secondHeading_array = shareData.GetShareData<List<uint>>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTutorialFirstHeading.bytes";
		}

		private static CSVTutorialFirstHeading instance = null;			
		public static CSVTutorialFirstHeading Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTutorialFirstHeading 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTutorialFirstHeading forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTutorialFirstHeading();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTutorialFirstHeading");

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

    sealed public partial class CSVTutorialFirstHeading : FCSVTutorialFirstHeading
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTutorialFirstHeading.bytes";
		}

		private static CSVTutorialFirstHeading instance = null;			
		public static CSVTutorialFirstHeading Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTutorialFirstHeading 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTutorialFirstHeading forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTutorialFirstHeading();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTutorialFirstHeading");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}