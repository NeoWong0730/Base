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

	sealed public partial class CSVTemporary : Framework.Table.TableBase<CSVTemporary.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly List<uint> equip_type;
			public readonly uint attr_id;
			public readonly uint low;
			public readonly uint up;
			public readonly uint time;
			public readonly uint type;
			public readonly uint lev;
			public readonly uint equip_lev;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				equip_type = shareData.GetShareData<List<uint>>(binaryReader, 0);
				attr_id = ReadHelper.ReadUInt(binaryReader);
				low = ReadHelper.ReadUInt(binaryReader);
				up = ReadHelper.ReadUInt(binaryReader);
				time = ReadHelper.ReadUInt(binaryReader);
				type = ReadHelper.ReadUInt(binaryReader);
				lev = ReadHelper.ReadUInt(binaryReader);
				equip_lev = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTemporary.bytes";
		}

		private static CSVTemporary instance = null;			
		public static CSVTemporary Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTemporary 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTemporary forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTemporary();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTemporary");

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

    sealed public partial class CSVTemporary : FCSVTemporary
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTemporary.bytes";
		}

		private static CSVTemporary instance = null;			
		public static CSVTemporary Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTemporary 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTemporary forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTemporary();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTemporary");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}