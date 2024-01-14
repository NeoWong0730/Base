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

	sealed public partial class CSVFamilyArchitecture : Framework.Table.TableBase<CSVFamilyArchitecture.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Architecture;
			public readonly uint BuildLevel;
			public readonly uint MaxLevel;
			public readonly uint InitialLevel;
			public readonly List<uint> FrontBuilding;
			public readonly uint DemandLevel;
			public readonly uint demandProsperityLevel;
			public readonly uint BuildLag;
			public readonly uint FundsRequired;
			public readonly uint MaintenanceCost;
			public readonly uint RefundOfFees;
			public readonly uint UpgradeTime;
			public readonly uint Name;
			public readonly uint Introduce;
			public readonly uint iconId;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Architecture = ReadHelper.ReadUInt(binaryReader);
				BuildLevel = ReadHelper.ReadUInt(binaryReader);
				MaxLevel = ReadHelper.ReadUInt(binaryReader);
				InitialLevel = ReadHelper.ReadUInt(binaryReader);
				FrontBuilding = shareData.GetShareData<List<uint>>(binaryReader, 0);
				DemandLevel = ReadHelper.ReadUInt(binaryReader);
				demandProsperityLevel = ReadHelper.ReadUInt(binaryReader);
				BuildLag = ReadHelper.ReadUInt(binaryReader);
				FundsRequired = ReadHelper.ReadUInt(binaryReader);
				MaintenanceCost = ReadHelper.ReadUInt(binaryReader);
				RefundOfFees = ReadHelper.ReadUInt(binaryReader);
				UpgradeTime = ReadHelper.ReadUInt(binaryReader);
				Name = ReadHelper.ReadUInt(binaryReader);
				Introduce = ReadHelper.ReadUInt(binaryReader);
				iconId = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyArchitecture.bytes";
		}

		private static CSVFamilyArchitecture instance = null;			
		public static CSVFamilyArchitecture Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyArchitecture 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyArchitecture forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyArchitecture();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyArchitecture");

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

    sealed public partial class CSVFamilyArchitecture : FCSVFamilyArchitecture
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyArchitecture.bytes";
		}

		private static CSVFamilyArchitecture instance = null;			
		public static CSVFamilyArchitecture Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyArchitecture 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyArchitecture forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyArchitecture();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyArchitecture");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}