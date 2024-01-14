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

	sealed public partial class CSVDaliyPacks : Framework.Table.TableBase<CSVDaliyPacks.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint need_level;
			public readonly uint pack1_name;
			public readonly uint drop1_Id;
			public readonly uint drop2_Id;
			public readonly List<uint> price_pack1;
			public readonly uint pack2_name;
			public readonly uint drop3_Id;
			public readonly uint drop4_Id;
			public readonly List<uint> price_pack2;
			public readonly uint pack3_name;
			public readonly uint drop5_Id;
			public readonly uint drop6_Id;
			public readonly List<uint> price_pack3;
			public readonly uint dailyDrop_Id;
			public readonly List<uint> price_7day;
			public readonly uint currecny_sign;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				need_level = ReadHelper.ReadUInt(binaryReader);
				pack1_name = ReadHelper.ReadUInt(binaryReader);
				drop1_Id = ReadHelper.ReadUInt(binaryReader);
				drop2_Id = ReadHelper.ReadUInt(binaryReader);
				price_pack1 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				pack2_name = ReadHelper.ReadUInt(binaryReader);
				drop3_Id = ReadHelper.ReadUInt(binaryReader);
				drop4_Id = ReadHelper.ReadUInt(binaryReader);
				price_pack2 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				pack3_name = ReadHelper.ReadUInt(binaryReader);
				drop5_Id = ReadHelper.ReadUInt(binaryReader);
				drop6_Id = ReadHelper.ReadUInt(binaryReader);
				price_pack3 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				dailyDrop_Id = ReadHelper.ReadUInt(binaryReader);
				price_7day = shareData.GetShareData<List<uint>>(binaryReader, 0);
				currecny_sign = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVDaliyPacks.bytes";
		}

		private static CSVDaliyPacks instance = null;			
		public static CSVDaliyPacks Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVDaliyPacks 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVDaliyPacks forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVDaliyPacks();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVDaliyPacks");

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

    sealed public partial class CSVDaliyPacks : FCSVDaliyPacks
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVDaliyPacks.bytes";
		}

		private static CSVDaliyPacks instance = null;			
		public static CSVDaliyPacks Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVDaliyPacks 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVDaliyPacks forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVDaliyPacks();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVDaliyPacks");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}