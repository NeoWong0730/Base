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

	sealed public partial class CSVSwitchCondition : Framework.Table.TableBase<CSVSwitchCondition.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint stageName;
			public readonly uint switchWay;
			public readonly uint stageDescription;
			public readonly string stagePrefabe;
			public bool isShowStage { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				stageName = ReadHelper.ReadUInt(binaryReader);
				switchWay = ReadHelper.ReadUInt(binaryReader);
				stageDescription = ReadHelper.ReadUInt(binaryReader);
				stagePrefabe = shareData.GetShareData<string>(binaryReader, 0);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVSwitchCondition.bytes";
		}

		private static CSVSwitchCondition instance = null;			
		public static CSVSwitchCondition Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSwitchCondition 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSwitchCondition forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSwitchCondition();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSwitchCondition");

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

    sealed public partial class CSVSwitchCondition : FCSVSwitchCondition
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVSwitchCondition.bytes";
		}

		private static CSVSwitchCondition instance = null;			
		public static CSVSwitchCondition Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSwitchCondition 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSwitchCondition forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSwitchCondition();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSwitchCondition");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}