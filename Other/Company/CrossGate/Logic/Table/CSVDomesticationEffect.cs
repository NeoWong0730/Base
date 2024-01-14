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

	sealed public partial class CSVDomesticationEffect : Framework.Table.TableBase<CSVDomesticationEffect.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly int rank_gap;
			public readonly int level_gap;
			public readonly int add_damage;
			public readonly int add_control;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				rank_gap = ReadHelper.ReadInt(binaryReader);
				level_gap = ReadHelper.ReadInt(binaryReader);
				add_damage = ReadHelper.ReadInt(binaryReader);
				add_control = ReadHelper.ReadInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVDomesticationEffect.bytes";
		}

		private static CSVDomesticationEffect instance = null;			
		public static CSVDomesticationEffect Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVDomesticationEffect 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVDomesticationEffect forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVDomesticationEffect();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVDomesticationEffect");

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

    sealed public partial class CSVDomesticationEffect : FCSVDomesticationEffect
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVDomesticationEffect.bytes";
		}

		private static CSVDomesticationEffect instance = null;			
		public static CSVDomesticationEffect Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVDomesticationEffect 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVDomesticationEffect forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVDomesticationEffect();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVDomesticationEffect");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}