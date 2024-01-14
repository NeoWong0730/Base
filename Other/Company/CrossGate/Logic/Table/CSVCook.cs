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

	sealed public partial class CSVCook : Framework.Table.TableBase<CSVCook.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint food_type;
			public readonly uint cook_type;
			public readonly uint name;
			public readonly uint desc;
			public readonly uint icon;
			public readonly uint sort;
			public readonly uint tool1;
			public readonly List<uint> fire_value1;
			public readonly List<List<uint>> food1;
			public readonly uint tool2;
			public readonly List<uint> fire_value2;
			public readonly List<List<uint>> food2;
			public readonly uint tool3;
			public readonly List<uint> fire_value3;
			public readonly List<List<uint>> food3;
			public readonly List<uint> need_score;
			public readonly List<uint> result;
			public readonly List<uint> cookbook;
			public readonly uint active_type;
			public readonly List<List<uint>> submit_item;
			public readonly uint weight;
			public bool is_special { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				food_type = ReadHelper.ReadUInt(binaryReader);
				cook_type = ReadHelper.ReadUInt(binaryReader);
				name = ReadHelper.ReadUInt(binaryReader);
				desc = ReadHelper.ReadUInt(binaryReader);
				icon = ReadHelper.ReadUInt(binaryReader);
				sort = ReadHelper.ReadUInt(binaryReader);
				tool1 = ReadHelper.ReadUInt(binaryReader);
				fire_value1 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				food1 = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				tool2 = ReadHelper.ReadUInt(binaryReader);
				fire_value2 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				food2 = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				tool3 = ReadHelper.ReadUInt(binaryReader);
				fire_value3 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				food3 = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				need_score = shareData.GetShareData<List<uint>>(binaryReader, 0);
				result = shareData.GetShareData<List<uint>>(binaryReader, 0);
				cookbook = shareData.GetShareData<List<uint>>(binaryReader, 0);
				active_type = ReadHelper.ReadUInt(binaryReader);
				submit_item = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				weight = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVCook.bytes";
		}

		private static CSVCook instance = null;			
		public static CSVCook Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCook 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCook forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCook();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCook");

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

    sealed public partial class CSVCook : FCSVCook
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVCook.bytes";
		}

		private static CSVCook instance = null;			
		public static CSVCook Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCook 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCook forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCook();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCook");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}