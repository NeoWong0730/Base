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

	sealed public partial class CSVexpRetrieve : Framework.Table.TableBase<CSVexpRetrieve.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint ActiveName;
			public readonly uint Activity_id;
			public readonly uint Type;
			public readonly uint Exp;
			public readonly uint Exp_Ratio;
			public readonly uint Time;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				ActiveName = ReadHelper.ReadUInt(binaryReader);
				Activity_id = ReadHelper.ReadUInt(binaryReader);
				Type = ReadHelper.ReadUInt(binaryReader);
				Exp = ReadHelper.ReadUInt(binaryReader);
				Exp_Ratio = ReadHelper.ReadUInt(binaryReader);
				Time = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVexpRetrieve.bytes";
		}

		private static CSVexpRetrieve instance = null;			
		public static CSVexpRetrieve Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVexpRetrieve 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVexpRetrieve forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVexpRetrieve();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVexpRetrieve");

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

    sealed public partial class CSVexpRetrieve : FCSVexpRetrieve
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVexpRetrieve.bytes";
		}

		private static CSVexpRetrieve instance = null;			
		public static CSVexpRetrieve Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVexpRetrieve 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVexpRetrieve forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVexpRetrieve();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVexpRetrieve");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}