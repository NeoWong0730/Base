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

	sealed public partial class CSVPetEquip : Framework.Table.TableBase<CSVPetEquip.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint equipment_category;
			public readonly uint des;
			public readonly uint equipment_level;
			public readonly uint attr_id;
			public readonly uint attr_num;
			public readonly List<uint> special_range;
			public readonly uint skill;
			public readonly List<List<uint>> forge_base;
			public readonly List<List<uint>> forge_special;
			public readonly List<uint> smelt;
			public readonly List<uint> decompose;
			public readonly uint sale_least;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				equipment_category = ReadHelper.ReadUInt(binaryReader);
				des = ReadHelper.ReadUInt(binaryReader);
				equipment_level = ReadHelper.ReadUInt(binaryReader);
				attr_id = ReadHelper.ReadUInt(binaryReader);
				attr_num = ReadHelper.ReadUInt(binaryReader);
				special_range = shareData.GetShareData<List<uint>>(binaryReader, 0);
				skill = ReadHelper.ReadUInt(binaryReader);
				forge_base = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				forge_special = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				smelt = shareData.GetShareData<List<uint>>(binaryReader, 0);
				decompose = shareData.GetShareData<List<uint>>(binaryReader, 0);
				sale_least = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVPetEquip.bytes";
		}

		private static CSVPetEquip instance = null;			
		public static CSVPetEquip Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetEquip 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetEquip forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetEquip();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetEquip");

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

    sealed public partial class CSVPetEquip : FCSVPetEquip
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVPetEquip.bytes";
		}

		private static CSVPetEquip instance = null;			
		public static CSVPetEquip Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetEquip 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetEquip forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetEquip();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetEquip");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}