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

	sealed public partial class CSVFamilySkillUp : Framework.Table.TableBase<CSVFamilySkillUp.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint SkillName;
			public readonly uint SkillLevel;
			public readonly uint Maxlevel;
			public readonly uint UpgradeCost;
			public readonly uint TotalUpgradeCost;
			public readonly uint SkillType;
			public readonly uint Parameter1;
			public readonly uint Parameter2;
			public readonly uint Parameter3;
			public readonly uint UpgradeConditions;
			public readonly uint HallUpgradeConditions;
			public readonly uint Upgrade;
			public readonly uint SkillLag;
			public readonly uint SkillIcon;
			public readonly uint BulidName;
			public readonly uint CurrentEffectDescription;
			public readonly uint MaxWords;
			public readonly uint UpgradeTime;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				SkillName = ReadHelper.ReadUInt(binaryReader);
				SkillLevel = ReadHelper.ReadUInt(binaryReader);
				Maxlevel = ReadHelper.ReadUInt(binaryReader);
				UpgradeCost = ReadHelper.ReadUInt(binaryReader);
				TotalUpgradeCost = ReadHelper.ReadUInt(binaryReader);
				SkillType = ReadHelper.ReadUInt(binaryReader);
				Parameter1 = ReadHelper.ReadUInt(binaryReader);
				Parameter2 = ReadHelper.ReadUInt(binaryReader);
				Parameter3 = ReadHelper.ReadUInt(binaryReader);
				UpgradeConditions = ReadHelper.ReadUInt(binaryReader);
				HallUpgradeConditions = ReadHelper.ReadUInt(binaryReader);
				Upgrade = ReadHelper.ReadUInt(binaryReader);
				SkillLag = ReadHelper.ReadUInt(binaryReader);
				SkillIcon = ReadHelper.ReadUInt(binaryReader);
				BulidName = ReadHelper.ReadUInt(binaryReader);
				CurrentEffectDescription = ReadHelper.ReadUInt(binaryReader);
				MaxWords = ReadHelper.ReadUInt(binaryReader);
				UpgradeTime = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFamilySkillUp.bytes";
		}

		private static CSVFamilySkillUp instance = null;			
		public static CSVFamilySkillUp Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilySkillUp 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilySkillUp forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilySkillUp();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilySkillUp");

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

    sealed public partial class CSVFamilySkillUp : FCSVFamilySkillUp
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFamilySkillUp.bytes";
		}

		private static CSVFamilySkillUp instance = null;			
		public static CSVFamilySkillUp Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilySkillUp 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilySkillUp forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilySkillUp();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilySkillUp");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}