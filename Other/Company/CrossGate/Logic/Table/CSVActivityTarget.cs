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

	sealed public partial class CSVActivityTarget : Framework.Table.TableBase<CSVActivityTarget.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Activityid;
			public readonly uint RankType;
			public readonly uint Priority;
			public readonly uint TypeAchievement;
			public readonly List<uint> ReachTypeAchievement;
			public readonly uint Titleid;
			public readonly uint Langid;
			public readonly uint Tel_type;
			public readonly List<uint> Skip_Id;
			public readonly uint ActivityType;
			public readonly uint Dropid;
			public readonly uint Points;
			public readonly uint Functionid;
			public readonly uint FamilyIn;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Activityid = ReadHelper.ReadUInt(binaryReader);
				RankType = ReadHelper.ReadUInt(binaryReader);
				Priority = ReadHelper.ReadUInt(binaryReader);
				TypeAchievement = ReadHelper.ReadUInt(binaryReader);
				ReachTypeAchievement = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Titleid = ReadHelper.ReadUInt(binaryReader);
				Langid = ReadHelper.ReadUInt(binaryReader);
				Tel_type = ReadHelper.ReadUInt(binaryReader);
				Skip_Id = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ActivityType = ReadHelper.ReadUInt(binaryReader);
				Dropid = ReadHelper.ReadUInt(binaryReader);
				Points = ReadHelper.ReadUInt(binaryReader);
				Functionid = ReadHelper.ReadUInt(binaryReader);
				FamilyIn = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVActivityTarget.bytes";
		}

		private static CSVActivityTarget instance = null;			
		public static CSVActivityTarget Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVActivityTarget 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVActivityTarget forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVActivityTarget();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVActivityTarget");

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

    sealed public partial class CSVActivityTarget : FCSVActivityTarget
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVActivityTarget.bytes";
		}

		private static CSVActivityTarget instance = null;			
		public static CSVActivityTarget Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVActivityTarget 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVActivityTarget forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVActivityTarget();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVActivityTarget");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}