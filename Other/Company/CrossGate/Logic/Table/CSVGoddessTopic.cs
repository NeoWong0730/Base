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

	sealed public partial class CSVGoddessTopic : Framework.Table.TableBase<CSVGoddessTopic.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint PlayType;
			public readonly uint topicLan;
			public readonly uint topicId;
			public readonly uint topicDifficulty;
			public readonly List<uint> InstanceId;
			public readonly List<uint> lanID;
			public readonly List<uint> iconId;
			public readonly string topicIcon;
			public readonly uint topicCondition;
			public readonly string ConditionNum;
			public readonly List<List<uint>> EndReward;
			public readonly List<uint> copyLevel;
			public readonly uint monsterLevel;
			public readonly List<List<uint>> monsterCharacter;
			public readonly List<List<uint>> aiCharacter;
			public readonly int modNum;
			public readonly List<List<uint>> rankReward;
			public readonly uint teamTarget;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				PlayType = ReadHelper.ReadUInt(binaryReader);
				topicLan = ReadHelper.ReadUInt(binaryReader);
				topicId = ReadHelper.ReadUInt(binaryReader);
				topicDifficulty = ReadHelper.ReadUInt(binaryReader);
				InstanceId = shareData.GetShareData<List<uint>>(binaryReader, 1);
				lanID = shareData.GetShareData<List<uint>>(binaryReader, 1);
				iconId = shareData.GetShareData<List<uint>>(binaryReader, 1);
				topicIcon = shareData.GetShareData<string>(binaryReader, 0);
				topicCondition = ReadHelper.ReadUInt(binaryReader);
				ConditionNum = shareData.GetShareData<string>(binaryReader, 0);
				EndReward = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				copyLevel = shareData.GetShareData<List<uint>>(binaryReader, 1);
				monsterLevel = ReadHelper.ReadUInt(binaryReader);
				monsterCharacter = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				aiCharacter = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				modNum = ReadHelper.ReadInt(binaryReader);
				rankReward = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				teamTarget = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVGoddessTopic.bytes";
		}

		private static CSVGoddessTopic instance = null;			
		public static CSVGoddessTopic Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVGoddessTopic 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVGoddessTopic forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVGoddessTopic();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVGoddessTopic");

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

    sealed public partial class CSVGoddessTopic : FCSVGoddessTopic
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVGoddessTopic.bytes";
		}

		private static CSVGoddessTopic instance = null;			
		public static CSVGoddessTopic Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVGoddessTopic 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVGoddessTopic forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVGoddessTopic();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVGoddessTopic");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}