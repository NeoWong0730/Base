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

	sealed public partial class CSVAchievement : Framework.Table.TableBase<CSVAchievement.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Achievement_Title;
			public readonly uint MainClass;
			public readonly uint SubClass;
			public readonly uint SubClassType;
			public readonly uint Order;
			public readonly uint TypeAchievement;
			public readonly List<uint> ReachTypeAchievement;
			public readonly uint Daliy_Limit;
			public readonly uint Task_Test;
			public readonly uint Trigger_Type;
			public readonly uint Rare;
			public readonly uint Point;
			public readonly uint Icon_Id;
			public readonly uint Drop_Id;
			public readonly uint Is_New;
			public readonly uint Show_Type;
			public readonly uint AnnouncementId;
			public readonly List<uint> CollectInfo;
			public readonly uint OpenLimiet;
			public readonly uint DateLimiet;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Achievement_Title = ReadHelper.ReadUInt(binaryReader);
				MainClass = ReadHelper.ReadUInt(binaryReader);
				SubClass = ReadHelper.ReadUInt(binaryReader);
				SubClassType = ReadHelper.ReadUInt(binaryReader);
				Order = ReadHelper.ReadUInt(binaryReader);
				TypeAchievement = ReadHelper.ReadUInt(binaryReader);
				ReachTypeAchievement = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Daliy_Limit = ReadHelper.ReadUInt(binaryReader);
				Task_Test = ReadHelper.ReadUInt(binaryReader);
				Trigger_Type = ReadHelper.ReadUInt(binaryReader);
				Rare = ReadHelper.ReadUInt(binaryReader);
				Point = ReadHelper.ReadUInt(binaryReader);
				Icon_Id = ReadHelper.ReadUInt(binaryReader);
				Drop_Id = ReadHelper.ReadUInt(binaryReader);
				Is_New = ReadHelper.ReadUInt(binaryReader);
				Show_Type = ReadHelper.ReadUInt(binaryReader);
				AnnouncementId = ReadHelper.ReadUInt(binaryReader);
				CollectInfo = shareData.GetShareData<List<uint>>(binaryReader, 0);
				OpenLimiet = ReadHelper.ReadUInt(binaryReader);
				DateLimiet = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVAchievement.bytes";
		}

		private static CSVAchievement instance = null;			
		public static CSVAchievement Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAchievement 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAchievement forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAchievement();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAchievement");

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

    sealed public partial class CSVAchievement : FCSVAchievement
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVAchievement.bytes";
		}

		private static CSVAchievement instance = null;			
		public static CSVAchievement Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAchievement 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAchievement forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAchievement();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAchievement");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}