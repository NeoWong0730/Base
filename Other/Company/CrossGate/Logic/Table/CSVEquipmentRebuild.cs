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

	sealed public partial class CSVEquipmentRebuild : Framework.Table.TableBase<CSVEquipmentRebuild.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint type_id;
			public readonly uint aim_type_id;
			public readonly List<uint> attr_change1;
			public readonly List<uint> attr_change2;
			public readonly List<uint> attr_change3;
			public readonly List<uint> attr_change4;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				type_id = ReadHelper.ReadUInt(binaryReader);
				aim_type_id = ReadHelper.ReadUInt(binaryReader);
				attr_change1 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				attr_change2 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				attr_change3 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				attr_change4 = shareData.GetShareData<List<uint>>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVEquipmentRebuild.bytes";
		}

		private static CSVEquipmentRebuild instance = null;			
		public static CSVEquipmentRebuild Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVEquipmentRebuild 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVEquipmentRebuild forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVEquipmentRebuild();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVEquipmentRebuild");

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

    sealed public partial class CSVEquipmentRebuild : FCSVEquipmentRebuild
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVEquipmentRebuild.bytes";
		}

		private static CSVEquipmentRebuild instance = null;			
		public static CSVEquipmentRebuild Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVEquipmentRebuild 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVEquipmentRebuild forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVEquipmentRebuild();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVEquipmentRebuild");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}