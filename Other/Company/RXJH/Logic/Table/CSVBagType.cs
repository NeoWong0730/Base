﻿//
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

	sealed public partial class CSVBagType : Framework.Table.TableBase<CSVBagType.Data>
	{
	    sealed public partial class Data
	    {
			public readonly uint id; // 背包id


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVBagType.bytes";
		}

		private static CSVBagType instance = null;			
		public static CSVBagType Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBagType 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBagType forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBagType();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBagType");

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

    sealed public partial class CSVBagType : FCSVBagType
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVBagType.bytes";
		}

		private static CSVBagType instance = null;			
		public static CSVBagType Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBagType 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBagType forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBagType();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBagType");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}