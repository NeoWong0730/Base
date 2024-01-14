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

	sealed public partial class CSVBless : Framework.Table.TableBase<CSVBless.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly string image;
			public readonly uint NameLan;
			public readonly uint CondLan;
			public readonly uint StoryLan;
			public readonly uint Reward;
			public readonly uint Times;
			public readonly uint npcID;
			public readonly uint BattleID;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				image = shareData.GetShareData<string>(binaryReader, 0);
				NameLan = ReadHelper.ReadUInt(binaryReader);
				CondLan = ReadHelper.ReadUInt(binaryReader);
				StoryLan = ReadHelper.ReadUInt(binaryReader);
				Reward = ReadHelper.ReadUInt(binaryReader);
				Times = ReadHelper.ReadUInt(binaryReader);
				npcID = ReadHelper.ReadUInt(binaryReader);
				BattleID = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVBless.bytes";
		}

		private static CSVBless instance = null;			
		public static CSVBless Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBless 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBless forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBless();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBless");

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
			shareData.ReadStrings(binaryReader, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVBless : FCSVBless
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVBless.bytes";
		}

		private static CSVBless instance = null;			
		public static CSVBless Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBless 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBless forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBless();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBless");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}