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

	sealed public partial class CSVItemSource : Framework.Table.TableBase<CSVItemSource.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Icon_id;
			public readonly uint Name_id;
			public readonly uint Function_id;
			public readonly List<uint> Level;
			public readonly uint Type;
			public readonly uint UI_id;
			public readonly uint Activity_id;
			public readonly uint NPC_id;
			public readonly List<uint> Parameter;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Icon_id = ReadHelper.ReadUInt(binaryReader);
				Name_id = ReadHelper.ReadUInt(binaryReader);
				Function_id = ReadHelper.ReadUInt(binaryReader);
				Level = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Type = ReadHelper.ReadUInt(binaryReader);
				UI_id = ReadHelper.ReadUInt(binaryReader);
				Activity_id = ReadHelper.ReadUInt(binaryReader);
				NPC_id = ReadHelper.ReadUInt(binaryReader);
				Parameter = shareData.GetShareData<List<uint>>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVItemSource.bytes";
		}

		private static CSVItemSource instance = null;			
		public static CSVItemSource Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVItemSource 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVItemSource forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVItemSource();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVItemSource");

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

    sealed public partial class CSVItemSource : FCSVItemSource
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVItemSource.bytes";
		}

		private static CSVItemSource instance = null;			
		public static CSVItemSource Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVItemSource 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVItemSource forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVItemSource();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVItemSource");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}