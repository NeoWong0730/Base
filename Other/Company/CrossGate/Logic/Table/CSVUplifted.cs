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

	sealed public partial class CSVUplifted : Framework.Table.TableBase<CSVUplifted.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Name_id;
			public readonly uint Describe_id;
			public readonly uint Icon_id;
			public readonly uint Type;
			public readonly uint Parameter;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Name_id = ReadHelper.ReadUInt(binaryReader);
				Describe_id = ReadHelper.ReadUInt(binaryReader);
				Icon_id = ReadHelper.ReadUInt(binaryReader);
				Type = ReadHelper.ReadUInt(binaryReader);
				Parameter = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVUplifted.bytes";
		}

		private static CSVUplifted instance = null;			
		public static CSVUplifted Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVUplifted 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVUplifted forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVUplifted();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVUplifted");

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

    sealed public partial class CSVUplifted : FCSVUplifted
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVUplifted.bytes";
		}

		private static CSVUplifted instance = null;			
		public static CSVUplifted Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVUplifted 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVUplifted forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVUplifted();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVUplifted");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}