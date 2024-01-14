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

	sealed public partial class CSVOrnamentsAttributes : Framework.Table.TableBase<CSVOrnamentsAttributes.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint group_id;
			public readonly uint weight;
			public readonly uint attr_id;
			public readonly uint min_attr;
			public readonly uint max_attr;
			public readonly uint score;
			public readonly uint group_type;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				group_id = ReadHelper.ReadUInt(binaryReader);
				weight = ReadHelper.ReadUInt(binaryReader);
				attr_id = ReadHelper.ReadUInt(binaryReader);
				min_attr = ReadHelper.ReadUInt(binaryReader);
				max_attr = ReadHelper.ReadUInt(binaryReader);
				score = ReadHelper.ReadUInt(binaryReader);
				group_type = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVOrnamentsAttributes.bytes";
		}

		private static CSVOrnamentsAttributes instance = null;			
		public static CSVOrnamentsAttributes Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVOrnamentsAttributes 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVOrnamentsAttributes forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVOrnamentsAttributes();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVOrnamentsAttributes");

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

    sealed public partial class CSVOrnamentsAttributes : FCSVOrnamentsAttributes
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVOrnamentsAttributes.bytes";
		}

		private static CSVOrnamentsAttributes instance = null;			
		public static CSVOrnamentsAttributes Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVOrnamentsAttributes 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVOrnamentsAttributes forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVOrnamentsAttributes();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVOrnamentsAttributes");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}