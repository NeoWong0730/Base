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

	sealed public partial class CSVPromptBox : Framework.Table.TableBase<CSVPromptBox.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Time;
			public bool Close { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public bool Jump { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
			public readonly uint UIId;
			public readonly uint Icon;
			public readonly uint Title;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Time = ReadHelper.ReadUInt(binaryReader);
				UIId = ReadHelper.ReadUInt(binaryReader);
				Icon = ReadHelper.ReadUInt(binaryReader);
				Title = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVPromptBox.bytes";
		}

		private static CSVPromptBox instance = null;			
		public static CSVPromptBox Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPromptBox 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPromptBox forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPromptBox();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPromptBox");

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

    sealed public partial class CSVPromptBox : FCSVPromptBox
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVPromptBox.bytes";
		}

		private static CSVPromptBox instance = null;			
		public static CSVPromptBox Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPromptBox 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPromptBox forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPromptBox();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPromptBox");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}