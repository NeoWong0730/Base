//
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

	sealed public partial class CSVRanklistmain : Framework.Table.TableBase<CSVRanklistmain.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint RankType;
			public readonly uint Subtype;
			public readonly uint Name;
			public readonly uint ObjectType;
			public bool Group { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public bool GlobalRank { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
			public bool Canset { get { return ReadHelper.GetBoolByIndex(boolArray0, 2); } }
			public readonly uint Descshowtype;
			public readonly List<uint> Tittle;
			public bool percentage { get { return ReadHelper.GetBoolByIndex(boolArray0, 3); } }
			public readonly uint Owndata;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				RankType = ReadHelper.ReadUInt(binaryReader);
				Subtype = ReadHelper.ReadUInt(binaryReader);
				Name = ReadHelper.ReadUInt(binaryReader);
				ObjectType = ReadHelper.ReadUInt(binaryReader);
				Descshowtype = ReadHelper.ReadUInt(binaryReader);
				Tittle = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Owndata = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVRanklistmain.bytes";
		}

		private static CSVRanklistmain instance = null;			
		public static CSVRanklistmain Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVRanklistmain 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVRanklistmain forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVRanklistmain();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVRanklistmain");

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

    sealed public partial class CSVRanklistmain : FCSVRanklistmain
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVRanklistmain.bytes";
		}

		private static CSVRanklistmain instance = null;			
		public static CSVRanklistmain Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVRanklistmain 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVRanklistmain forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVRanklistmain();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVRanklistmain");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}