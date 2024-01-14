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

	sealed public partial class CSVPKMatch : Framework.Table.TableBase<CSVPKMatch.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint startTime;
			public readonly uint endTime;
			public readonly uint priceType;
			public readonly uint price;
			public readonly uint matchName;
			public readonly List<List<uint>> level;
			public readonly List<uint> charScore;
			public readonly uint createTeam;
			public readonly uint checkTeam;
			public readonly uint pkStart;
			public readonly uint pkOver;
			public readonly string pkURL;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				startTime = ReadHelper.ReadUInt(binaryReader);
				endTime = ReadHelper.ReadUInt(binaryReader);
				priceType = ReadHelper.ReadUInt(binaryReader);
				price = ReadHelper.ReadUInt(binaryReader);
				matchName = ReadHelper.ReadUInt(binaryReader);
				level = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				charScore = shareData.GetShareData<List<uint>>(binaryReader, 1);
				createTeam = ReadHelper.ReadUInt(binaryReader);
				checkTeam = ReadHelper.ReadUInt(binaryReader);
				pkStart = ReadHelper.ReadUInt(binaryReader);
				pkOver = ReadHelper.ReadUInt(binaryReader);
				pkURL = shareData.GetShareData<string>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVPKMatch.bytes";
		}

		private static CSVPKMatch instance = null;			
		public static CSVPKMatch Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPKMatch 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPKMatch forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPKMatch();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPKMatch");

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
			TableShareData shareData = new TableShareData(3);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<uint>(binaryReader, 2, 1);

			return shareData;
		}
	}

#else

    sealed public partial class CSVPKMatch : FCSVPKMatch
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVPKMatch.bytes";
		}

		private static CSVPKMatch instance = null;			
		public static CSVPKMatch Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPKMatch 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPKMatch forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPKMatch();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPKMatch");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}