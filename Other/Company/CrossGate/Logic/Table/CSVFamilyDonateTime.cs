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

	sealed public partial class CSVFamilyDonateTime : Framework.Table.TableBase<CSVFamilyDonateTime.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint GoldSingleCost;
			public readonly uint DiamondsSingleCost;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				GoldSingleCost = ReadHelper.ReadUInt(binaryReader);
				DiamondsSingleCost = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyDonateTime.bytes";
		}

		private static CSVFamilyDonateTime instance = null;			
		public static CSVFamilyDonateTime Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyDonateTime 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyDonateTime forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyDonateTime();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyDonateTime");

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

    sealed public partial class CSVFamilyDonateTime : FCSVFamilyDonateTime
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyDonateTime.bytes";
		}

		private static CSVFamilyDonateTime instance = null;			
		public static CSVFamilyDonateTime Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyDonateTime 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyDonateTime forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyDonateTime();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyDonateTime");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}