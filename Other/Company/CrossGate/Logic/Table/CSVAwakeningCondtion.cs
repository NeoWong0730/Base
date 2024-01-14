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

	sealed public partial class CSVAwakeningCondtion : Framework.Table.TableBase<CSVAwakeningCondtion.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint descripvive_lag_Id;
			public readonly uint Condition_lag_Id;
			public readonly uint pop_up_box;
			public readonly uint tel_type;
			public readonly List<List<uint>> skip_Id;
			public readonly List<uint> function_Id;
			public readonly uint not_open;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				descripvive_lag_Id = ReadHelper.ReadUInt(binaryReader);
				Condition_lag_Id = ReadHelper.ReadUInt(binaryReader);
				pop_up_box = ReadHelper.ReadUInt(binaryReader);
				tel_type = ReadHelper.ReadUInt(binaryReader);
				skip_Id = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				function_Id = shareData.GetShareData<List<uint>>(binaryReader, 0);
				not_open = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVAwakeningCondtion.bytes";
		}

		private static CSVAwakeningCondtion instance = null;			
		public static CSVAwakeningCondtion Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAwakeningCondtion 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAwakeningCondtion forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAwakeningCondtion();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAwakeningCondtion");

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
			TableShareData shareData = new TableShareData(2);
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<uint>(binaryReader, 1, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVAwakeningCondtion : FCSVAwakeningCondtion
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVAwakeningCondtion.bytes";
		}

		private static CSVAwakeningCondtion instance = null;			
		public static CSVAwakeningCondtion Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAwakeningCondtion 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAwakeningCondtion forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAwakeningCondtion();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAwakeningCondtion");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}