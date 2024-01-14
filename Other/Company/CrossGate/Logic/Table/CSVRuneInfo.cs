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

	sealed public partial class CSVRuneInfo : Framework.Table.TableBase<CSVRuneInfo.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint rune_type;
			public readonly uint rune_lvl;
			public readonly uint is_exclusive;
			public readonly List<uint> rune_attribute;
			public readonly uint rune_passiveskillID;
			public readonly uint disintegrate;
			public readonly uint stack_max;
			public readonly uint icon;
			public readonly uint rune_name;
			public readonly uint typeID;
			public readonly uint lvlID;
			public readonly byte profession_101;
			public readonly byte profession_201;
			public readonly byte profession_301;
			public readonly byte profession_401;
			public readonly byte profession_501;
			public readonly byte profession_601;
			public readonly byte profession_701;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				rune_type = ReadHelper.ReadUInt(binaryReader);
				rune_lvl = ReadHelper.ReadUInt(binaryReader);
				is_exclusive = ReadHelper.ReadUInt(binaryReader);
				rune_attribute = shareData.GetShareData<List<uint>>(binaryReader, 0);
				rune_passiveskillID = ReadHelper.ReadUInt(binaryReader);
				disintegrate = ReadHelper.ReadUInt(binaryReader);
				stack_max = ReadHelper.ReadUInt(binaryReader);
				icon = ReadHelper.ReadUInt(binaryReader);
				rune_name = ReadHelper.ReadUInt(binaryReader);
				typeID = ReadHelper.ReadUInt(binaryReader);
				lvlID = ReadHelper.ReadUInt(binaryReader);
				profession_101 = ReadHelper.ReadByte(binaryReader);
				profession_201 = ReadHelper.ReadByte(binaryReader);
				profession_301 = ReadHelper.ReadByte(binaryReader);
				profession_401 = ReadHelper.ReadByte(binaryReader);
				profession_501 = ReadHelper.ReadByte(binaryReader);
				profession_601 = ReadHelper.ReadByte(binaryReader);
				profession_701 = ReadHelper.ReadByte(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVRuneInfo.bytes";
		}

		private static CSVRuneInfo instance = null;			
		public static CSVRuneInfo Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVRuneInfo 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVRuneInfo forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVRuneInfo();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVRuneInfo");

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

    sealed public partial class CSVRuneInfo : FCSVRuneInfo
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVRuneInfo.bytes";
		}

		private static CSVRuneInfo instance = null;			
		public static CSVRuneInfo Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVRuneInfo 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVRuneInfo forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVRuneInfo();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVRuneInfo");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}