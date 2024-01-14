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

	sealed public partial class CSVDeliver : Framework.Table.TableBase<CSVDeliver.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly int MapType;
			public readonly int MapId;
			public readonly List<int> MapPosition;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				MapType = ReadHelper.ReadInt(binaryReader);
				MapId = ReadHelper.ReadInt(binaryReader);
				MapPosition = shareData.GetShareData<List<int>>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVDeliver.bytes";
		}

		private static CSVDeliver instance = null;			
		public static CSVDeliver Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVDeliver 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVDeliver forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVDeliver();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVDeliver");

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
			shareData.ReadArrays<int>(binaryReader, 0, ReadHelper.ReadArray_ReadInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVDeliver : FCSVDeliver
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVDeliver.bytes";
		}

		private static CSVDeliver instance = null;			
		public static CSVDeliver Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVDeliver 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVDeliver forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVDeliver();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVDeliver");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}