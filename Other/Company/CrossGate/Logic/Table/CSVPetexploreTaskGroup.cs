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

	sealed public partial class CSVPetexploreTaskGroup : Framework.Table.TableBase<CSVPetexploreTaskGroup.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Title;
			public readonly uint Task_Des;
			public readonly uint Difficulty;
			public readonly List<uint> Chieftain;
			public readonly uint Times;
			public readonly uint Level_Condition;
			public readonly uint Score_Condition;
			public readonly List<List<uint>> Attr_Condition;
			public readonly uint Point;
			public readonly uint Race_Condition;
			public readonly uint Success_Reward;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Title = ReadHelper.ReadUInt(binaryReader);
				Task_Des = ReadHelper.ReadUInt(binaryReader);
				Difficulty = ReadHelper.ReadUInt(binaryReader);
				Chieftain = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Times = ReadHelper.ReadUInt(binaryReader);
				Level_Condition = ReadHelper.ReadUInt(binaryReader);
				Score_Condition = ReadHelper.ReadUInt(binaryReader);
				Attr_Condition = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				Point = ReadHelper.ReadUInt(binaryReader);
				Race_Condition = ReadHelper.ReadUInt(binaryReader);
				Success_Reward = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVPetexploreTaskGroup.bytes";
		}

		private static CSVPetexploreTaskGroup instance = null;			
		public static CSVPetexploreTaskGroup Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetexploreTaskGroup 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetexploreTaskGroup forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetexploreTaskGroup();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetexploreTaskGroup");

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

    sealed public partial class CSVPetexploreTaskGroup : FCSVPetexploreTaskGroup
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVPetexploreTaskGroup.bytes";
		}

		private static CSVPetexploreTaskGroup instance = null;			
		public static CSVPetexploreTaskGroup Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetexploreTaskGroup 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetexploreTaskGroup forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetexploreTaskGroup();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetexploreTaskGroup");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}