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

	sealed public partial class CSVMerchantFleetTask : Framework.Table.TableBase<CSVMerchantFleetTask.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly List<List<uint>> taskId1;
			public readonly List<List<uint>> taskId2;
			public readonly List<List<uint>> taskId3;
			public readonly List<uint> taskLimit;
			public readonly int businessType;
			public readonly uint battleMonster ;
			public readonly uint battleReward;
			public readonly int battlePoint;
			public readonly List<List<uint>> handItem;
			public readonly List<uint> handWeight;
			public readonly List<uint> handReward;
			public readonly List<List<uint>> handCost;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				taskId1 = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				taskId2 = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				taskId3 = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				taskLimit = shareData.GetShareData<List<uint>>(binaryReader, 0);
				businessType = ReadHelper.ReadInt(binaryReader);
				battleMonster  = ReadHelper.ReadUInt(binaryReader);
				battleReward = ReadHelper.ReadUInt(binaryReader);
				battlePoint = ReadHelper.ReadInt(binaryReader);
				handItem = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				handWeight = shareData.GetShareData<List<uint>>(binaryReader, 0);
				handReward = shareData.GetShareData<List<uint>>(binaryReader, 0);
				handCost = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVMerchantFleetTask.bytes";
		}

		private static CSVMerchantFleetTask instance = null;			
		public static CSVMerchantFleetTask Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVMerchantFleetTask 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVMerchantFleetTask forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVMerchantFleetTask();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVMerchantFleetTask");

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

    sealed public partial class CSVMerchantFleetTask : FCSVMerchantFleetTask
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVMerchantFleetTask.bytes";
		}

		private static CSVMerchantFleetTask instance = null;			
		public static CSVMerchantFleetTask Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVMerchantFleetTask 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVMerchantFleetTask forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVMerchantFleetTask();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVMerchantFleetTask");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}