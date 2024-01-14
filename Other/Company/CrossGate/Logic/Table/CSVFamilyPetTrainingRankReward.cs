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

	sealed public partial class CSVFamilyPetTrainingRankReward : Framework.Table.TableBase<CSVFamilyPetTrainingRankReward.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint stage_id;
			public readonly List<uint> rankingRange;
			public readonly uint drop_id;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				stage_id = ReadHelper.ReadUInt(binaryReader);
				rankingRange = shareData.GetShareData<List<uint>>(binaryReader, 0);
				drop_id = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyPetTrainingRankReward.bytes";
		}

		private static CSVFamilyPetTrainingRankReward instance = null;			
		public static CSVFamilyPetTrainingRankReward Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyPetTrainingRankReward 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyPetTrainingRankReward forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyPetTrainingRankReward();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyPetTrainingRankReward");

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

    sealed public partial class CSVFamilyPetTrainingRankReward : FCSVFamilyPetTrainingRankReward
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyPetTrainingRankReward.bytes";
		}

		private static CSVFamilyPetTrainingRankReward instance = null;			
		public static CSVFamilyPetTrainingRankReward Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyPetTrainingRankReward 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyPetTrainingRankReward forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyPetTrainingRankReward();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyPetTrainingRankReward");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}