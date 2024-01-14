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

	sealed public partial class CSVChatWord : Framework.Table.TableBase<CSVChatWord.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint WordName;
			public readonly uint WordDescribe;
			public readonly uint Lock;
			public readonly uint LimitedTime;
			public readonly uint WordGetFor;
			public readonly List<uint> WordParamFor;
			public readonly uint WordGetLimit;
			public readonly List<uint> WordParamLimit;
			public readonly uint WordIcon;
			public readonly uint Word;
			public readonly uint Unlocktips;
			public readonly List<string> SubPackageShow;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				WordName = ReadHelper.ReadUInt(binaryReader);
				WordDescribe = ReadHelper.ReadUInt(binaryReader);
				Lock = ReadHelper.ReadUInt(binaryReader);
				LimitedTime = ReadHelper.ReadUInt(binaryReader);
				WordGetFor = ReadHelper.ReadUInt(binaryReader);
				WordParamFor = shareData.GetShareData<List<uint>>(binaryReader, 1);
				WordGetLimit = ReadHelper.ReadUInt(binaryReader);
				WordParamLimit = shareData.GetShareData<List<uint>>(binaryReader, 1);
				WordIcon = ReadHelper.ReadUInt(binaryReader);
				Word = ReadHelper.ReadUInt(binaryReader);
				Unlocktips = ReadHelper.ReadUInt(binaryReader);
				SubPackageShow = shareData.GetShareData<List<string>>(binaryReader, 2);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVChatWord.bytes";
		}

		private static CSVChatWord instance = null;			
		public static CSVChatWord Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVChatWord 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVChatWord forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVChatWord();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVChatWord");

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
			shareData.ReadStringArrays(binaryReader, 2, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVChatWord : FCSVChatWord
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVChatWord.bytes";
		}

		private static CSVChatWord instance = null;			
		public static CSVChatWord Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVChatWord 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVChatWord forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVChatWord();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVChatWord");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}