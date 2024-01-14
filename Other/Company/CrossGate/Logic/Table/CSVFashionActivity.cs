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

	sealed public partial class CSVFashionActivity : Framework.Table.TableBase<CSVFashionActivity.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint FashionCoin;
			public readonly uint Value;
			public bool Initial { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly uint Days;
			public readonly uint ActivityTime;
			public readonly uint EndTime;
			public readonly uint Reward;
			public readonly List<uint> Show;
			public readonly uint Name;
			public readonly uint Rules;
			public readonly List<uint> PtStore;
			public readonly List<uint> Tag;
			public readonly List<int> Pos;
			public bool Free { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				FashionCoin = ReadHelper.ReadUInt(binaryReader);
				Value = ReadHelper.ReadUInt(binaryReader);
				Days = ReadHelper.ReadUInt(binaryReader);
				ActivityTime = ReadHelper.ReadUInt(binaryReader);
				EndTime = ReadHelper.ReadUInt(binaryReader);
				Reward = ReadHelper.ReadUInt(binaryReader);
				Show = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Name = ReadHelper.ReadUInt(binaryReader);
				Rules = ReadHelper.ReadUInt(binaryReader);
				PtStore = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Tag = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Pos = shareData.GetShareData<List<int>>(binaryReader, 1);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFashionActivity.bytes";
		}

		private static CSVFashionActivity instance = null;			
		public static CSVFashionActivity Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFashionActivity 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFashionActivity forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFashionActivity();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFashionActivity");

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
			shareData.ReadArrays<int>(binaryReader, 1, ReadHelper.ReadArray_ReadInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVFashionActivity : FCSVFashionActivity
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFashionActivity.bytes";
		}

		private static CSVFashionActivity instance = null;			
		public static CSVFashionActivity Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFashionActivity 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFashionActivity forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFashionActivity();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFashionActivity");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}