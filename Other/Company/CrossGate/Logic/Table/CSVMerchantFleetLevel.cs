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

	sealed public partial class CSVMerchantFleetLevel : Framework.Table.TableBase<CSVMerchantFleetLevel.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint allExp;
			public readonly uint singleExp;
			public readonly string picture;
			public readonly uint levelReward;
			public readonly List<List<uint>> functionOpen;
			public readonly uint functionLan;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				allExp = ReadHelper.ReadUInt(binaryReader);
				singleExp = ReadHelper.ReadUInt(binaryReader);
				picture = shareData.GetShareData<string>(binaryReader, 0);
				levelReward = ReadHelper.ReadUInt(binaryReader);
				functionOpen = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				functionLan = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVMerchantFleetLevel.bytes";
		}

		private static CSVMerchantFleetLevel instance = null;			
		public static CSVMerchantFleetLevel Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVMerchantFleetLevel 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVMerchantFleetLevel forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVMerchantFleetLevel();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVMerchantFleetLevel");

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
			TableShareData shareData = new TableShareData(3);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<uint>(binaryReader, 2, 1);

			return shareData;
		}
	}

#else

    sealed public partial class CSVMerchantFleetLevel : FCSVMerchantFleetLevel
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVMerchantFleetLevel.bytes";
		}

		private static CSVMerchantFleetLevel instance = null;			
		public static CSVMerchantFleetLevel Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVMerchantFleetLevel 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVMerchantFleetLevel forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVMerchantFleetLevel();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVMerchantFleetLevel");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}