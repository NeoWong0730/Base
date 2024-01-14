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

	sealed public partial class CSVAward : Framework.Table.TableBase<CSVAward.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint itemId;
			public readonly string name_id;
			public readonly uint iconId;
			public readonly uint itemNum;
			public bool isExclusive { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly uint number;
			public readonly uint quality;
			public readonly uint isBroadCast;
			public readonly string backgroundType;
			public readonly string backgroundlightType;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				itemId = ReadHelper.ReadUInt(binaryReader);
				name_id = shareData.GetShareData<string>(binaryReader, 0);
				iconId = ReadHelper.ReadUInt(binaryReader);
				itemNum = ReadHelper.ReadUInt(binaryReader);
				number = ReadHelper.ReadUInt(binaryReader);
				quality = ReadHelper.ReadUInt(binaryReader);
				isBroadCast = ReadHelper.ReadUInt(binaryReader);
				backgroundType = shareData.GetShareData<string>(binaryReader, 0);
				backgroundlightType = shareData.GetShareData<string>(binaryReader, 0);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVAward.bytes";
		}

		private static CSVAward instance = null;			
		public static CSVAward Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAward 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAward forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAward();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAward");

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
			shareData.ReadStrings(binaryReader, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVAward : FCSVAward
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVAward.bytes";
		}

		private static CSVAward instance = null;			
		public static CSVAward Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAward 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAward forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAward();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAward");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}