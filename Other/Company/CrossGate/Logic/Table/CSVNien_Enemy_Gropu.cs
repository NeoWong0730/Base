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

	sealed public partial class CSVNien_Enemy_Gropu : Framework.Table.TableBase<CSVNien_Enemy_Gropu.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Activity_Id;
			public readonly List<uint> Enemy_GropuID;
			public readonly uint Reward;
			public readonly uint Sessions;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Activity_Id = ReadHelper.ReadUInt(binaryReader);
				Enemy_GropuID = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Reward = ReadHelper.ReadUInt(binaryReader);
				Sessions = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVNien_Enemy_Gropu.bytes";
		}

		private static CSVNien_Enemy_Gropu instance = null;			
		public static CSVNien_Enemy_Gropu Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVNien_Enemy_Gropu 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVNien_Enemy_Gropu forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVNien_Enemy_Gropu();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVNien_Enemy_Gropu");

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

    sealed public partial class CSVNien_Enemy_Gropu : FCSVNien_Enemy_Gropu
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVNien_Enemy_Gropu.bytes";
		}

		private static CSVNien_Enemy_Gropu instance = null;			
		public static CSVNien_Enemy_Gropu Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVNien_Enemy_Gropu 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVNien_Enemy_Gropu forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVNien_Enemy_Gropu();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVNien_Enemy_Gropu");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}