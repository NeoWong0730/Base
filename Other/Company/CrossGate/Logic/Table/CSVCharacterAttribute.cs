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

	sealed public partial class CSVCharacterAttribute : Framework.Table.TableBase<CSVCharacterAttribute.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint attr_id;
			public readonly uint totol_exp;
			public readonly uint upgrade_exp;
			public readonly List<List<uint>> attribute;
			public readonly uint offline_exp;
			public readonly byte pet_summon_num;
			public readonly uint VitalityLimit;
			public readonly uint AidPointLimit;
			public readonly uint CaptainPointLimit;
			public readonly uint PerWorkingHourExp;
			public readonly uint DailyHangupTotalExp;
			public readonly uint WelfareExp;
			public readonly uint FamilyReceptionSecondExp;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				attr_id = ReadHelper.ReadUInt(binaryReader);
				totol_exp = ReadHelper.ReadUInt(binaryReader);
				upgrade_exp = ReadHelper.ReadUInt(binaryReader);
				attribute = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				offline_exp = ReadHelper.ReadUInt(binaryReader);
				pet_summon_num = ReadHelper.ReadByte(binaryReader);
				VitalityLimit = ReadHelper.ReadUInt(binaryReader);
				AidPointLimit = ReadHelper.ReadUInt(binaryReader);
				CaptainPointLimit = ReadHelper.ReadUInt(binaryReader);
				PerWorkingHourExp = ReadHelper.ReadUInt(binaryReader);
				DailyHangupTotalExp = ReadHelper.ReadUInt(binaryReader);
				WelfareExp = ReadHelper.ReadUInt(binaryReader);
				FamilyReceptionSecondExp = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVCharacterAttribute.bytes";
		}

		private static CSVCharacterAttribute instance = null;			
		public static CSVCharacterAttribute Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCharacterAttribute 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCharacterAttribute forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCharacterAttribute();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCharacterAttribute");

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

    sealed public partial class CSVCharacterAttribute : FCSVCharacterAttribute
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVCharacterAttribute.bytes";
		}

		private static CSVCharacterAttribute instance = null;			
		public static CSVCharacterAttribute Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCharacterAttribute 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCharacterAttribute forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCharacterAttribute();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCharacterAttribute");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}