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

	sealed public partial class CSVActivityUiJump : Framework.Table.TableBase<CSVActivityUiJump.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint ActivityId;
			public readonly uint PreformId;
			public readonly uint UiId;
			public readonly uint UiParam;
			public readonly uint Begining_Date;
			public readonly uint Duration_Day;
			public readonly uint Tittle;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				ActivityId = ReadHelper.ReadUInt(binaryReader);
				PreformId = ReadHelper.ReadUInt(binaryReader);
				UiId = ReadHelper.ReadUInt(binaryReader);
				UiParam = ReadHelper.ReadUInt(binaryReader);
				Begining_Date = ReadHelper.ReadUInt(binaryReader);
				Duration_Day = ReadHelper.ReadUInt(binaryReader);
				Tittle = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVActivityUiJump.bytes";
		}

		private static CSVActivityUiJump instance = null;			
		public static CSVActivityUiJump Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVActivityUiJump 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVActivityUiJump forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVActivityUiJump();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVActivityUiJump");

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

    sealed public partial class CSVActivityUiJump : FCSVActivityUiJump
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVActivityUiJump.bytes";
		}

		private static CSVActivityUiJump instance = null;			
		public static CSVActivityUiJump Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVActivityUiJump 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVActivityUiJump forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVActivityUiJump();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVActivityUiJump");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}