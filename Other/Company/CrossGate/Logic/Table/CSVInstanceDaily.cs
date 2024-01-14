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

	sealed public partial class CSVInstanceDaily : Framework.Table.TableBase<CSVInstanceDaily.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint InstanceId;
			public readonly uint LayerStage;
			public readonly uint Layerlevel;
			public readonly uint Award;
			public readonly uint RandomAward;
			public readonly uint Name;
			public readonly uint Describe;
			public readonly uint icon;
			public readonly byte ChangeUI;
			public readonly uint LevelLimited;
			public readonly uint Awakeningid;
			public readonly uint Score;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				InstanceId = ReadHelper.ReadUInt(binaryReader);
				LayerStage = ReadHelper.ReadUInt(binaryReader);
				Layerlevel = ReadHelper.ReadUInt(binaryReader);
				Award = ReadHelper.ReadUInt(binaryReader);
				RandomAward = ReadHelper.ReadUInt(binaryReader);
				Name = ReadHelper.ReadUInt(binaryReader);
				Describe = ReadHelper.ReadUInt(binaryReader);
				icon = ReadHelper.ReadUInt(binaryReader);
				ChangeUI = ReadHelper.ReadByte(binaryReader);
				LevelLimited = ReadHelper.ReadUInt(binaryReader);
				Awakeningid = ReadHelper.ReadUInt(binaryReader);
				Score = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVInstanceDaily.bytes";
		}

		private static CSVInstanceDaily instance = null;			
		public static CSVInstanceDaily Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVInstanceDaily 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVInstanceDaily forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVInstanceDaily();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVInstanceDaily");

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
			TableShareData shareData = null;

			return shareData;
		}
	}

#else

    sealed public partial class CSVInstanceDaily : FCSVInstanceDaily
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVInstanceDaily.bytes";
		}

		private static CSVInstanceDaily instance = null;			
		public static CSVInstanceDaily Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVInstanceDaily 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVInstanceDaily forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVInstanceDaily();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVInstanceDaily");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}