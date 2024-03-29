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

	sealed public partial class CSVGrowthFund : Framework.Table.TableBase<CSVGrowthFund.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Charge_Id;
			public readonly List<uint> level;
			public readonly List<uint> reward_Id;
			public readonly uint Title;
			public readonly List<uint> Fun_Des;
			public readonly string Show_Icon;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Charge_Id = ReadHelper.ReadUInt(binaryReader);
				level = shareData.GetShareData<List<uint>>(binaryReader, 1);
				reward_Id = shareData.GetShareData<List<uint>>(binaryReader, 1);
				Title = ReadHelper.ReadUInt(binaryReader);
				Fun_Des = shareData.GetShareData<List<uint>>(binaryReader, 1);
				Show_Icon = shareData.GetShareData<string>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVGrowthFund.bytes";
		}

		private static CSVGrowthFund instance = null;			
		public static CSVGrowthFund Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVGrowthFund 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVGrowthFund forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVGrowthFund();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVGrowthFund");

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
			TableShareData shareData = new TableShareData(2);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVGrowthFund : FCSVGrowthFund
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVGrowthFund.bytes";
		}

		private static CSVGrowthFund instance = null;			
		public static CSVGrowthFund Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVGrowthFund 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVGrowthFund forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVGrowthFund();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVGrowthFund");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}