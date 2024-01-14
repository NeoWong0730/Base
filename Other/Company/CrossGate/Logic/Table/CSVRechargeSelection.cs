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

	sealed public partial class CSVRechargeSelection : Framework.Table.TableBase<CSVRechargeSelection.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Activity_Id;
			public readonly uint Recharge;
			public readonly List<uint> reward;
			public readonly uint Sort;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Activity_Id = ReadHelper.ReadUInt(binaryReader);
				Recharge = ReadHelper.ReadUInt(binaryReader);
				reward = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Sort = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVRechargeSelection.bytes";
		}

		private static CSVRechargeSelection instance = null;			
		public static CSVRechargeSelection Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVRechargeSelection 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVRechargeSelection forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVRechargeSelection();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVRechargeSelection");

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

    sealed public partial class CSVRechargeSelection : FCSVRechargeSelection
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVRechargeSelection.bytes";
		}

		private static CSVRechargeSelection instance = null;			
		public static CSVRechargeSelection Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVRechargeSelection 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVRechargeSelection forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVRechargeSelection();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVRechargeSelection");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}