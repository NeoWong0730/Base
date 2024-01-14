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

	sealed public partial class CSVChronology : Framework.Table.TableBase<CSVChronology.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly string icon;
			public readonly uint years;
			public readonly uint event_titel;
			public readonly string show_image;
			public readonly uint event_text;
			public readonly uint Source;
			public readonly uint deblock_type;
			public readonly uint deblock_condition;
			public readonly uint currency;
			public readonly uint show_above;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				icon = shareData.GetShareData<string>(binaryReader, 0);
				years = ReadHelper.ReadUInt(binaryReader);
				event_titel = ReadHelper.ReadUInt(binaryReader);
				show_image = shareData.GetShareData<string>(binaryReader, 0);
				event_text = ReadHelper.ReadUInt(binaryReader);
				Source = ReadHelper.ReadUInt(binaryReader);
				deblock_type = ReadHelper.ReadUInt(binaryReader);
				deblock_condition = ReadHelper.ReadUInt(binaryReader);
				currency = ReadHelper.ReadUInt(binaryReader);
				show_above = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVChronology.bytes";
		}

		private static CSVChronology instance = null;			
		public static CSVChronology Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVChronology 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVChronology forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVChronology();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVChronology");

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

    sealed public partial class CSVChronology : FCSVChronology
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVChronology.bytes";
		}

		private static CSVChronology instance = null;			
		public static CSVChronology Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVChronology 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVChronology forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVChronology();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVChronology");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}