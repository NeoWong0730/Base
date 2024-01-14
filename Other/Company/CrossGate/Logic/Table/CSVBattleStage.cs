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

	sealed public partial class CSVBattleStage : Framework.Table.TableBase<CSVBattleStage.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint enemy_group;
			public readonly uint battle_stage;
			public readonly uint battle_scene;
			public readonly uint position_type;
			public readonly uint new_role;
			public readonly List<uint> skill_id;
			public readonly uint refresh_work_id;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				enemy_group = ReadHelper.ReadUInt(binaryReader);
				battle_stage = ReadHelper.ReadUInt(binaryReader);
				battle_scene = ReadHelper.ReadUInt(binaryReader);
				position_type = ReadHelper.ReadUInt(binaryReader);
				new_role = ReadHelper.ReadUInt(binaryReader);
				skill_id = shareData.GetShareData<List<uint>>(binaryReader, 0);
				refresh_work_id = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVBattleStage.bytes";
		}

		private static CSVBattleStage instance = null;			
		public static CSVBattleStage Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBattleStage 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBattleStage forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBattleStage();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBattleStage");

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
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVBattleStage : FCSVBattleStage
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVBattleStage.bytes";
		}

		private static CSVBattleStage instance = null;			
		public static CSVBattleStage Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBattleStage 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBattleStage forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBattleStage();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBattleStage");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}