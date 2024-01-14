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

	sealed public partial class CSVNien_Presonal_Rank : Framework.Table.TableBase<CSVNien_Presonal_Rank.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Activity_Id;
			public readonly uint Presonal_Rank;
			public readonly uint Reward;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Activity_Id = ReadHelper.ReadUInt(binaryReader);
				Presonal_Rank = ReadHelper.ReadUInt(binaryReader);
				Reward = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVNien_Presonal_Rank.bytes";
		}

		private static CSVNien_Presonal_Rank instance = null;			
		public static CSVNien_Presonal_Rank Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVNien_Presonal_Rank 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVNien_Presonal_Rank forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVNien_Presonal_Rank();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVNien_Presonal_Rank");

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

    sealed public partial class CSVNien_Presonal_Rank : FCSVNien_Presonal_Rank
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVNien_Presonal_Rank.bytes";
		}

		private static CSVNien_Presonal_Rank instance = null;			
		public static CSVNien_Presonal_Rank Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVNien_Presonal_Rank 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVNien_Presonal_Rank forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVNien_Presonal_Rank();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVNien_Presonal_Rank");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}