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

	sealed public partial class CSVFavorabilityBehavior : Framework.Table.TableBase<CSVFavorabilityBehavior.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Name;
			public readonly uint CostPoint;
			public readonly uint DailyTotal;
			public readonly uint DailyTimes;
			public readonly uint FunctionOpenid;
			public readonly uint IncreaseFavorabilityValue;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Name = ReadHelper.ReadUInt(binaryReader);
				CostPoint = ReadHelper.ReadUInt(binaryReader);
				DailyTotal = ReadHelper.ReadUInt(binaryReader);
				DailyTimes = ReadHelper.ReadUInt(binaryReader);
				FunctionOpenid = ReadHelper.ReadUInt(binaryReader);
				IncreaseFavorabilityValue = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFavorabilityBehavior.bytes";
		}

		private static CSVFavorabilityBehavior instance = null;			
		public static CSVFavorabilityBehavior Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFavorabilityBehavior 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFavorabilityBehavior forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFavorabilityBehavior();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFavorabilityBehavior");

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

    sealed public partial class CSVFavorabilityBehavior : FCSVFavorabilityBehavior
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFavorabilityBehavior.bytes";
		}

		private static CSVFavorabilityBehavior instance = null;			
		public static CSVFavorabilityBehavior Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFavorabilityBehavior 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFavorabilityBehavior forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFavorabilityBehavior();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFavorabilityBehavior");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}