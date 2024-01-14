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

	sealed public partial class CSVShock : Framework.Table.TableBase<CSVShock.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint duration;
			public readonly uint strength_x;
			public readonly uint strength_y;
			public readonly uint strength_z;
			public readonly uint vibrato;
			public readonly uint randomness;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				duration = ReadHelper.ReadUInt(binaryReader);
				strength_x = ReadHelper.ReadUInt(binaryReader);
				strength_y = ReadHelper.ReadUInt(binaryReader);
				strength_z = ReadHelper.ReadUInt(binaryReader);
				vibrato = ReadHelper.ReadUInt(binaryReader);
				randomness = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVShock.bytes";
		}

		private static CSVShock instance = null;			
		public static CSVShock Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVShock 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVShock forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVShock();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVShock");

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

    sealed public partial class CSVShock : FCSVShock
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVShock.bytes";
		}

		private static CSVShock instance = null;			
		public static CSVShock Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVShock 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVShock forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVShock();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVShock");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}