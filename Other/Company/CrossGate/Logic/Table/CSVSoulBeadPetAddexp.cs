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

	sealed public partial class CSVSoulBeadPetAddexp : Framework.Table.TableBase<CSVSoulBeadPetAddexp.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint base_exp;
			public readonly List<List<uint>> multiplier;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				base_exp = ReadHelper.ReadUInt(binaryReader);
				multiplier = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVSoulBeadPetAddexp.bytes";
		}

		private static CSVSoulBeadPetAddexp instance = null;			
		public static CSVSoulBeadPetAddexp Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSoulBeadPetAddexp 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSoulBeadPetAddexp forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSoulBeadPetAddexp();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSoulBeadPetAddexp");

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

    sealed public partial class CSVSoulBeadPetAddexp : FCSVSoulBeadPetAddexp
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVSoulBeadPetAddexp.bytes";
		}

		private static CSVSoulBeadPetAddexp instance = null;			
		public static CSVSoulBeadPetAddexp Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSoulBeadPetAddexp 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSoulBeadPetAddexp forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSoulBeadPetAddexp();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSoulBeadPetAddexp");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}