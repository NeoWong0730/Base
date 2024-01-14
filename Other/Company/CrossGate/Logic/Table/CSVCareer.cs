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

	sealed public partial class CSVCareer : Framework.Table.TableBase<CSVCareer.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint name;
			public readonly List<uint> desc;
			public readonly uint icon;
			public readonly uint select_icon;
			public readonly uint team_icon;
			public readonly uint logo_icon;
			public readonly uint pvp_icon;
			public readonly string profession_icon1;
			public readonly string profession_icon2;
			public readonly uint open;
			public readonly uint career;
			public readonly string model;
			public readonly uint weapon;
			public readonly uint test_battleid;
			public readonly List<uint> inti_skill;
			public readonly List<List<uint>> battle_skill;
			public readonly List<uint> passive_skill_show;
			public readonly List<uint> proud_initial;
			public readonly List<uint> currency_initial;
			public readonly List<uint> recommend_skill;
			public readonly List<List<uint>> proud_skill;
			public readonly List<List<uint>> currency_skill;
			public readonly List<List<uint>> learnable_skill;
			public readonly uint recommend;
			public readonly List<List<uint>> add_point;
			public readonly List<uint> Capability_value;
			public readonly uint job_task_ID;
			public readonly uint job_lan_ID;
			public readonly List<uint> TheChargeItem;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				name = ReadHelper.ReadUInt(binaryReader);
				desc = shareData.GetShareData<List<uint>>(binaryReader, 1);
				icon = ReadHelper.ReadUInt(binaryReader);
				select_icon = ReadHelper.ReadUInt(binaryReader);
				team_icon = ReadHelper.ReadUInt(binaryReader);
				logo_icon = ReadHelper.ReadUInt(binaryReader);
				pvp_icon = ReadHelper.ReadUInt(binaryReader);
				profession_icon1 = shareData.GetShareData<string>(binaryReader, 0);
				profession_icon2 = shareData.GetShareData<string>(binaryReader, 0);
				open = ReadHelper.ReadUInt(binaryReader);
				career = ReadHelper.ReadUInt(binaryReader);
				model = shareData.GetShareData<string>(binaryReader, 0);
				weapon = ReadHelper.ReadUInt(binaryReader);
				test_battleid = ReadHelper.ReadUInt(binaryReader);
				inti_skill = shareData.GetShareData<List<uint>>(binaryReader, 1);
				battle_skill = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				passive_skill_show = shareData.GetShareData<List<uint>>(binaryReader, 1);
				proud_initial = shareData.GetShareData<List<uint>>(binaryReader, 1);
				currency_initial = shareData.GetShareData<List<uint>>(binaryReader, 1);
				recommend_skill = shareData.GetShareData<List<uint>>(binaryReader, 1);
				proud_skill = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				currency_skill = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				learnable_skill = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				recommend = ReadHelper.ReadUInt(binaryReader);
				add_point = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				Capability_value = shareData.GetShareData<List<uint>>(binaryReader, 1);
				job_task_ID = ReadHelper.ReadUInt(binaryReader);
				job_lan_ID = ReadHelper.ReadUInt(binaryReader);
				TheChargeItem = shareData.GetShareData<List<uint>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVCareer.bytes";
		}

		private static CSVCareer instance = null;			
		public static CSVCareer Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCareer 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCareer forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCareer();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCareer");

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
			TableShareData shareData = new TableShareData(3);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<uint>(binaryReader, 2, 1);

			return shareData;
		}
	}

#else

    sealed public partial class CSVCareer : FCSVCareer
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVCareer.bytes";
		}

		private static CSVCareer instance = null;			
		public static CSVCareer Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCareer 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCareer forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCareer();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCareer");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}