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

	sealed public partial class CSVSoulBead : Framework.Table.TableBase<CSVSoulBead.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint type;
			public readonly uint icon;
			public readonly uint level;
			public readonly uint score;
			public readonly uint exp;
			public readonly List<List<uint>> item_cost;
			public readonly List<uint> add_exp;
			public readonly uint pet_type;
			public readonly uint pet_score;
			public readonly List<uint> base_skill_group;
			public readonly List<uint> special_skill_group;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				type = ReadHelper.ReadUInt(binaryReader);
				icon = ReadHelper.ReadUInt(binaryReader);
				level = ReadHelper.ReadUInt(binaryReader);
				score = ReadHelper.ReadUInt(binaryReader);
				exp = ReadHelper.ReadUInt(binaryReader);
				item_cost = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				add_exp = shareData.GetShareData<List<uint>>(binaryReader, 0);
				pet_type = ReadHelper.ReadUInt(binaryReader);
				pet_score = ReadHelper.ReadUInt(binaryReader);
				base_skill_group = shareData.GetShareData<List<uint>>(binaryReader, 0);
				special_skill_group = shareData.GetShareData<List<uint>>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVSoulBead.bytes";
		}

		private static CSVSoulBead instance = null;			
		public static CSVSoulBead Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSoulBead 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSoulBead forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSoulBead();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSoulBead");

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

    sealed public partial class CSVSoulBead : FCSVSoulBead
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVSoulBead.bytes";
		}

		private static CSVSoulBead instance = null;			
		public static CSVSoulBead Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSoulBead 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSoulBead forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSoulBead();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSoulBead");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}