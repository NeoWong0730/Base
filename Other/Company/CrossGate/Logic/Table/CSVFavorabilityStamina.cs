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

	sealed public partial class CSVFavorabilityStamina : Framework.Table.TableBase<CSVFavorabilityStamina.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint FavorabilityStamina;
			public readonly uint MaxFavorabilityStamina;
			public readonly uint Time;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				FavorabilityStamina = ReadHelper.ReadUInt(binaryReader);
				MaxFavorabilityStamina = ReadHelper.ReadUInt(binaryReader);
				Time = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFavorabilityStamina.bytes";
		}

		private static CSVFavorabilityStamina instance = null;			
		public static CSVFavorabilityStamina Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFavorabilityStamina 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFavorabilityStamina forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFavorabilityStamina();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFavorabilityStamina");

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

    sealed public partial class CSVFavorabilityStamina : FCSVFavorabilityStamina
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFavorabilityStamina.bytes";
		}

		private static CSVFavorabilityStamina instance = null;			
		public static CSVFavorabilityStamina Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFavorabilityStamina 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFavorabilityStamina forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFavorabilityStamina();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFavorabilityStamina");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}