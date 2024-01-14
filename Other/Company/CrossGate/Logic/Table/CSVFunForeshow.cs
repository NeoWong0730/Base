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

	sealed public partial class CSVFunForeshow : Framework.Table.TableBase<CSVFunForeshow.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint words;
			public readonly uint FunctionLv;
			public readonly uint OpenForecastLan;
			public readonly uint SysIcon;
			public readonly uint SysForecast;
			public readonly List<string> Picture;
			public readonly uint Type;
			public readonly List<uint> JumpInterface;
			public readonly uint Priority;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				words = ReadHelper.ReadUInt(binaryReader);
				FunctionLv = ReadHelper.ReadUInt(binaryReader);
				OpenForecastLan = ReadHelper.ReadUInt(binaryReader);
				SysIcon = ReadHelper.ReadUInt(binaryReader);
				SysForecast = ReadHelper.ReadUInt(binaryReader);
				Picture = shareData.GetShareData<List<string>>(binaryReader, 1);
				Type = ReadHelper.ReadUInt(binaryReader);
				JumpInterface = shareData.GetShareData<List<uint>>(binaryReader, 2);
				Priority = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFunForeshow.bytes";
		}

		private static CSVFunForeshow instance = null;			
		public static CSVFunForeshow Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFunForeshow 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFunForeshow forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFunForeshow();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFunForeshow");

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
			TableShareData shareData = new TableShareData(3);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadStringArrays(binaryReader, 1, 0);
			shareData.ReadArrays<uint>(binaryReader, 2, ReadHelper.ReadArray_ReadUInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVFunForeshow : FCSVFunForeshow
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFunForeshow.bytes";
		}

		private static CSVFunForeshow instance = null;			
		public static CSVFunForeshow Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFunForeshow 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFunForeshow forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFunForeshow();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFunForeshow");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}