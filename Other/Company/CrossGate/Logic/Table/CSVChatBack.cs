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

	sealed public partial class CSVChatBack : Framework.Table.TableBase<CSVChatBack.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint BackName;
			public readonly uint BackDescribe;
			public readonly uint Lock;
			public readonly uint LimitedTime;
			public readonly uint BackGetFor;
			public readonly List<uint> BackParamFor;
			public readonly uint BackGetLimit;
			public readonly List<uint> BackParamLimit;
			public readonly string BackIcon;
			public readonly uint Unlocktips;
			public readonly List<string> SubPackageShow;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				BackName = ReadHelper.ReadUInt(binaryReader);
				BackDescribe = ReadHelper.ReadUInt(binaryReader);
				Lock = ReadHelper.ReadUInt(binaryReader);
				LimitedTime = ReadHelper.ReadUInt(binaryReader);
				BackGetFor = ReadHelper.ReadUInt(binaryReader);
				BackParamFor = shareData.GetShareData<List<uint>>(binaryReader, 1);
				BackGetLimit = ReadHelper.ReadUInt(binaryReader);
				BackParamLimit = shareData.GetShareData<List<uint>>(binaryReader, 1);
				BackIcon = shareData.GetShareData<string>(binaryReader, 0);
				Unlocktips = ReadHelper.ReadUInt(binaryReader);
				SubPackageShow = shareData.GetShareData<List<string>>(binaryReader, 2);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVChatBack.bytes";
		}

		private static CSVChatBack instance = null;			
		public static CSVChatBack Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVChatBack 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVChatBack forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVChatBack();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVChatBack");

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

    sealed public partial class CSVChatBack : FCSVChatBack
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVChatBack.bytes";
		}

		private static CSVChatBack instance = null;			
		public static CSVChatBack Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVChatBack 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVChatBack forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVChatBack();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVChatBack");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}