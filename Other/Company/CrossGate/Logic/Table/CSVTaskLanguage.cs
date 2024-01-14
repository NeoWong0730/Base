﻿//CSVWordStyle
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

	sealed public partial class CSVTaskLanguage : Framework.Table.TableBase<CSVTaskLanguage.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly string words;
			public readonly uint wordStyle;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				words = shareData.GetShareData<string>(binaryReader, 0);
				wordStyle = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTaskLanguage.bytes";
		}

		private static CSVTaskLanguage instance = null;			
		public static CSVTaskLanguage Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTaskLanguage 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTaskLanguage forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTaskLanguage();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTaskLanguage");

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

    sealed public partial class CSVTaskLanguage : FCSVTaskLanguage
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTaskLanguage.bytes";
		}

		private static CSVTaskLanguage instance = null;			
		public static CSVTaskLanguage Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTaskLanguage 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTaskLanguage forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTaskLanguage();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTaskLanguage");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}