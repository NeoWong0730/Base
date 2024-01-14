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

	sealed public partial class CSVFavorabilityBanquet : Framework.Table.TableBase<CSVFavorabilityBanquet.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Name;
			public readonly uint Des;
			public readonly uint FavorabilityValue;
			public readonly uint IncreaseMoodValue;
			public readonly uint ItemID1;
			public readonly uint Num1;
			public readonly uint ItemID2;
			public readonly uint Num2;
			public readonly uint ItemID3;
			public readonly uint Num3;
			public readonly uint ItemID4;
			public readonly uint Num4;
			public readonly uint ItemID5;
			public readonly uint Num5;
			public readonly uint ItemID6;
			public readonly uint Num6;
			public readonly uint ItemID7;
			public readonly uint Num7;
			public readonly uint ItemID8;
			public readonly uint Num8;
			public readonly uint ItemID9;
			public readonly uint Num9;
			public readonly uint ItemID10;
			public readonly uint Num10;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Name = ReadHelper.ReadUInt(binaryReader);
				Des = ReadHelper.ReadUInt(binaryReader);
				FavorabilityValue = ReadHelper.ReadUInt(binaryReader);
				IncreaseMoodValue = ReadHelper.ReadUInt(binaryReader);
				ItemID1 = ReadHelper.ReadUInt(binaryReader);
				Num1 = ReadHelper.ReadUInt(binaryReader);
				ItemID2 = ReadHelper.ReadUInt(binaryReader);
				Num2 = ReadHelper.ReadUInt(binaryReader);
				ItemID3 = ReadHelper.ReadUInt(binaryReader);
				Num3 = ReadHelper.ReadUInt(binaryReader);
				ItemID4 = ReadHelper.ReadUInt(binaryReader);
				Num4 = ReadHelper.ReadUInt(binaryReader);
				ItemID5 = ReadHelper.ReadUInt(binaryReader);
				Num5 = ReadHelper.ReadUInt(binaryReader);
				ItemID6 = ReadHelper.ReadUInt(binaryReader);
				Num6 = ReadHelper.ReadUInt(binaryReader);
				ItemID7 = ReadHelper.ReadUInt(binaryReader);
				Num7 = ReadHelper.ReadUInt(binaryReader);
				ItemID8 = ReadHelper.ReadUInt(binaryReader);
				Num8 = ReadHelper.ReadUInt(binaryReader);
				ItemID9 = ReadHelper.ReadUInt(binaryReader);
				Num9 = ReadHelper.ReadUInt(binaryReader);
				ItemID10 = ReadHelper.ReadUInt(binaryReader);
				Num10 = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFavorabilityBanquet.bytes";
		}

		private static CSVFavorabilityBanquet instance = null;			
		public static CSVFavorabilityBanquet Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFavorabilityBanquet 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFavorabilityBanquet forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFavorabilityBanquet();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFavorabilityBanquet");

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

    sealed public partial class CSVFavorabilityBanquet : FCSVFavorabilityBanquet
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFavorabilityBanquet.bytes";
		}

		private static CSVFavorabilityBanquet instance = null;			
		public static CSVFavorabilityBanquet Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFavorabilityBanquet 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFavorabilityBanquet forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFavorabilityBanquet();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFavorabilityBanquet");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}