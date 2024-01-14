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

	sealed public partial class CSVSlotUpgrade : Framework.Table.TableBase<CSVSlotUpgrade.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint slot_id;
			public readonly uint lev;
			public readonly uint exp;
			public readonly uint exp_add;
			public readonly List<List<uint>> attr;
			public readonly uint score;
			public readonly List<uint> materia_lev;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				slot_id = ReadHelper.ReadUInt(binaryReader);
				lev = ReadHelper.ReadUInt(binaryReader);
				exp = ReadHelper.ReadUInt(binaryReader);
				exp_add = ReadHelper.ReadUInt(binaryReader);
				attr = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				score = ReadHelper.ReadUInt(binaryReader);
				materia_lev = shareData.GetShareData<List<uint>>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVSlotUpgrade.bytes";
		}

		private static CSVSlotUpgrade instance = null;			
		public static CSVSlotUpgrade Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSlotUpgrade 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSlotUpgrade forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSlotUpgrade();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSlotUpgrade");

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

    sealed public partial class CSVSlotUpgrade : FCSVSlotUpgrade
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVSlotUpgrade.bytes";
		}

		private static CSVSlotUpgrade instance = null;			
		public static CSVSlotUpgrade Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSlotUpgrade 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSlotUpgrade forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSlotUpgrade();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSlotUpgrade");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}