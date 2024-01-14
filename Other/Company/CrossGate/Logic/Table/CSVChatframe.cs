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

	sealed public partial class CSVChatframe : Framework.Table.TableBase<CSVChatframe.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint ChatName;
			public readonly uint ChatDescribe;
			public readonly uint Lock;
			public readonly uint LimitedTime;
			public readonly uint ChatGetFor;
			public readonly List<uint> ChatParamFor;
			public readonly uint ChatGetLimit;
			public readonly List<uint> ChatParamLimit;
			public readonly uint ChatIcon;
			public readonly uint Word;
			public readonly uint Text;
			public readonly uint Unlocktips;
			public readonly List<string> SubPackageShow;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				ChatName = ReadHelper.ReadUInt(binaryReader);
				ChatDescribe = ReadHelper.ReadUInt(binaryReader);
				Lock = ReadHelper.ReadUInt(binaryReader);
				LimitedTime = ReadHelper.ReadUInt(binaryReader);
				ChatGetFor = ReadHelper.ReadUInt(binaryReader);
				ChatParamFor = shareData.GetShareData<List<uint>>(binaryReader, 1);
				ChatGetLimit = ReadHelper.ReadUInt(binaryReader);
				ChatParamLimit = shareData.GetShareData<List<uint>>(binaryReader, 1);
				ChatIcon = ReadHelper.ReadUInt(binaryReader);
				Word = ReadHelper.ReadUInt(binaryReader);
				Text = ReadHelper.ReadUInt(binaryReader);
				Unlocktips = ReadHelper.ReadUInt(binaryReader);
				SubPackageShow = shareData.GetShareData<List<string>>(binaryReader, 2);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVChatframe.bytes";
		}

		private static CSVChatframe instance = null;			
		public static CSVChatframe Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVChatframe 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVChatframe forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVChatframe();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVChatframe");

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

    sealed public partial class CSVChatframe : FCSVChatframe
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVChatframe.bytes";
		}

		private static CSVChatframe instance = null;			
		public static CSVChatframe Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVChatframe 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVChatframe forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVChatframe();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVChatframe");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}