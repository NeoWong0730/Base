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

	sealed public partial class CSVLevelName : Framework.Table.TableBase<CSVLevelName.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint level_name;
			public readonly List<uint> prompt_title;
			public readonly List<uint> prompt_info;
			public readonly List<uint> prompt_model;
			public readonly List<uint> model_zoom;
			public readonly List<float> rotation_y;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				level_name = ReadHelper.ReadUInt(binaryReader);
				prompt_title = shareData.GetShareData<List<uint>>(binaryReader, 0);
				prompt_info = shareData.GetShareData<List<uint>>(binaryReader, 0);
				prompt_model = shareData.GetShareData<List<uint>>(binaryReader, 0);
				model_zoom = shareData.GetShareData<List<uint>>(binaryReader, 0);
				rotation_y = shareData.GetShareData<List<float>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVLevelName.bytes";
		}

		private static CSVLevelName instance = null;			
		public static CSVLevelName Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVLevelName 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVLevelName forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVLevelName();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVLevelName");

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
			shareData.ReadArrays<float>(binaryReader, 1, ReadHelper.ReadArray_ReadFloat);

			return shareData;
		}
	}

#else

    sealed public partial class CSVLevelName : FCSVLevelName
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVLevelName.bytes";
		}

		private static CSVLevelName instance = null;			
		public static CSVLevelName Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVLevelName 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVLevelName forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVLevelName();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVLevelName");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}