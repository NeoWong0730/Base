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

	sealed public partial class CSVSubmit : Framework.Table.TableBase<CSVSubmit.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint itemId1;
			public readonly uint itemCount1;
			public readonly uint itemId2;
			public readonly uint itemCount2;
			public readonly uint itemId3;
			public readonly uint itemCount3;
			public readonly uint itemId4;
			public readonly uint itemCount4;
			public readonly uint itemId5;
			public readonly uint itemCount5;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				itemId1 = ReadHelper.ReadUInt(binaryReader);
				itemCount1 = ReadHelper.ReadUInt(binaryReader);
				itemId2 = ReadHelper.ReadUInt(binaryReader);
				itemCount2 = ReadHelper.ReadUInt(binaryReader);
				itemId3 = ReadHelper.ReadUInt(binaryReader);
				itemCount3 = ReadHelper.ReadUInt(binaryReader);
				itemId4 = ReadHelper.ReadUInt(binaryReader);
				itemCount4 = ReadHelper.ReadUInt(binaryReader);
				itemId5 = ReadHelper.ReadUInt(binaryReader);
				itemCount5 = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVSubmit.bytes";
		}

		private static CSVSubmit instance = null;			
		public static CSVSubmit Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSubmit 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSubmit forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSubmit();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSubmit");

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

    sealed public partial class CSVSubmit : FCSVSubmit
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVSubmit.bytes";
		}

		private static CSVSubmit instance = null;			
		public static CSVSubmit Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSubmit 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSubmit forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSubmit();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSubmit");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}