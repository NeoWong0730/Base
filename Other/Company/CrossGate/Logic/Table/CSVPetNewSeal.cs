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

	sealed public partial class CSVPetNewSeal : Framework.Table.TableBase<CSVPetNewSeal.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint card;
			public readonly uint seal_area_spe;
			public readonly List<uint> seal_quality;
			public readonly uint seal_type;
			public readonly uint map;
			public readonly List<int> coordinate_center;
			public readonly List<uint> center_range;
			public readonly uint seal_range;
			public readonly List<int> coordinate;
			public readonly uint AccInfo;
			public readonly uint enemy_group;
			public readonly uint seal_difficulty;
			public readonly uint frequency;
			public bool is_msg { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public bool is_unloce { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				card = ReadHelper.ReadUInt(binaryReader);
				seal_area_spe = ReadHelper.ReadUInt(binaryReader);
				seal_quality = shareData.GetShareData<List<uint>>(binaryReader, 0);
				seal_type = ReadHelper.ReadUInt(binaryReader);
				map = ReadHelper.ReadUInt(binaryReader);
				coordinate_center = shareData.GetShareData<List<int>>(binaryReader, 1);
				center_range = shareData.GetShareData<List<uint>>(binaryReader, 0);
				seal_range = ReadHelper.ReadUInt(binaryReader);
				coordinate = shareData.GetShareData<List<int>>(binaryReader, 1);
				AccInfo = ReadHelper.ReadUInt(binaryReader);
				enemy_group = ReadHelper.ReadUInt(binaryReader);
				seal_difficulty = ReadHelper.ReadUInt(binaryReader);
				frequency = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVPetNewSeal.bytes";
		}

		private static CSVPetNewSeal instance = null;			
		public static CSVPetNewSeal Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetNewSeal 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetNewSeal forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetNewSeal();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetNewSeal");

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
			shareData.ReadArrays<int>(binaryReader, 1, ReadHelper.ReadArray_ReadInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVPetNewSeal : FCSVPetNewSeal
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVPetNewSeal.bytes";
		}

		private static CSVPetNewSeal instance = null;			
		public static CSVPetNewSeal Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetNewSeal 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetNewSeal forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetNewSeal();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetNewSeal");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}