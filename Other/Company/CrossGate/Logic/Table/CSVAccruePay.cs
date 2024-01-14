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

	sealed public partial class CSVAccruePay : Framework.Table.TableBase<CSVAccruePay.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Activity_Id;
			public readonly uint Recharge;
			public readonly uint reward;
			public readonly uint Sort;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Activity_Id = ReadHelper.ReadUInt(binaryReader);
				Recharge = ReadHelper.ReadUInt(binaryReader);
				reward = ReadHelper.ReadUInt(binaryReader);
				Sort = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVAccruePay.bytes";
		}

		private static CSVAccruePay instance = null;			
		public static CSVAccruePay Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAccruePay 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAccruePay forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAccruePay();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAccruePay");

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

    sealed public partial class CSVAccruePay : FCSVAccruePay
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVAccruePay.bytes";
		}

		private static CSVAccruePay instance = null;			
		public static CSVAccruePay Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAccruePay 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAccruePay forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAccruePay();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAccruePay");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}