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

	sealed public partial class CSVTalentExchange : Framework.Table.TableBase<CSVTalentExchange.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint exp;
			public readonly uint gold;
			public readonly uint time;
			public readonly List<uint> item;
			public readonly int level;
			public readonly int totalitem;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				exp = ReadHelper.ReadUInt(binaryReader);
				gold = ReadHelper.ReadUInt(binaryReader);
				time = ReadHelper.ReadUInt(binaryReader);
				item = shareData.GetShareData<List<uint>>(binaryReader, 0);
				level = ReadHelper.ReadInt(binaryReader);
				totalitem = ReadHelper.ReadInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTalentExchange.bytes";
		}

		private static CSVTalentExchange instance = null;			
		public static CSVTalentExchange Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTalentExchange 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTalentExchange forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTalentExchange();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTalentExchange");

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

    sealed public partial class CSVTalentExchange : FCSVTalentExchange
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTalentExchange.bytes";
		}

		private static CSVTalentExchange instance = null;			
		public static CSVTalentExchange Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTalentExchange 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTalentExchange forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTalentExchange();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTalentExchange");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}