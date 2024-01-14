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

	sealed public partial class CSVActivityShop : Framework.Table.TableBase<CSVActivityShop.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Type;
			public readonly uint show_num;
			public readonly uint Show_Discount;
			public readonly List<uint> Show_Currency;
			public readonly uint Theme_Coin;
			public readonly List<List<uint>> Break_Item;
			public readonly uint Mail_Id;
			public readonly List<uint> RefreshTime;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Type = ReadHelper.ReadUInt(binaryReader);
				show_num = ReadHelper.ReadUInt(binaryReader);
				Show_Discount = ReadHelper.ReadUInt(binaryReader);
				Show_Currency = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Theme_Coin = ReadHelper.ReadUInt(binaryReader);
				Break_Item = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				Mail_Id = ReadHelper.ReadUInt(binaryReader);
				RefreshTime = shareData.GetShareData<List<uint>>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVActivityShop.bytes";
		}

		private static CSVActivityShop instance = null;			
		public static CSVActivityShop Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVActivityShop 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVActivityShop forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVActivityShop();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVActivityShop");

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

    sealed public partial class CSVActivityShop : FCSVActivityShop
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVActivityShop.bytes";
		}

		private static CSVActivityShop instance = null;			
		public static CSVActivityShop Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVActivityShop 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVActivityShop forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVActivityShop();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVActivityShop");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}