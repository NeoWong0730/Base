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

	sealed public partial class CSVDailyActivityPush : Framework.Table.TableBase<CSVDailyActivityPush.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint ActiveId;
			public readonly List<uint> PrompMethod;
			public readonly uint Cycle;
			public readonly uint CycleShow;
			public readonly uint TimeShow;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				ActiveId = ReadHelper.ReadUInt(binaryReader);
				PrompMethod = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Cycle = ReadHelper.ReadUInt(binaryReader);
				CycleShow = ReadHelper.ReadUInt(binaryReader);
				TimeShow = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVDailyActivityPush.bytes";
		}

		private static CSVDailyActivityPush instance = null;			
		public static CSVDailyActivityPush Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVDailyActivityPush 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVDailyActivityPush forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVDailyActivityPush();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVDailyActivityPush");

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

    sealed public partial class CSVDailyActivityPush : FCSVDailyActivityPush
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVDailyActivityPush.bytes";
		}

		private static CSVDailyActivityPush instance = null;			
		public static CSVDailyActivityPush Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVDailyActivityPush 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVDailyActivityPush forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVDailyActivityPush();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVDailyActivityPush");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}