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

	sealed public partial class CSVReturnChanllengeIntegral : Framework.Table.TableBase<CSVReturnChanllengeIntegral.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Activity_Group;
			public readonly uint Requiredpoints;
			public readonly uint Dropid;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Activity_Group = ReadHelper.ReadUInt(binaryReader);
				Requiredpoints = ReadHelper.ReadUInt(binaryReader);
				Dropid = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVReturnChanllengeIntegral.bytes";
		}

		private static CSVReturnChanllengeIntegral instance = null;			
		public static CSVReturnChanllengeIntegral Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVReturnChanllengeIntegral 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVReturnChanllengeIntegral forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVReturnChanllengeIntegral();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVReturnChanllengeIntegral");

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

    sealed public partial class CSVReturnChanllengeIntegral : FCSVReturnChanllengeIntegral
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVReturnChanllengeIntegral.bytes";
		}

		private static CSVReturnChanllengeIntegral instance = null;			
		public static CSVReturnChanllengeIntegral Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVReturnChanllengeIntegral 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVReturnChanllengeIntegral forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVReturnChanllengeIntegral();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVReturnChanllengeIntegral");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}