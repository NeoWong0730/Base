//
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

	sealed public partial class CSVCookLv : Framework.Table.TableBase<CSVCookLv.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint need_score;
			public readonly uint name;
			public readonly uint desc;
			public readonly List<uint> allow_tool;
			public bool free_cook { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public bool stage2_cook { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
			public bool multi_cook { get { return ReadHelper.GetBoolByIndex(boolArray0, 2); } }
			public bool batch_cook { get { return ReadHelper.GetBoolByIndex(boolArray0, 3); } }
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				need_score = ReadHelper.ReadUInt(binaryReader);
				name = ReadHelper.ReadUInt(binaryReader);
				desc = ReadHelper.ReadUInt(binaryReader);
				allow_tool = shareData.GetShareData<List<uint>>(binaryReader, 0);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVCookLv.bytes";
		}

		private static CSVCookLv instance = null;			
		public static CSVCookLv Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCookLv 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCookLv forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCookLv();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCookLv");

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

    sealed public partial class CSVCookLv : FCSVCookLv
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVCookLv.bytes";
		}

		private static CSVCookLv instance = null;			
		public static CSVCookLv Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCookLv 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCookLv forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCookLv();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCookLv");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}