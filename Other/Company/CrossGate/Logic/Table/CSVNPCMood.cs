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

	sealed public partial class CSVNPCMood : Framework.Table.TableBase<CSVNPCMood.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Name;
			public readonly uint Des;
			public readonly uint Icon;
			public readonly List<uint> Range;
			public readonly uint FavorabilityRatio;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Name = ReadHelper.ReadUInt(binaryReader);
				Des = ReadHelper.ReadUInt(binaryReader);
				Icon = ReadHelper.ReadUInt(binaryReader);
				Range = shareData.GetShareData<List<uint>>(binaryReader, 0);
				FavorabilityRatio = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVNPCMood.bytes";
		}

		private static CSVNPCMood instance = null;			
		public static CSVNPCMood Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVNPCMood 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVNPCMood forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVNPCMood();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVNPCMood");

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

    sealed public partial class CSVNPCMood : FCSVNPCMood
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVNPCMood.bytes";
		}

		private static CSVNPCMood instance = null;			
		public static CSVNPCMood Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVNPCMood 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVNPCMood forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVNPCMood();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVNPCMood");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}