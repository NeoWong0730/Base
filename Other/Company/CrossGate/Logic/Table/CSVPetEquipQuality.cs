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

	sealed public partial class CSVPetEquipQuality : Framework.Table.TableBase<CSVPetEquipQuality.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint is_band;
			public readonly uint lower_limit;
			public readonly uint upper_limit;
			public readonly List<List<uint>> green;
			public readonly List<List<uint>> effect;
			public readonly List<uint> quality_weight;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				is_band = ReadHelper.ReadUInt(binaryReader);
				lower_limit = ReadHelper.ReadUInt(binaryReader);
				upper_limit = ReadHelper.ReadUInt(binaryReader);
				green = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				effect = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				quality_weight = shareData.GetShareData<List<uint>>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVPetEquipQuality.bytes";
		}

		private static CSVPetEquipQuality instance = null;			
		public static CSVPetEquipQuality Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetEquipQuality 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetEquipQuality forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetEquipQuality();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetEquipQuality");

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

    sealed public partial class CSVPetEquipQuality : FCSVPetEquipQuality
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVPetEquipQuality.bytes";
		}

		private static CSVPetEquipQuality instance = null;			
		public static CSVPetEquipQuality Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetEquipQuality 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetEquipQuality forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetEquipQuality();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetEquipQuality");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}