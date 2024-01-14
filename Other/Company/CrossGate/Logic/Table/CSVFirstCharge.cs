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

	sealed public partial class CSVFirstCharge : Framework.Table.TableBase<CSVFirstCharge.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Job_Id;
			public readonly List<List<int>> Reward_Items_d1;
			public readonly List<List<int>> Reward_Items_d2;
			public readonly List<List<int>> Reward_Items_d3;
			public readonly uint Title_Des;
			public readonly List<uint> Item_Des_d1;
			public readonly List<uint> Item_Des_d2;
			public readonly List<uint> Item_Des_d3;
			public readonly List<string> Show_Item;
			public readonly List<string> Show_Id;
			public readonly List<string> Show_height;
			public readonly List<uint> Item_Size;
			public readonly List<List<string>> spin_coordinate;
			public readonly uint spin_speed;
			public readonly uint spin_axle;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Job_Id = ReadHelper.ReadUInt(binaryReader);
				Reward_Items_d1 = shareData.GetShareData<List<List<int>>>(binaryReader, 4);
				Reward_Items_d2 = shareData.GetShareData<List<List<int>>>(binaryReader, 4);
				Reward_Items_d3 = shareData.GetShareData<List<List<int>>>(binaryReader, 4);
				Title_Des = ReadHelper.ReadUInt(binaryReader);
				Item_Des_d1 = shareData.GetShareData<List<uint>>(binaryReader, 2);
				Item_Des_d2 = shareData.GetShareData<List<uint>>(binaryReader, 2);
				Item_Des_d3 = shareData.GetShareData<List<uint>>(binaryReader, 2);
				Show_Item = shareData.GetShareData<List<string>>(binaryReader, 3);
				Show_Id = shareData.GetShareData<List<string>>(binaryReader, 3);
				Show_height = shareData.GetShareData<List<string>>(binaryReader, 3);
				Item_Size = shareData.GetShareData<List<uint>>(binaryReader, 2);
				spin_coordinate = shareData.GetShareData<List<List<string>>>(binaryReader, 5);
				spin_speed = ReadHelper.ReadUInt(binaryReader);
				spin_axle = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFirstCharge.bytes";
		}

		private static CSVFirstCharge instance = null;			
		public static CSVFirstCharge Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFirstCharge 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFirstCharge forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFirstCharge();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFirstCharge");

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
			TableShareData shareData = new TableShareData(6);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<int>(binaryReader, 1, ReadHelper.ReadArray_ReadInt);
			shareData.ReadArrays<uint>(binaryReader, 2, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadStringArrays(binaryReader, 3, 0);
			shareData.ReadArray2s<int>(binaryReader, 4, 1);
			shareData.ReadArray2s<string>(binaryReader, 5, 3);

			return shareData;
		}
	}

#else

    sealed public partial class CSVFirstCharge : FCSVFirstCharge
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFirstCharge.bytes";
		}

		private static CSVFirstCharge instance = null;			
		public static CSVFirstCharge Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFirstCharge 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFirstCharge forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFirstCharge();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFirstCharge");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}