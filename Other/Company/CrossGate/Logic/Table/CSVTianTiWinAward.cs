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

	sealed public partial class CSVTianTiWinAward : Framework.Table.TableBase<CSVTianTiWinAward.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint CourtId;
			public readonly uint Win;
			public readonly uint Fail;
			public readonly uint Teamid;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				CourtId = ReadHelper.ReadUInt(binaryReader);
				Win = ReadHelper.ReadUInt(binaryReader);
				Fail = ReadHelper.ReadUInt(binaryReader);
				Teamid = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTianTiWinAward.bytes";
		}

		private static CSVTianTiWinAward instance = null;			
		public static CSVTianTiWinAward Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTianTiWinAward 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTianTiWinAward forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTianTiWinAward();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTianTiWinAward");

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

    sealed public partial class CSVTianTiWinAward : FCSVTianTiWinAward
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTianTiWinAward.bytes";
		}

		private static CSVTianTiWinAward instance = null;			
		public static CSVTianTiWinAward Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTianTiWinAward 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTianTiWinAward forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTianTiWinAward();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTianTiWinAward");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}