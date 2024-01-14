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

	sealed public partial class CSVFavorabilityGift : Framework.Table.TableBase<CSVFavorabilityGift.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Gift_Type;
			public readonly uint Stack;
			public readonly uint Num;
			public readonly uint IncreaseFavorabilityValue;
			public readonly uint FavouriteFavorabilityValue;
			public readonly uint IncreaseMoodValue;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Gift_Type = ReadHelper.ReadUInt(binaryReader);
				Stack = ReadHelper.ReadUInt(binaryReader);
				Num = ReadHelper.ReadUInt(binaryReader);
				IncreaseFavorabilityValue = ReadHelper.ReadUInt(binaryReader);
				FavouriteFavorabilityValue = ReadHelper.ReadUInt(binaryReader);
				IncreaseMoodValue = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFavorabilityGift.bytes";
		}

		private static CSVFavorabilityGift instance = null;			
		public static CSVFavorabilityGift Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFavorabilityGift 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFavorabilityGift forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFavorabilityGift();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFavorabilityGift");

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

    sealed public partial class CSVFavorabilityGift : FCSVFavorabilityGift
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFavorabilityGift.bytes";
		}

		private static CSVFavorabilityGift instance = null;			
		public static CSVFavorabilityGift Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFavorabilityGift 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFavorabilityGift forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFavorabilityGift();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFavorabilityGift");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}