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

	sealed public partial class CSVFamilyAuctionAct : Framework.Table.TableBase<CSVFamilyAuctionAct.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint subordinatelangid;
			public readonly uint Superior;
			public readonly uint Superiorlangid;
			public readonly uint SuperiorIcom;
			public readonly uint Sort;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				subordinatelangid = ReadHelper.ReadUInt(binaryReader);
				Superior = ReadHelper.ReadUInt(binaryReader);
				Superiorlangid = ReadHelper.ReadUInt(binaryReader);
				SuperiorIcom = ReadHelper.ReadUInt(binaryReader);
				Sort = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyAuctionAct.bytes";
		}

		private static CSVFamilyAuctionAct instance = null;			
		public static CSVFamilyAuctionAct Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyAuctionAct 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyAuctionAct forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyAuctionAct();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyAuctionAct");

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
			TableShareData shareData = null;

			return shareData;
		}
	}

#else

    sealed public partial class CSVFamilyAuctionAct : FCSVFamilyAuctionAct
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyAuctionAct.bytes";
		}

		private static CSVFamilyAuctionAct instance = null;			
		public static CSVFamilyAuctionAct Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyAuctionAct 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyAuctionAct forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyAuctionAct();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyAuctionAct");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}