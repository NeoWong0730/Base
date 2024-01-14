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

	sealed public partial class CSVActiveSkillShow : Framework.Table.TableBase<CSVActiveSkillShow.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly List<List<uint>> enemy_position;
			public readonly List<List<uint>> friend_position;
			public readonly List<uint> skill_combat_id;
			public readonly List<uint> interval;
			public readonly uint repeat_interval;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				enemy_position = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				friend_position = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				skill_combat_id = shareData.GetShareData<List<uint>>(binaryReader, 0);
				interval = shareData.GetShareData<List<uint>>(binaryReader, 0);
				repeat_interval = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVActiveSkillShow.bytes";
		}

		private static CSVActiveSkillShow instance = null;			
		public static CSVActiveSkillShow Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVActiveSkillShow 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVActiveSkillShow forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVActiveSkillShow();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVActiveSkillShow");

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

    sealed public partial class CSVActiveSkillShow : FCSVActiveSkillShow
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVActiveSkillShow.bytes";
		}

		private static CSVActiveSkillShow instance = null;			
		public static CSVActiveSkillShow Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVActiveSkillShow 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVActiveSkillShow forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVActiveSkillShow();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVActiveSkillShow");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}