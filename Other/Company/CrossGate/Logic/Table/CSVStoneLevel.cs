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

	sealed public partial class CSVStoneLevel : Framework.Table.TableBase<CSVStoneLevel.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint stone_id;
			public readonly uint stone_level;
			public readonly uint sum_stage;
			public readonly uint exp_stone;
			public readonly uint exp_stone_add;
			public readonly List<uint> cost_money;
			public readonly uint activeskill;
			public readonly uint passiveskill;
			public readonly uint decompose;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				stone_id = ReadHelper.ReadUInt(binaryReader);
				stone_level = ReadHelper.ReadUInt(binaryReader);
				sum_stage = ReadHelper.ReadUInt(binaryReader);
				exp_stone = ReadHelper.ReadUInt(binaryReader);
				exp_stone_add = ReadHelper.ReadUInt(binaryReader);
				cost_money = shareData.GetShareData<List<uint>>(binaryReader, 0);
				activeskill = ReadHelper.ReadUInt(binaryReader);
				passiveskill = ReadHelper.ReadUInt(binaryReader);
				decompose = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVStoneLevel.bytes";
		}

		private static CSVStoneLevel instance = null;			
		public static CSVStoneLevel Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVStoneLevel 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVStoneLevel forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVStoneLevel();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVStoneLevel");

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

    sealed public partial class CSVStoneLevel : FCSVStoneLevel
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVStoneLevel.bytes";
		}

		private static CSVStoneLevel instance = null;			
		public static CSVStoneLevel Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVStoneLevel 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVStoneLevel forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVStoneLevel();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVStoneLevel");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}