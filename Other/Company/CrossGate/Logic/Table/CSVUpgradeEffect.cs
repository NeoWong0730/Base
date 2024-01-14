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

	sealed public partial class CSVUpgradeEffect : Framework.Table.TableBase<CSVUpgradeEffect.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint lev;
			public readonly List<uint> effect_beg_id;
			public readonly uint quality;
			public readonly List<uint> effect_beg_max;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				lev = ReadHelper.ReadUInt(binaryReader);
				effect_beg_id = shareData.GetShareData<List<uint>>(binaryReader, 0);
				quality = ReadHelper.ReadUInt(binaryReader);
				effect_beg_max = shareData.GetShareData<List<uint>>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVUpgradeEffect.bytes";
		}

		private static CSVUpgradeEffect instance = null;			
		public static CSVUpgradeEffect Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVUpgradeEffect 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVUpgradeEffect forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVUpgradeEffect();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVUpgradeEffect");

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

    sealed public partial class CSVUpgradeEffect : FCSVUpgradeEffect
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVUpgradeEffect.bytes";
		}

		private static CSVUpgradeEffect instance = null;			
		public static CSVUpgradeEffect Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVUpgradeEffect 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVUpgradeEffect forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVUpgradeEffect();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVUpgradeEffect");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}