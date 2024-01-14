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

	sealed public partial class CSVFamilyArchitectureEffect : Framework.Table.TableBase<CSVFamilyArchitectureEffect.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint HouseLevel;
			public readonly uint UnlockLimit;
			public readonly List<uint> UnlockSkillId;
			public readonly uint CapitalCeiling;
			public readonly uint DividendCap;
			public readonly uint SignInReward;
			public readonly uint DailyDonationLimit;
			public readonly List<uint> RewardCollectionTrigger;
			public readonly List<uint> DailyReward;
			public readonly uint BulidIcon;
			public readonly uint BulidName;
			public readonly uint CurrentEffectDescription;
			public readonly uint NextEffectDescription;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				HouseLevel = ReadHelper.ReadUInt(binaryReader);
				UnlockLimit = ReadHelper.ReadUInt(binaryReader);
				UnlockSkillId = shareData.GetShareData<List<uint>>(binaryReader, 0);
				CapitalCeiling = ReadHelper.ReadUInt(binaryReader);
				DividendCap = ReadHelper.ReadUInt(binaryReader);
				SignInReward = ReadHelper.ReadUInt(binaryReader);
				DailyDonationLimit = ReadHelper.ReadUInt(binaryReader);
				RewardCollectionTrigger = shareData.GetShareData<List<uint>>(binaryReader, 0);
				DailyReward = shareData.GetShareData<List<uint>>(binaryReader, 0);
				BulidIcon = ReadHelper.ReadUInt(binaryReader);
				BulidName = ReadHelper.ReadUInt(binaryReader);
				CurrentEffectDescription = ReadHelper.ReadUInt(binaryReader);
				NextEffectDescription = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyArchitectureEffect.bytes";
		}

		private static CSVFamilyArchitectureEffect instance = null;			
		public static CSVFamilyArchitectureEffect Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyArchitectureEffect 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyArchitectureEffect forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyArchitectureEffect();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyArchitectureEffect");

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

    sealed public partial class CSVFamilyArchitectureEffect : FCSVFamilyArchitectureEffect
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyArchitectureEffect.bytes";
		}

		private static CSVFamilyArchitectureEffect instance = null;			
		public static CSVFamilyArchitectureEffect Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyArchitectureEffect 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyArchitectureEffect forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyArchitectureEffect();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyArchitectureEffect");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}