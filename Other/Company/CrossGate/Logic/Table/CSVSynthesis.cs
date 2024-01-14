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

	sealed public partial class CSVSynthesis : Framework.Table.TableBase<CSVSynthesis.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly List<uint> Goal;
			public readonly uint Goaltips;
			public readonly List<uint> imageId;
			public readonly float time;
			public readonly uint tips;
			public readonly uint gameDescribe;
			public readonly List<uint> Reward_Time;
			public readonly List<uint> Reward_Id;
			public readonly List<uint> Time_Section;
			public readonly List<uint> Rank_Section;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Goal = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Goaltips = ReadHelper.ReadUInt(binaryReader);
				imageId = shareData.GetShareData<List<uint>>(binaryReader, 0);
				time = ReadHelper.ReadFloat(binaryReader);
				tips = ReadHelper.ReadUInt(binaryReader);
				gameDescribe = ReadHelper.ReadUInt(binaryReader);
				Reward_Time = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Reward_Id = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Time_Section = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Rank_Section = shareData.GetShareData<List<uint>>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVSynthesis.bytes";
		}

		private static CSVSynthesis instance = null;			
		public static CSVSynthesis Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSynthesis 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSynthesis forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSynthesis();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSynthesis");

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

    sealed public partial class CSVSynthesis : FCSVSynthesis
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVSynthesis.bytes";
		}

		private static CSVSynthesis instance = null;			
		public static CSVSynthesis Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSynthesis 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSynthesis forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSynthesis();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSynthesis");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}