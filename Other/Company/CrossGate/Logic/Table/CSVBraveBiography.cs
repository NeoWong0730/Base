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

	sealed public partial class CSVBraveBiography : Framework.Table.TableBase<CSVBraveBiography.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint biography_name;
			public readonly uint biography_text;
			public readonly uint Source;
			public readonly uint deblock_type;
			public readonly uint deblock_condition;
			public readonly uint currency;
			public readonly uint show_above;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				biography_name = ReadHelper.ReadUInt(binaryReader);
				biography_text = ReadHelper.ReadUInt(binaryReader);
				Source = ReadHelper.ReadUInt(binaryReader);
				deblock_type = ReadHelper.ReadUInt(binaryReader);
				deblock_condition = ReadHelper.ReadUInt(binaryReader);
				currency = ReadHelper.ReadUInt(binaryReader);
				show_above = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVBraveBiography.bytes";
		}

		private static CSVBraveBiography instance = null;			
		public static CSVBraveBiography Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBraveBiography 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBraveBiography forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBraveBiography();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBraveBiography");

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

    sealed public partial class CSVBraveBiography : FCSVBraveBiography
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVBraveBiography.bytes";
		}

		private static CSVBraveBiography instance = null;			
		public static CSVBraveBiography Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBraveBiography 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBraveBiography forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBraveBiography();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBraveBiography");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}