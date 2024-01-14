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

	sealed public partial class CSVFashionIcon : Framework.Table.TableBase<CSVFashionIcon.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly string Icon_Path;
			public readonly float Icon_scale;
			public readonly List<float> Icon_pos;
			public readonly float CompeteIcon_scale;
			public readonly List<float> CompeteIcon_pos;
			public readonly float Arena_scale;
			public readonly List<float> Arena_pos;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Icon_Path = shareData.GetShareData<string>(binaryReader, 0);
				Icon_scale = ReadHelper.ReadFloat(binaryReader);
				Icon_pos = shareData.GetShareData<List<float>>(binaryReader, 1);
				CompeteIcon_scale = ReadHelper.ReadFloat(binaryReader);
				CompeteIcon_pos = shareData.GetShareData<List<float>>(binaryReader, 1);
				Arena_scale = ReadHelper.ReadFloat(binaryReader);
				Arena_pos = shareData.GetShareData<List<float>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFashionIcon.bytes";
		}

		private static CSVFashionIcon instance = null;			
		public static CSVFashionIcon Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFashionIcon 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFashionIcon forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFashionIcon();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFashionIcon");

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
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<float>(binaryReader, 1, ReadHelper.ReadArray_ReadFloat);

			return shareData;
		}
	}

#else

    sealed public partial class CSVFashionIcon : FCSVFashionIcon
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFashionIcon.bytes";
		}

		private static CSVFashionIcon instance = null;			
		public static CSVFashionIcon Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFashionIcon 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFashionIcon forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFashionIcon();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFashionIcon");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}