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

	sealed public partial class CSVCheckseq : Framework.Table.TableBase<CSVCheckseq.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly List<List<int>> CheckCondi1;
			public readonly List<List<int>> CheckCondi2;
			public readonly List<List<int>> CheckCondi3;
			public readonly List<List<int>> CheckCondi4;
			public readonly List<List<int>> CheckCondi5;
			public readonly List<List<int>> CheckCondi6;
			public readonly List<List<int>> CheckCondi7;
			public readonly List<List<int>> CheckCondi8;
			public readonly List<List<int>> CheckCondi9;
			public readonly List<List<int>> CheckCondi10;
			public readonly uint CondiNoPasarTips;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				CheckCondi1 = shareData.GetShareData<List<List<int>>>(binaryReader, 1);
				CheckCondi2 = shareData.GetShareData<List<List<int>>>(binaryReader, 1);
				CheckCondi3 = shareData.GetShareData<List<List<int>>>(binaryReader, 1);
				CheckCondi4 = shareData.GetShareData<List<List<int>>>(binaryReader, 1);
				CheckCondi5 = shareData.GetShareData<List<List<int>>>(binaryReader, 1);
				CheckCondi6 = shareData.GetShareData<List<List<int>>>(binaryReader, 1);
				CheckCondi7 = shareData.GetShareData<List<List<int>>>(binaryReader, 1);
				CheckCondi8 = shareData.GetShareData<List<List<int>>>(binaryReader, 1);
				CheckCondi9 = shareData.GetShareData<List<List<int>>>(binaryReader, 1);
				CheckCondi10 = shareData.GetShareData<List<List<int>>>(binaryReader, 1);
				CondiNoPasarTips = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVCheckseq.bytes";
		}

		private static CSVCheckseq instance = null;			
		public static CSVCheckseq Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCheckseq 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCheckseq forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCheckseq();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCheckseq");

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
			shareData.ReadArrays<int>(binaryReader, 0, ReadHelper.ReadArray_ReadInt);
			shareData.ReadArray2s<int>(binaryReader, 1, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVCheckseq : FCSVCheckseq
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVCheckseq.bytes";
		}

		private static CSVCheckseq instance = null;			
		public static CSVCheckseq Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCheckseq 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCheckseq forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCheckseq();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCheckseq");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}