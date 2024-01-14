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

	sealed public partial class CSVActivityQuestGroup : Framework.Table.TableBase<CSVActivityQuestGroup.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Activity_Id;
			public readonly uint Quest_Info;
			public readonly uint QuestType;
			public readonly uint TypeAchievement;
			public readonly List<uint> ReachTypeAchievement;
			public readonly uint Tel_type;
			public readonly List<uint> Skip_Id;
			public readonly uint Function_Id;
			public readonly uint FamilyIn;
			public readonly uint Drop_Id;
			public readonly uint Order;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Activity_Id = ReadHelper.ReadUInt(binaryReader);
				Quest_Info = ReadHelper.ReadUInt(binaryReader);
				QuestType = ReadHelper.ReadUInt(binaryReader);
				TypeAchievement = ReadHelper.ReadUInt(binaryReader);
				ReachTypeAchievement = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Tel_type = ReadHelper.ReadUInt(binaryReader);
				Skip_Id = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Function_Id = ReadHelper.ReadUInt(binaryReader);
				FamilyIn = ReadHelper.ReadUInt(binaryReader);
				Drop_Id = ReadHelper.ReadUInt(binaryReader);
				Order = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVActivityQuestGroup.bytes";
		}

		private static CSVActivityQuestGroup instance = null;			
		public static CSVActivityQuestGroup Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVActivityQuestGroup 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVActivityQuestGroup forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVActivityQuestGroup();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVActivityQuestGroup");

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

    sealed public partial class CSVActivityQuestGroup : FCSVActivityQuestGroup
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVActivityQuestGroup.bytes";
		}

		private static CSVActivityQuestGroup instance = null;			
		public static CSVActivityQuestGroup Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVActivityQuestGroup 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVActivityQuestGroup forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVActivityQuestGroup();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVActivityQuestGroup");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}