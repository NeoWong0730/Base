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

	sealed public partial class CSVBOSSInformation : Framework.Table.TableBase<CSVBOSSInformation.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint BOSS_level;
			public readonly List<uint> limit_level;
			public readonly uint minimumTeamSize;
			public bool isFlip_X { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public bool isFlip_Y { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
			public readonly List<float> BOSS_position;
			public readonly uint location_description;
			public readonly uint AI_description;
			public readonly uint biographyKey_drop;
			public readonly uint challengeMode_id;
			public readonly uint playMode_id;
			public readonly uint bossManual_id;
			public readonly uint targetNPC;
			public readonly uint bossRankSubType;
			public bool bossIsRanking { get { return ReadHelper.GetBoolByIndex(boolArray0, 2); } }
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				BOSS_level = ReadHelper.ReadUInt(binaryReader);
				limit_level = shareData.GetShareData<List<uint>>(binaryReader, 0);
				minimumTeamSize = ReadHelper.ReadUInt(binaryReader);
				BOSS_position = shareData.GetShareData<List<float>>(binaryReader, 1);
				location_description = ReadHelper.ReadUInt(binaryReader);
				AI_description = ReadHelper.ReadUInt(binaryReader);
				biographyKey_drop = ReadHelper.ReadUInt(binaryReader);
				challengeMode_id = ReadHelper.ReadUInt(binaryReader);
				playMode_id = ReadHelper.ReadUInt(binaryReader);
				bossManual_id = ReadHelper.ReadUInt(binaryReader);
				targetNPC = ReadHelper.ReadUInt(binaryReader);
				bossRankSubType = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVBOSSInformation.bytes";
		}

		private static CSVBOSSInformation instance = null;			
		public static CSVBOSSInformation Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBOSSInformation 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBOSSInformation forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBOSSInformation();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBOSSInformation");

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
			shareData.ReadArrays<float>(binaryReader, 1, ReadHelper.ReadArray_ReadFloat);

			return shareData;
		}
	}

#else

    sealed public partial class CSVBOSSInformation : FCSVBOSSInformation
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVBOSSInformation.bytes";
		}

		private static CSVBOSSInformation instance = null;			
		public static CSVBOSSInformation Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBOSSInformation 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBOSSInformation forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBOSSInformation();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBOSSInformation");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}