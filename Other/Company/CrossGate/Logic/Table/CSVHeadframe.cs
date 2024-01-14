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

	sealed public partial class CSVHeadframe : Framework.Table.TableBase<CSVHeadframe.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint HeadframeName;
			public readonly uint HeadframeDescribe;
			public readonly uint Lock;
			public readonly uint LimitedTime;
			public readonly uint HeadframeGetFor;
			public readonly List<uint> HeadframeParamFor;
			public readonly uint HeadframeGetLimit;
			public readonly List<uint> HeadframeParamLimit;
			public readonly uint HeadframeIcon;
			public readonly uint Unlocktips;
			public readonly List<string> SubPackageShow;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				HeadframeName = ReadHelper.ReadUInt(binaryReader);
				HeadframeDescribe = ReadHelper.ReadUInt(binaryReader);
				Lock = ReadHelper.ReadUInt(binaryReader);
				LimitedTime = ReadHelper.ReadUInt(binaryReader);
				HeadframeGetFor = ReadHelper.ReadUInt(binaryReader);
				HeadframeParamFor = shareData.GetShareData<List<uint>>(binaryReader, 1);
				HeadframeGetLimit = ReadHelper.ReadUInt(binaryReader);
				HeadframeParamLimit = shareData.GetShareData<List<uint>>(binaryReader, 1);
				HeadframeIcon = ReadHelper.ReadUInt(binaryReader);
				Unlocktips = ReadHelper.ReadUInt(binaryReader);
				SubPackageShow = shareData.GetShareData<List<string>>(binaryReader, 2);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVHeadframe.bytes";
		}

		private static CSVHeadframe instance = null;			
		public static CSVHeadframe Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVHeadframe 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVHeadframe forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVHeadframe();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVHeadframe");

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

    sealed public partial class CSVHeadframe : FCSVHeadframe
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVHeadframe.bytes";
		}

		private static CSVHeadframe instance = null;			
		public static CSVHeadframe Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVHeadframe 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVHeadframe forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVHeadframe();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVHeadframe");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}