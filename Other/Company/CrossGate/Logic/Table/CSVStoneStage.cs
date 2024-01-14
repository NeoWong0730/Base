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

	sealed public partial class CSVStoneStage : Framework.Table.TableBase<CSVStoneStage.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint stone_id;
			public readonly uint stage;
			public readonly uint stone_level;
			public readonly List<List<uint>> cost_item;
			public readonly uint icon_light;
			public readonly uint desc_light;
			public readonly uint icon_dark;
			public readonly uint desc_dark;
			public readonly List<List<uint>> reverse_cost;
			public readonly uint decompose;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				stone_id = ReadHelper.ReadUInt(binaryReader);
				stage = ReadHelper.ReadUInt(binaryReader);
				stone_level = ReadHelper.ReadUInt(binaryReader);
				cost_item = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				icon_light = ReadHelper.ReadUInt(binaryReader);
				desc_light = ReadHelper.ReadUInt(binaryReader);
				icon_dark = ReadHelper.ReadUInt(binaryReader);
				desc_dark = ReadHelper.ReadUInt(binaryReader);
				reverse_cost = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				decompose = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVStoneStage.bytes";
		}

		private static CSVStoneStage instance = null;			
		public static CSVStoneStage Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVStoneStage 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVStoneStage forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVStoneStage();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVStoneStage");

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

    sealed public partial class CSVStoneStage : FCSVStoneStage
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVStoneStage.bytes";
		}

		private static CSVStoneStage instance = null;			
		public static CSVStoneStage Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVStoneStage 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVStoneStage forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVStoneStage();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVStoneStage");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}