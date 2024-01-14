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

	sealed public partial class CSVReturnRecover : Framework.Table.TableBase<CSVReturnRecover.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Activity_Group;
			public readonly uint type;
			public readonly List<uint> Reward_Group;
			public readonly List<uint> price;
			public readonly uint pack_name;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Activity_Group = ReadHelper.ReadUInt(binaryReader);
				type = ReadHelper.ReadUInt(binaryReader);
				Reward_Group = shareData.GetShareData<List<uint>>(binaryReader, 0);
				price = shareData.GetShareData<List<uint>>(binaryReader, 0);
				pack_name = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVReturnRecover.bytes";
		}

		private static CSVReturnRecover instance = null;			
		public static CSVReturnRecover Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVReturnRecover 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVReturnRecover forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVReturnRecover();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVReturnRecover");

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

    sealed public partial class CSVReturnRecover : FCSVReturnRecover
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVReturnRecover.bytes";
		}

		private static CSVReturnRecover instance = null;			
		public static CSVReturnRecover Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVReturnRecover 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVReturnRecover forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVReturnRecover();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVReturnRecover");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}