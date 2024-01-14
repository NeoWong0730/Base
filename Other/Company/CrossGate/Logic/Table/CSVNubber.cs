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

	sealed public partial class CSVNubber : Framework.Table.TableBase<CSVNubber.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint url_resources;
			public readonly uint para_id;
			public readonly uint proportion;
			public readonly List<int> model_position;
			public readonly uint auto_use;
			public readonly uint zooming;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				url_resources = ReadHelper.ReadUInt(binaryReader);
				para_id = ReadHelper.ReadUInt(binaryReader);
				proportion = ReadHelper.ReadUInt(binaryReader);
				model_position = shareData.GetShareData<List<int>>(binaryReader, 0);
				auto_use = ReadHelper.ReadUInt(binaryReader);
				zooming = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVNubber.bytes";
		}

		private static CSVNubber instance = null;			
		public static CSVNubber Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVNubber 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVNubber forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVNubber();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVNubber");

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
			shareData.ReadArrays<int>(binaryReader, 0, ReadHelper.ReadArray_ReadInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVNubber : FCSVNubber
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVNubber.bytes";
		}

		private static CSVNubber instance = null;			
		public static CSVNubber Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVNubber 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVNubber forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVNubber();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVNubber");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}