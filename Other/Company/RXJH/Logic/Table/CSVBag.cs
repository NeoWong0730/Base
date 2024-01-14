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

	sealed public partial class CSVBag : Framework.Table.TableBase<CSVBag.Data>
	{
	    sealed public partial class Data
	    {
			public readonly uint id; // 背包id
			public readonly uint bag; // 背包类型
			public readonly uint stage; // 背包等级
			public readonly uint capacity; // 容量
			public readonly uint upgradeCondition; // 升级条件
			public readonly List<uint> upgradeCost; // 升级消耗


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				bag = ReadHelper.ReadUInt(binaryReader);
				stage = ReadHelper.ReadUInt(binaryReader);
				capacity = ReadHelper.ReadUInt(binaryReader);
				upgradeCondition = ReadHelper.ReadUInt(binaryReader);
				upgradeCost = shareData.GetShareData<List<uint>>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVBag.bytes";
		}

		private static CSVBag instance = null;			
		public static CSVBag Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBag 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBag forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBag();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBag");

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

    sealed public partial class CSVBag : FCSVBag
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVBag.bytes";
		}

		private static CSVBag instance = null;			
		public static CSVBag Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBag 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBag forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBag();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBag");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}