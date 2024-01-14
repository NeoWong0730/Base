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

	sealed public partial class CSVCharacterWeapon : Framework.Table.TableBase<CSVCharacterWeapon.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly string create_char_timeline1;
			public readonly string mode_weapon1;
			public readonly string create_char_timeline2;
			public readonly string mode_weapon2;
			public readonly string create_char_timeline3;
			public readonly string mode_weapon3;
			public readonly string create_char_timeline4;
			public readonly string mode_weapon4;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				create_char_timeline1 = shareData.GetShareData<string>(binaryReader, 0);
				mode_weapon1 = shareData.GetShareData<string>(binaryReader, 0);
				create_char_timeline2 = shareData.GetShareData<string>(binaryReader, 0);
				mode_weapon2 = shareData.GetShareData<string>(binaryReader, 0);
				create_char_timeline3 = shareData.GetShareData<string>(binaryReader, 0);
				mode_weapon3 = shareData.GetShareData<string>(binaryReader, 0);
				create_char_timeline4 = shareData.GetShareData<string>(binaryReader, 0);
				mode_weapon4 = shareData.GetShareData<string>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVCharacterWeapon.bytes";
		}

		private static CSVCharacterWeapon instance = null;			
		public static CSVCharacterWeapon Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCharacterWeapon 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCharacterWeapon forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCharacterWeapon();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCharacterWeapon");

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
			shareData.ReadStrings(binaryReader, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVCharacterWeapon : FCSVCharacterWeapon
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVCharacterWeapon.bytes";
		}

		private static CSVCharacterWeapon instance = null;			
		public static CSVCharacterWeapon Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCharacterWeapon 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCharacterWeapon forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCharacterWeapon();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCharacterWeapon");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}