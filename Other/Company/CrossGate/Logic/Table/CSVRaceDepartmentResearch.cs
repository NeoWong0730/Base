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

	sealed public partial class CSVRaceDepartmentResearch : Framework.Table.TableBase<CSVRaceDepartmentResearch.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint nextlevelid;
			public readonly uint type;
			public readonly uint rank;
			public readonly uint level;
			public readonly List<List<uint>> upgrade_cost;
			public readonly uint upgrade_restrict;
			public readonly List<List<uint>> up_attr1;
			public readonly uint unlock_skill;
			public readonly List<List<uint>> up_rank_cost;
			public readonly uint score;
			public readonly List<List<uint>> restitution;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				nextlevelid = ReadHelper.ReadUInt(binaryReader);
				type = ReadHelper.ReadUInt(binaryReader);
				rank = ReadHelper.ReadUInt(binaryReader);
				level = ReadHelper.ReadUInt(binaryReader);
				upgrade_cost = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				upgrade_restrict = ReadHelper.ReadUInt(binaryReader);
				up_attr1 = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				unlock_skill = ReadHelper.ReadUInt(binaryReader);
				up_rank_cost = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				score = ReadHelper.ReadUInt(binaryReader);
				restitution = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVRaceDepartmentResearch.bytes";
		}

		private static CSVRaceDepartmentResearch instance = null;			
		public static CSVRaceDepartmentResearch Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVRaceDepartmentResearch 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVRaceDepartmentResearch forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVRaceDepartmentResearch();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVRaceDepartmentResearch");

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

    sealed public partial class CSVRaceDepartmentResearch : FCSVRaceDepartmentResearch
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVRaceDepartmentResearch.bytes";
		}

		private static CSVRaceDepartmentResearch instance = null;			
		public static CSVRaceDepartmentResearch Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVRaceDepartmentResearch 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVRaceDepartmentResearch forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVRaceDepartmentResearch();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVRaceDepartmentResearch");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}