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

	sealed public partial class CSVDialogue : Framework.Table.TableBase<CSVDialogue.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint whetherSkipDialogue;
			public readonly uint whetherAutomaticDialogue;
			public readonly uint whetherShowSymbol;
			public readonly uint dialoguePerform;
			public readonly uint DubbingId;
			public readonly List<List<uint>> dialogueContent;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				whetherSkipDialogue = ReadHelper.ReadUInt(binaryReader);
				whetherAutomaticDialogue = ReadHelper.ReadUInt(binaryReader);
				whetherShowSymbol = ReadHelper.ReadUInt(binaryReader);
				dialoguePerform = ReadHelper.ReadUInt(binaryReader);
				DubbingId = ReadHelper.ReadUInt(binaryReader);
				dialogueContent = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVDialogue.bytes";
		}

		private static CSVDialogue instance = null;			
		public static CSVDialogue Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVDialogue 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVDialogue forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVDialogue();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVDialogue");

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

    sealed public partial class CSVDialogue : FCSVDialogue
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVDialogue.bytes";
		}

		private static CSVDialogue instance = null;			
		public static CSVDialogue Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVDialogue 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVDialogue forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVDialogue();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVDialogue");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}