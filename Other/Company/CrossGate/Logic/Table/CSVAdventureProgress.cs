﻿//
//#define USE_HOTFIX_LOGIC

using Lib.AssetLoader;
using Lib.Core;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Framework.Table;

namespace Table
{
#if USE_HOTFIX_LOGIC

	sealed public partial class CSVAdventureProgress : Framework.Table.TableBase<CSVAdventureProgress.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint name;
			public readonly uint icon;
			public readonly uint sort;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				name = ReadHelper.ReadUInt(binaryReader);
				icon = ReadHelper.ReadUInt(binaryReader);
				sort = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVAdventureProgress.bytes";
		}

		private static CSVAdventureProgress instance = null;			
		public static CSVAdventureProgress Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAdventureProgress 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAdventureProgress forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAdventureProgress();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAdventureProgress");

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

    sealed public partial class CSVAdventureProgress : FCSVAdventureProgress
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVAdventureProgress.bytes";
		}

		private static CSVAdventureProgress instance = null;			
		public static CSVAdventureProgress Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAdventureProgress 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAdventureProgress forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAdventureProgress();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAdventureProgress");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}