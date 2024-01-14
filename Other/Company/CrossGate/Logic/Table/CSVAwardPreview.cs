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

	sealed public partial class CSVAwardPreview : Framework.Table.TableBase<CSVAwardPreview.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly List<uint> awardId;
			public readonly uint pool;
			public readonly uint probability;
			public readonly uint quality;
			public readonly uint number;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				awardId = shareData.GetShareData<List<uint>>(binaryReader, 0);
				pool = ReadHelper.ReadUInt(binaryReader);
				probability = ReadHelper.ReadUInt(binaryReader);
				quality = ReadHelper.ReadUInt(binaryReader);
				number = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVAwardPreview.bytes";
		}

		private static CSVAwardPreview instance = null;			
		public static CSVAwardPreview Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAwardPreview 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAwardPreview forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAwardPreview();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAwardPreview");

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

    sealed public partial class CSVAwardPreview : FCSVAwardPreview
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVAwardPreview.bytes";
		}

		private static CSVAwardPreview instance = null;			
		public static CSVAwardPreview Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAwardPreview 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAwardPreview forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAwardPreview();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAwardPreview");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}