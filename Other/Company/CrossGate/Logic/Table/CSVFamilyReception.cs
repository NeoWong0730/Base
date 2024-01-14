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

	sealed public partial class CSVFamilyReception : Framework.Table.TableBase<CSVFamilyReception.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint familyCastleLv;
			public readonly uint receptionStar;
			public readonly uint receptionValue;
			public readonly uint level1Food;
			public readonly uint level2Food;
			public readonly uint level3Food;
			public readonly uint foodTotal;
			public readonly uint level1monster;
			public readonly uint level2monster;
			public readonly uint level3monster;
			public readonly uint monsterTotal;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				familyCastleLv = ReadHelper.ReadUInt(binaryReader);
				receptionStar = ReadHelper.ReadUInt(binaryReader);
				receptionValue = ReadHelper.ReadUInt(binaryReader);
				level1Food = ReadHelper.ReadUInt(binaryReader);
				level2Food = ReadHelper.ReadUInt(binaryReader);
				level3Food = ReadHelper.ReadUInt(binaryReader);
				foodTotal = ReadHelper.ReadUInt(binaryReader);
				level1monster = ReadHelper.ReadUInt(binaryReader);
				level2monster = ReadHelper.ReadUInt(binaryReader);
				level3monster = ReadHelper.ReadUInt(binaryReader);
				monsterTotal = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyReception.bytes";
		}

		private static CSVFamilyReception instance = null;			
		public static CSVFamilyReception Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyReception 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyReception forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyReception();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyReception");

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

    sealed public partial class CSVFamilyReception : FCSVFamilyReception
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyReception.bytes";
		}

		private static CSVFamilyReception instance = null;			
		public static CSVFamilyReception Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyReception 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyReception forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyReception();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyReception");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}