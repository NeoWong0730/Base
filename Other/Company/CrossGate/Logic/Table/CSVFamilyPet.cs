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

	sealed public partial class CSVFamilyPet : Framework.Table.TableBase<CSVFamilyPet.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly string name;
			public readonly uint food_Type;
			public readonly uint stage;
			public readonly uint familyPet_id;
			public readonly uint icon_id;
			public readonly string icon2_id;
			public readonly uint growthValueMax;
			public readonly uint dailyGrowthValueMax;
			public readonly List<uint> train_id;
			public readonly uint simpleText;
			public readonly uint rewardPreview;
			public readonly uint diffcultyDetails;
			public readonly uint battle_id;
			public readonly List<List<uint>> moodPassiveSkill_id;
			public readonly List<List<uint>> healthPassiveSkill_id;
			public readonly uint trainingIntegralCondition;
			public readonly string model;
			public readonly string model_show;
			public readonly uint action_id;
			public readonly uint action_show_id;
			public readonly int positionx;
			public readonly int positiony;
			public readonly int positionz;
			public readonly int rotationx;
			public readonly int rotationy;
			public readonly int rotationz;
			public readonly int scale;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				name = shareData.GetShareData<string>(binaryReader, 0);
				food_Type = ReadHelper.ReadUInt(binaryReader);
				stage = ReadHelper.ReadUInt(binaryReader);
				familyPet_id = ReadHelper.ReadUInt(binaryReader);
				icon_id = ReadHelper.ReadUInt(binaryReader);
				icon2_id = shareData.GetShareData<string>(binaryReader, 0);
				growthValueMax = ReadHelper.ReadUInt(binaryReader);
				dailyGrowthValueMax = ReadHelper.ReadUInt(binaryReader);
				train_id = shareData.GetShareData<List<uint>>(binaryReader, 1);
				simpleText = ReadHelper.ReadUInt(binaryReader);
				rewardPreview = ReadHelper.ReadUInt(binaryReader);
				diffcultyDetails = ReadHelper.ReadUInt(binaryReader);
				battle_id = ReadHelper.ReadUInt(binaryReader);
				moodPassiveSkill_id = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				healthPassiveSkill_id = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				trainingIntegralCondition = ReadHelper.ReadUInt(binaryReader);
				model = shareData.GetShareData<string>(binaryReader, 0);
				model_show = shareData.GetShareData<string>(binaryReader, 0);
				action_id = ReadHelper.ReadUInt(binaryReader);
				action_show_id = ReadHelper.ReadUInt(binaryReader);
				positionx = ReadHelper.ReadInt(binaryReader);
				positiony = ReadHelper.ReadInt(binaryReader);
				positionz = ReadHelper.ReadInt(binaryReader);
				rotationx = ReadHelper.ReadInt(binaryReader);
				rotationy = ReadHelper.ReadInt(binaryReader);
				rotationz = ReadHelper.ReadInt(binaryReader);
				scale = ReadHelper.ReadInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyPet.bytes";
		}

		private static CSVFamilyPet instance = null;			
		public static CSVFamilyPet Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyPet 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyPet forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyPet();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyPet");

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
			TableShareData shareData = new TableShareData(3);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<uint>(binaryReader, 2, 1);

			return shareData;
		}
	}

#else

    sealed public partial class CSVFamilyPet : FCSVFamilyPet
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyPet.bytes";
		}

		private static CSVFamilyPet instance = null;			
		public static CSVFamilyPet Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyPet 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyPet forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyPet();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyPet");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}