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

	sealed public partial class CSVOrnamentsUpgrade : Framework.Table.TableBase<CSVOrnamentsUpgrade.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint type;
			public readonly uint lv;
			public readonly uint equipment_level;
			public readonly uint nextlevelid;
			public readonly List<uint> upgrade_cost_equip;
			public readonly List<List<uint>> upgrade_cost_item;
			public readonly uint upgrade_rate;
			public readonly List<List<uint>> decompose_item;
			public readonly List<List<uint>> reforge_cost;
			public readonly List<List<uint>> base_attr;
			public readonly uint extra_attr_num;
			public readonly uint extra_attr_add;
			public readonly uint senior_max_num;
			public readonly uint senior_rate;
			public readonly uint senior_group;
			public readonly uint senior_skill_pro;
			public readonly uint junior_group;
			public readonly uint skill_pro;
			public readonly uint score;
			public readonly List<uint> score_sec;
			public readonly List<List<uint>> lock_reforge_cost;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				type = ReadHelper.ReadUInt(binaryReader);
				lv = ReadHelper.ReadUInt(binaryReader);
				equipment_level = ReadHelper.ReadUInt(binaryReader);
				nextlevelid = ReadHelper.ReadUInt(binaryReader);
				upgrade_cost_equip = shareData.GetShareData<List<uint>>(binaryReader, 0);
				upgrade_cost_item = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				upgrade_rate = ReadHelper.ReadUInt(binaryReader);
				decompose_item = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				reforge_cost = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				base_attr = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				extra_attr_num = ReadHelper.ReadUInt(binaryReader);
				extra_attr_add = ReadHelper.ReadUInt(binaryReader);
				senior_max_num = ReadHelper.ReadUInt(binaryReader);
				senior_rate = ReadHelper.ReadUInt(binaryReader);
				senior_group = ReadHelper.ReadUInt(binaryReader);
				senior_skill_pro = ReadHelper.ReadUInt(binaryReader);
				junior_group = ReadHelper.ReadUInt(binaryReader);
				skill_pro = ReadHelper.ReadUInt(binaryReader);
				score = ReadHelper.ReadUInt(binaryReader);
				score_sec = shareData.GetShareData<List<uint>>(binaryReader, 0);
				lock_reforge_cost = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVOrnamentsUpgrade.bytes";
		}

		private static CSVOrnamentsUpgrade instance = null;			
		public static CSVOrnamentsUpgrade Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVOrnamentsUpgrade 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVOrnamentsUpgrade forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVOrnamentsUpgrade();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVOrnamentsUpgrade");

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

    sealed public partial class CSVOrnamentsUpgrade : FCSVOrnamentsUpgrade
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVOrnamentsUpgrade.bytes";
		}

		private static CSVOrnamentsUpgrade instance = null;			
		public static CSVOrnamentsUpgrade Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVOrnamentsUpgrade 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVOrnamentsUpgrade forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVOrnamentsUpgrade();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVOrnamentsUpgrade");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}