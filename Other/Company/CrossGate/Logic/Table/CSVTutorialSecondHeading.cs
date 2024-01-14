﻿//
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

	sealed public partial class CSVTutorialSecondHeading : Framework.Table.TableBase<CSVTutorialSecondHeading.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint secondHeadingName;
			public readonly uint secondHeadingSrot;
			public readonly List<uint> tutorial_array;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				secondHeadingName = ReadHelper.ReadUInt(binaryReader);
				secondHeadingSrot = ReadHelper.ReadUInt(binaryReader);
				tutorial_array = shareData.GetShareData<List<uint>>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTutorialSecondHeading.bytes";
		}

		private static CSVTutorialSecondHeading instance = null;			
		public static CSVTutorialSecondHeading Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTutorialSecondHeading 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTutorialSecondHeading forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTutorialSecondHeading();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTutorialSecondHeading");

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
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVTutorialSecondHeading : FCSVTutorialSecondHeading
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTutorialSecondHeading.bytes";
		}

		private static CSVTutorialSecondHeading instance = null;			
		public static CSVTutorialSecondHeading Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTutorialSecondHeading 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTutorialSecondHeading forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTutorialSecondHeading();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTutorialSecondHeading");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}