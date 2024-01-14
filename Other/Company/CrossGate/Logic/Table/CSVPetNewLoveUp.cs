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

	sealed public partial class CSVPetNewLoveUp : Framework.Table.TableBase<CSVPetNewLoveUp.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint exp;
			public readonly byte level;
			public readonly List<List<uint>> OneselfEffec;
			public readonly uint RaceType;
			public readonly List<List<uint>> RaceEffec;
			public readonly uint OneselfScore;
			public readonly uint txt;
			public readonly uint contests;
			public readonly uint strengthen_lv;
			public readonly List<uint> BackgroundStory;
			public readonly List<uint> breakthrough_mission;
			public readonly List<List<uint>> IncreaseEffect;
			public readonly uint RaceScore;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				exp = ReadHelper.ReadUInt(binaryReader);
				level = ReadHelper.ReadByte(binaryReader);
				OneselfEffec = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				RaceType = ReadHelper.ReadUInt(binaryReader);
				RaceEffec = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				OneselfScore = ReadHelper.ReadUInt(binaryReader);
				txt = ReadHelper.ReadUInt(binaryReader);
				contests = ReadHelper.ReadUInt(binaryReader);
				strengthen_lv = ReadHelper.ReadUInt(binaryReader);
				BackgroundStory = shareData.GetShareData<List<uint>>(binaryReader, 0);
				breakthrough_mission = shareData.GetShareData<List<uint>>(binaryReader, 0);
				IncreaseEffect = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				RaceScore = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVPetNewLoveUp.bytes";
		}

		private static CSVPetNewLoveUp instance = null;			
		public static CSVPetNewLoveUp Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetNewLoveUp 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetNewLoveUp forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetNewLoveUp();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetNewLoveUp");

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

    sealed public partial class CSVPetNewLoveUp : FCSVPetNewLoveUp
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVPetNewLoveUp.bytes";
		}

		private static CSVPetNewLoveUp instance = null;			
		public static CSVPetNewLoveUp Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetNewLoveUp 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetNewLoveUp forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetNewLoveUp();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetNewLoveUp");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}