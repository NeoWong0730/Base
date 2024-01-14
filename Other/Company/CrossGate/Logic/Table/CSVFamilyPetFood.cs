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

	sealed public partial class CSVFamilyPetFood : Framework.Table.TableBase<CSVFamilyPetFood.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint food_Type;
			public readonly uint isDrugs;
			public readonly uint stack;
			public readonly uint num;
			public readonly uint addGrowthValue;
			public readonly uint addMoodValue;
			public readonly uint addHealthValue;
			public readonly uint feedReward;
			public readonly List<List<uint>> feedRewardEXP;
			public readonly uint feedRewardCapital;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				food_Type = ReadHelper.ReadUInt(binaryReader);
				isDrugs = ReadHelper.ReadUInt(binaryReader);
				stack = ReadHelper.ReadUInt(binaryReader);
				num = ReadHelper.ReadUInt(binaryReader);
				addGrowthValue = ReadHelper.ReadUInt(binaryReader);
				addMoodValue = ReadHelper.ReadUInt(binaryReader);
				addHealthValue = ReadHelper.ReadUInt(binaryReader);
				feedReward = ReadHelper.ReadUInt(binaryReader);
				feedRewardEXP = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				feedRewardCapital = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyPetFood.bytes";
		}

		private static CSVFamilyPetFood instance = null;			
		public static CSVFamilyPetFood Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyPetFood 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyPetFood forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyPetFood();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyPetFood");

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

    sealed public partial class CSVFamilyPetFood : FCSVFamilyPetFood
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyPetFood.bytes";
		}

		private static CSVFamilyPetFood instance = null;			
		public static CSVFamilyPetFood Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyPetFood 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyPetFood forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyPetFood();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyPetFood");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}