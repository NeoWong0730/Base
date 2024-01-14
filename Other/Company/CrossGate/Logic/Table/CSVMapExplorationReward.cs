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

	sealed public partial class CSVMapExplorationReward : Framework.Table.TableBase<CSVMapExplorationReward.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint type;
			public readonly uint mapId;
			public readonly List<List<uint>> resource;
			public readonly uint ExplorationDegree;
			public readonly uint DropId;
			public readonly uint lan;
			public readonly uint title_lan;
			public readonly uint title_icon;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				type = ReadHelper.ReadUInt(binaryReader);
				mapId = ReadHelper.ReadUInt(binaryReader);
				resource = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				ExplorationDegree = ReadHelper.ReadUInt(binaryReader);
				DropId = ReadHelper.ReadUInt(binaryReader);
				lan = ReadHelper.ReadUInt(binaryReader);
				title_lan = ReadHelper.ReadUInt(binaryReader);
				title_icon = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVMapExplorationReward.bytes";
		}

		private static CSVMapExplorationReward instance = null;			
		public static CSVMapExplorationReward Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVMapExplorationReward 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVMapExplorationReward forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVMapExplorationReward();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVMapExplorationReward");

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

    sealed public partial class CSVMapExplorationReward : FCSVMapExplorationReward
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVMapExplorationReward.bytes";
		}

		private static CSVMapExplorationReward instance = null;			
		public static CSVMapExplorationReward Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVMapExplorationReward 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVMapExplorationReward forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVMapExplorationReward();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVMapExplorationReward");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}