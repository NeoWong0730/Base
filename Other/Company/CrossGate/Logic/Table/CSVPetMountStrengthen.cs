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

	sealed public partial class CSVPetMountStrengthen : Framework.Table.TableBase<CSVPetMountStrengthen.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint level;
			public readonly uint type;
			public readonly List<List<uint>> strengthen_cost;
			public readonly List<uint> advanced_cost;
			public readonly uint attribute_bonus;
			public readonly uint extra_skill_grid;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				level = ReadHelper.ReadUInt(binaryReader);
				type = ReadHelper.ReadUInt(binaryReader);
				strengthen_cost = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				advanced_cost = shareData.GetShareData<List<uint>>(binaryReader, 0);
				attribute_bonus = ReadHelper.ReadUInt(binaryReader);
				extra_skill_grid = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVPetMountStrengthen.bytes";
		}

		private static CSVPetMountStrengthen instance = null;			
		public static CSVPetMountStrengthen Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetMountStrengthen 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetMountStrengthen forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetMountStrengthen();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetMountStrengthen");

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

    sealed public partial class CSVPetMountStrengthen : FCSVPetMountStrengthen
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVPetMountStrengthen.bytes";
		}

		private static CSVPetMountStrengthen instance = null;			
		public static CSVPetMountStrengthen Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetMountStrengthen 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetMountStrengthen forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetMountStrengthen();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetMountStrengthen");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}