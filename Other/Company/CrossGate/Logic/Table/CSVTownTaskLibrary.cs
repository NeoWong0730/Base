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

	sealed public partial class CSVTownTaskLibrary : Framework.Table.TableBase<CSVTownTaskLibrary.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint TaskType;
			public readonly uint TownTaskLv;
			public readonly uint NeedFavorabilityLv;
			public readonly uint ConsumePoints;
			public readonly List<uint> TaskLv;
			public readonly uint TaskConsumeItem;
			public readonly uint ConsumeItemNum;
			public readonly uint TaskConsumeEqpt;
			public readonly uint ConsumeEqptQuality;
			public readonly uint TaskReward;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				TaskType = ReadHelper.ReadUInt(binaryReader);
				TownTaskLv = ReadHelper.ReadUInt(binaryReader);
				NeedFavorabilityLv = ReadHelper.ReadUInt(binaryReader);
				ConsumePoints = ReadHelper.ReadUInt(binaryReader);
				TaskLv = shareData.GetShareData<List<uint>>(binaryReader, 0);
				TaskConsumeItem = ReadHelper.ReadUInt(binaryReader);
				ConsumeItemNum = ReadHelper.ReadUInt(binaryReader);
				TaskConsumeEqpt = ReadHelper.ReadUInt(binaryReader);
				ConsumeEqptQuality = ReadHelper.ReadUInt(binaryReader);
				TaskReward = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTownTaskLibrary.bytes";
		}

		private static CSVTownTaskLibrary instance = null;			
		public static CSVTownTaskLibrary Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTownTaskLibrary 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTownTaskLibrary forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTownTaskLibrary();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTownTaskLibrary");

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

    sealed public partial class CSVTownTaskLibrary : FCSVTownTaskLibrary
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTownTaskLibrary.bytes";
		}

		private static CSVTownTaskLibrary instance = null;			
		public static CSVTownTaskLibrary Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTownTaskLibrary 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTownTaskLibrary forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTownTaskLibrary();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTownTaskLibrary");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}