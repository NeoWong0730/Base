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

	sealed public partial class CSVShop : Framework.Table.TableBase<CSVShop.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint show_num;
			public readonly uint random_show;
			public readonly uint price_type;
			public readonly List<List<uint>> refresh_time;
			public readonly List<uint> show_currency;
			public readonly uint tab_icon;
			public readonly List<uint> refresh_random;
			public readonly uint npc_name;
			public readonly uint npc_id;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				show_num = ReadHelper.ReadUInt(binaryReader);
				random_show = ReadHelper.ReadUInt(binaryReader);
				price_type = ReadHelper.ReadUInt(binaryReader);
				refresh_time = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				show_currency = shareData.GetShareData<List<uint>>(binaryReader, 0);
				tab_icon = ReadHelper.ReadUInt(binaryReader);
				refresh_random = shareData.GetShareData<List<uint>>(binaryReader, 0);
				npc_name = ReadHelper.ReadUInt(binaryReader);
				npc_id = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVShop.bytes";
		}

		private static CSVShop instance = null;			
		public static CSVShop Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVShop 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVShop forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVShop();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVShop");

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

    sealed public partial class CSVShop : FCSVShop
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVShop.bytes";
		}

		private static CSVShop instance = null;			
		public static CSVShop Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVShop 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVShop forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVShop();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVShop");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}