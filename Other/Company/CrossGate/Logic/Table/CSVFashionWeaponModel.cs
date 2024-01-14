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

	sealed public partial class CSVFashionWeaponModel : Framework.Table.TableBase<CSVFashionWeaponModel.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint equip_id;
			public readonly string model_show;
			public readonly List<string> additional_show_model;
			public readonly string model;
			public readonly List<string> additional_model;
			public readonly string equip_pos;
			public readonly List<string> additional_equip_pos;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				equip_id = ReadHelper.ReadUInt(binaryReader);
				model_show = shareData.GetShareData<string>(binaryReader, 0);
				additional_show_model = shareData.GetShareData<List<string>>(binaryReader, 1);
				model = shareData.GetShareData<string>(binaryReader, 0);
				additional_model = shareData.GetShareData<List<string>>(binaryReader, 1);
				equip_pos = shareData.GetShareData<string>(binaryReader, 0);
				additional_equip_pos = shareData.GetShareData<List<string>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFashionWeaponModel.bytes";
		}

		private static CSVFashionWeaponModel instance = null;			
		public static CSVFashionWeaponModel Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFashionWeaponModel 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFashionWeaponModel forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFashionWeaponModel();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFashionWeaponModel");

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
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadStringArrays(binaryReader, 1, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVFashionWeaponModel : FCSVFashionWeaponModel
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFashionWeaponModel.bytes";
		}

		private static CSVFashionWeaponModel instance = null;			
		public static CSVFashionWeaponModel Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFashionWeaponModel 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFashionWeaponModel forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFashionWeaponModel();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFashionWeaponModel");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}