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

	sealed public partial class CSVHead : Framework.Table.TableBase<CSVHead.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint HeadName;
			public readonly uint HeadDescribe;
			public readonly uint Lock;
			public readonly uint LimitedTime;
			public readonly uint HeadGetFor;
			public readonly List<uint> HeadParamFor;
			public readonly uint HeadGetLimit;
			public readonly List<uint> HeadParamLimit;
			public readonly List<uint> HeadIcon;
			public readonly uint Unlocktips;
			public readonly List<string> SubPackageShow;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				HeadName = ReadHelper.ReadUInt(binaryReader);
				HeadDescribe = ReadHelper.ReadUInt(binaryReader);
				Lock = ReadHelper.ReadUInt(binaryReader);
				LimitedTime = ReadHelper.ReadUInt(binaryReader);
				HeadGetFor = ReadHelper.ReadUInt(binaryReader);
				HeadParamFor = shareData.GetShareData<List<uint>>(binaryReader, 1);
				HeadGetLimit = ReadHelper.ReadUInt(binaryReader);
				HeadParamLimit = shareData.GetShareData<List<uint>>(binaryReader, 1);
				HeadIcon = shareData.GetShareData<List<uint>>(binaryReader, 1);
				Unlocktips = ReadHelper.ReadUInt(binaryReader);
				SubPackageShow = shareData.GetShareData<List<string>>(binaryReader, 2);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVHead.bytes";
		}

		private static CSVHead instance = null;			
		public static CSVHead Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVHead 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVHead forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVHead();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVHead");

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

    sealed public partial class CSVHead : FCSVHead
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVHead.bytes";
		}

		private static CSVHead instance = null;			
		public static CSVHead Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVHead 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVHead forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVHead();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVHead");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}