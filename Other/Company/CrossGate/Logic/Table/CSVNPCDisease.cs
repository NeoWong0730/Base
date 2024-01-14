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

	sealed public partial class CSVNPCDisease : Framework.Table.TableBase<CSVNPCDisease.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Name;
			public readonly uint Des;
			public readonly uint Icon;
			public readonly uint ItemID1;
			public readonly uint Num1;
			public readonly uint ItemID2;
			public readonly uint Num2;
			public readonly uint ItemID3;
			public readonly uint Num3;
			public readonly uint ItemID4;
			public readonly uint Num4;
			public readonly uint FavorabilityValue;
			public readonly List<uint> IncreaseRange;
			public readonly List<uint> moodValue;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Name = ReadHelper.ReadUInt(binaryReader);
				Des = ReadHelper.ReadUInt(binaryReader);
				Icon = ReadHelper.ReadUInt(binaryReader);
				ItemID1 = ReadHelper.ReadUInt(binaryReader);
				Num1 = ReadHelper.ReadUInt(binaryReader);
				ItemID2 = ReadHelper.ReadUInt(binaryReader);
				Num2 = ReadHelper.ReadUInt(binaryReader);
				ItemID3 = ReadHelper.ReadUInt(binaryReader);
				Num3 = ReadHelper.ReadUInt(binaryReader);
				ItemID4 = ReadHelper.ReadUInt(binaryReader);
				Num4 = ReadHelper.ReadUInt(binaryReader);
				FavorabilityValue = ReadHelper.ReadUInt(binaryReader);
				IncreaseRange = shareData.GetShareData<List<uint>>(binaryReader, 0);
				moodValue = shareData.GetShareData<List<uint>>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVNPCDisease.bytes";
		}

		private static CSVNPCDisease instance = null;			
		public static CSVNPCDisease Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVNPCDisease 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVNPCDisease forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVNPCDisease();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVNPCDisease");

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

    sealed public partial class CSVNPCDisease : FCSVNPCDisease
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVNPCDisease.bytes";
		}

		private static CSVNPCDisease instance = null;			
		public static CSVNPCDisease Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVNPCDisease 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVNPCDisease forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVNPCDisease();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVNPCDisease");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}