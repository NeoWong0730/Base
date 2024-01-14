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

	sealed public partial class CSVMemoryPieces : Framework.Table.TableBase<CSVMemoryPieces.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly string icon;
			public readonly uint title_name;
			public readonly uint memory_name;
			public readonly string show_image;
			public readonly uint memory_text;
			public readonly uint Source;
			public readonly uint deblock_type;
			public readonly uint deblock_condition;
			public readonly uint currency;
			public readonly uint show_above;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				icon = shareData.GetShareData<string>(binaryReader, 0);
				title_name = ReadHelper.ReadUInt(binaryReader);
				memory_name = ReadHelper.ReadUInt(binaryReader);
				show_image = shareData.GetShareData<string>(binaryReader, 0);
				memory_text = ReadHelper.ReadUInt(binaryReader);
				Source = ReadHelper.ReadUInt(binaryReader);
				deblock_type = ReadHelper.ReadUInt(binaryReader);
				deblock_condition = ReadHelper.ReadUInt(binaryReader);
				currency = ReadHelper.ReadUInt(binaryReader);
				show_above = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVMemoryPieces.bytes";
		}

		private static CSVMemoryPieces instance = null;			
		public static CSVMemoryPieces Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVMemoryPieces 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVMemoryPieces forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVMemoryPieces();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVMemoryPieces");

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
			shareData.ReadStrings(binaryReader, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVMemoryPieces : FCSVMemoryPieces
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVMemoryPieces.bytes";
		}

		private static CSVMemoryPieces instance = null;			
		public static CSVMemoryPieces Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVMemoryPieces 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVMemoryPieces forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVMemoryPieces();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVMemoryPieces");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}