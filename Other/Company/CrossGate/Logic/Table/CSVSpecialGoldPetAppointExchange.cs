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

	sealed public partial class CSVSpecialGoldPetAppointExchange : Framework.Table.TableBase<CSVSpecialGoldPetAppointExchange.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint pet_id;
			public readonly List<uint> RecyclingItem1;
			public readonly List<uint> RecyclingItem2;
			public readonly List<uint> RecyclingItem3;
			public readonly List<uint> RecyclingItem4;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				pet_id = ReadHelper.ReadUInt(binaryReader);
				RecyclingItem1 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				RecyclingItem2 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				RecyclingItem3 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				RecyclingItem4 = shareData.GetShareData<List<uint>>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVSpecialGoldPetAppointExchange.bytes";
		}

		private static CSVSpecialGoldPetAppointExchange instance = null;			
		public static CSVSpecialGoldPetAppointExchange Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSpecialGoldPetAppointExchange 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSpecialGoldPetAppointExchange forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSpecialGoldPetAppointExchange();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSpecialGoldPetAppointExchange");

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

    sealed public partial class CSVSpecialGoldPetAppointExchange : FCSVSpecialGoldPetAppointExchange
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVSpecialGoldPetAppointExchange.bytes";
		}

		private static CSVSpecialGoldPetAppointExchange instance = null;			
		public static CSVSpecialGoldPetAppointExchange Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSpecialGoldPetAppointExchange 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSpecialGoldPetAppointExchange forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSpecialGoldPetAppointExchange();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSpecialGoldPetAppointExchange");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}