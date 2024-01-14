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

	sealed public partial class CSVJewel : Framework.Table.TableBase<CSVJewel.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint jewel_type;
			public readonly uint level;
			public readonly uint num;
			public readonly List<List<uint>> attr;
			public readonly uint percent;
			public readonly uint next_id;
			public readonly uint score;
			public readonly List<uint> price;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				jewel_type = ReadHelper.ReadUInt(binaryReader);
				level = ReadHelper.ReadUInt(binaryReader);
				num = ReadHelper.ReadUInt(binaryReader);
				attr = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				percent = ReadHelper.ReadUInt(binaryReader);
				next_id = ReadHelper.ReadUInt(binaryReader);
				score = ReadHelper.ReadUInt(binaryReader);
				price = shareData.GetShareData<List<uint>>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVJewel.bytes";
		}

		private static CSVJewel instance = null;			
		public static CSVJewel Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVJewel 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVJewel forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVJewel();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVJewel");

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
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<uint>(binaryReader, 1, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVJewel : FCSVJewel
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVJewel.bytes";
		}

		private static CSVJewel instance = null;			
		public static CSVJewel Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVJewel 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVJewel forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVJewel();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVJewel");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}