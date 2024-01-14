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

	sealed public partial class CSVEquipmentParameter : Framework.Table.TableBase<CSVEquipmentParameter.Data>
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
			public bool custom_made { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly List<List<uint>> base_attr;
			public readonly List<List<uint>> green_attr;
			public readonly List<uint> special;
			public readonly uint quality;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				is_band = ReadHelper.ReadUInt(binaryReader);
				lower_limit = ReadHelper.ReadUInt(binaryReader);
				upper_limit = ReadHelper.ReadUInt(binaryReader);
				green = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				effect = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				quality_weight = shareData.GetShareData<List<uint>>(binaryReader, 0);
				base_attr = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				green_attr = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				special = shareData.GetShareData<List<uint>>(binaryReader, 0);
				quality = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVEquipmentParameter.bytes";
		}

		private static CSVEquipmentParameter instance = null;			
		public static CSVEquipmentParameter Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVEquipmentParameter 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVEquipmentParameter forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVEquipmentParameter();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVEquipmentParameter");

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

    sealed public partial class CSVEquipmentParameter : FCSVEquipmentParameter
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVEquipmentParameter.bytes";
		}

		private static CSVEquipmentParameter instance = null;			
		public static CSVEquipmentParameter Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVEquipmentParameter 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVEquipmentParameter forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVEquipmentParameter();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVEquipmentParameter");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}