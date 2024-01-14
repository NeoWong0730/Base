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

	sealed public partial class CSVTianTiCumlativeRewards : Framework.Table.TableBase<CSVTianTiCumlativeRewards.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint SeasonDefaultRward;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				SeasonDefaultRward = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTianTiCumlativeRewards.bytes";
		}

		private static CSVTianTiCumlativeRewards instance = null;			
		public static CSVTianTiCumlativeRewards Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTianTiCumlativeRewards 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTianTiCumlativeRewards forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTianTiCumlativeRewards();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTianTiCumlativeRewards");

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
			TableShareData shareData = null;

			return shareData;
		}
	}

#else

    sealed public partial class CSVTianTiCumlativeRewards : FCSVTianTiCumlativeRewards
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTianTiCumlativeRewards.bytes";
		}

		private static CSVTianTiCumlativeRewards instance = null;			
		public static CSVTianTiCumlativeRewards Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTianTiCumlativeRewards 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTianTiCumlativeRewards forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTianTiCumlativeRewards();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTianTiCumlativeRewards");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}