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

	sealed public partial class CSVClueDetectiveLevel : Framework.Table.TableBase<CSVClueDetectiveLevel.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint UpgradeExp;
			public readonly uint Name;
			public readonly uint Badge;
			public readonly uint Reward;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				UpgradeExp = ReadHelper.ReadUInt(binaryReader);
				Name = ReadHelper.ReadUInt(binaryReader);
				Badge = ReadHelper.ReadUInt(binaryReader);
				Reward = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVClueDetectiveLevel.bytes";
		}

		private static CSVClueDetectiveLevel instance = null;			
		public static CSVClueDetectiveLevel Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVClueDetectiveLevel 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVClueDetectiveLevel forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVClueDetectiveLevel();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVClueDetectiveLevel");

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

    sealed public partial class CSVClueDetectiveLevel : FCSVClueDetectiveLevel
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVClueDetectiveLevel.bytes";
		}

		private static CSVClueDetectiveLevel instance = null;			
		public static CSVClueDetectiveLevel Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVClueDetectiveLevel 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVClueDetectiveLevel forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVClueDetectiveLevel();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVClueDetectiveLevel");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}