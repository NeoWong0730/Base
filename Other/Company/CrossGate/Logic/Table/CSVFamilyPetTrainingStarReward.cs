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

	sealed public partial class CSVFamilyPetTrainingStarReward : Framework.Table.TableBase<CSVFamilyPetTrainingStarReward.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint stage_id;
			public readonly uint trainingStar;
			public readonly uint trainingIntegralCondition;
			public readonly uint drop_id;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				stage_id = ReadHelper.ReadUInt(binaryReader);
				trainingStar = ReadHelper.ReadUInt(binaryReader);
				trainingIntegralCondition = ReadHelper.ReadUInt(binaryReader);
				drop_id = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyPetTrainingStarReward.bytes";
		}

		private static CSVFamilyPetTrainingStarReward instance = null;			
		public static CSVFamilyPetTrainingStarReward Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyPetTrainingStarReward 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyPetTrainingStarReward forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyPetTrainingStarReward();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyPetTrainingStarReward");

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

    sealed public partial class CSVFamilyPetTrainingStarReward : FCSVFamilyPetTrainingStarReward
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyPetTrainingStarReward.bytes";
		}

		private static CSVFamilyPetTrainingStarReward instance = null;			
		public static CSVFamilyPetTrainingStarReward Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyPetTrainingStarReward 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyPetTrainingStarReward forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyPetTrainingStarReward();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyPetTrainingStarReward");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}