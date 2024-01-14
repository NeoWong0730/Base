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

	sealed public partial class CSVStone : Framework.Table.TableBase<CSVStone.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint stone_name;
			public readonly uint icon;
			public readonly uint type;
			public bool exclusive { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly List<uint> career_limit;
			public readonly uint level_limit;
			public readonly uint initial_level;
			public readonly uint max_level;
			public readonly uint upgrade_type;
			public readonly List<List<uint>> cost;
			public readonly uint max_stage;
			public bool can_decompose { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				stone_name = ReadHelper.ReadUInt(binaryReader);
				icon = ReadHelper.ReadUInt(binaryReader);
				type = ReadHelper.ReadUInt(binaryReader);
				career_limit = shareData.GetShareData<List<uint>>(binaryReader, 0);
				level_limit = ReadHelper.ReadUInt(binaryReader);
				initial_level = ReadHelper.ReadUInt(binaryReader);
				max_level = ReadHelper.ReadUInt(binaryReader);
				upgrade_type = ReadHelper.ReadUInt(binaryReader);
				cost = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				max_stage = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVStone.bytes";
		}

		private static CSVStone instance = null;			
		public static CSVStone Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVStone 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVStone forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVStone();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVStone");

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
			shareData.ReadArray2s<uint>(binaryReader, 1, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVStone : FCSVStone
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVStone.bytes";
		}

		private static CSVStone instance = null;			
		public static CSVStone Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVStone 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVStone forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVStone();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVStone");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}