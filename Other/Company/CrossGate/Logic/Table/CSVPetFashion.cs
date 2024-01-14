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

	sealed public partial class CSVPetFashion : Framework.Table.TableBase<CSVPetFashion.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Petid;
			public readonly uint FashionName;
			public readonly string Icon_Path;
			public readonly List<string> model_show;
			public readonly List<string> model;
			public readonly uint FashionPrice;
			public readonly List<List<uint>> WeaponColour;
			public readonly List<uint> itemNeed;
			public readonly List<List<uint>> attr1_id;
			public readonly List<List<uint>> attr2_id;
			public readonly List<List<uint>> attr3_id;
			public bool Recommend { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public bool Hide { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Petid = ReadHelper.ReadUInt(binaryReader);
				FashionName = ReadHelper.ReadUInt(binaryReader);
				Icon_Path = shareData.GetShareData<string>(binaryReader, 0);
				model_show = shareData.GetShareData<List<string>>(binaryReader, 1);
				model = shareData.GetShareData<List<string>>(binaryReader, 1);
				FashionPrice = ReadHelper.ReadUInt(binaryReader);
				WeaponColour = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				itemNeed = shareData.GetShareData<List<uint>>(binaryReader, 2);
				attr1_id = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				attr2_id = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				attr3_id = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVPetFashion.bytes";
		}

		private static CSVPetFashion instance = null;			
		public static CSVPetFashion Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetFashion 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetFashion forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetFashion();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetFashion");

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
			TableShareData shareData = new TableShareData(4);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadStringArrays(binaryReader, 1, 0);
			shareData.ReadArrays<uint>(binaryReader, 2, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<uint>(binaryReader, 3, 2);

			return shareData;
		}
	}

#else

    sealed public partial class CSVPetFashion : FCSVPetFashion
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVPetFashion.bytes";
		}

		private static CSVPetFashion instance = null;			
		public static CSVPetFashion Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetFashion 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetFashion forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetFashion();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetFashion");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}