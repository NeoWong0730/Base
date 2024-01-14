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

	sealed public partial class CSVReturnArdent : Framework.Table.TableBase<CSVReturnArdent.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint text;
			public readonly uint param;
			public readonly uint enthusiasmPoint;
			public readonly uint enthusiasmLimit;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				text = ReadHelper.ReadUInt(binaryReader);
				param = ReadHelper.ReadUInt(binaryReader);
				enthusiasmPoint = ReadHelper.ReadUInt(binaryReader);
				enthusiasmLimit = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVReturnArdent.bytes";
		}

		private static CSVReturnArdent instance = null;			
		public static CSVReturnArdent Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVReturnArdent 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVReturnArdent forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVReturnArdent();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVReturnArdent");

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

    sealed public partial class CSVReturnArdent : FCSVReturnArdent
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVReturnArdent.bytes";
		}

		private static CSVReturnArdent instance = null;			
		public static CSVReturnArdent Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVReturnArdent 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVReturnArdent forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVReturnArdent();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVReturnArdent");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}