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

	sealed public partial class CSVGleanings : Framework.Table.TableBase<CSVGleanings.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint name_id;
			public readonly string icon_id;
			public readonly uint type_id;
			public readonly uint type2_id;
			public readonly uint TypeName;
			public readonly uint SubTypeName;
			public readonly string show_image;
			public readonly string show_image2;
			public readonly uint describe_id;
			public readonly uint Source;
			public readonly uint deblock_type;
			public readonly uint deblock_condition;
			public readonly uint currency;
			public readonly uint show_above;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				name_id = ReadHelper.ReadUInt(binaryReader);
				icon_id = shareData.GetShareData<string>(binaryReader, 0);
				type_id = ReadHelper.ReadUInt(binaryReader);
				type2_id = ReadHelper.ReadUInt(binaryReader);
				TypeName = ReadHelper.ReadUInt(binaryReader);
				SubTypeName = ReadHelper.ReadUInt(binaryReader);
				show_image = shareData.GetShareData<string>(binaryReader, 0);
				show_image2 = shareData.GetShareData<string>(binaryReader, 0);
				describe_id = ReadHelper.ReadUInt(binaryReader);
				Source = ReadHelper.ReadUInt(binaryReader);
				deblock_type = ReadHelper.ReadUInt(binaryReader);
				deblock_condition = ReadHelper.ReadUInt(binaryReader);
				currency = ReadHelper.ReadUInt(binaryReader);
				show_above = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVGleanings.bytes";
		}

		private static CSVGleanings instance = null;			
		public static CSVGleanings Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVGleanings 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVGleanings forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVGleanings();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVGleanings");

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

    sealed public partial class CSVGleanings : FCSVGleanings
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVGleanings.bytes";
		}

		private static CSVGleanings instance = null;			
		public static CSVGleanings Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVGleanings 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVGleanings forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVGleanings();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVGleanings");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}