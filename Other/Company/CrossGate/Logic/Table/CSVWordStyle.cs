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

	sealed public partial class CSVWordStyle : Framework.Table.TableBase<CSVWordStyle.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly Color32 FontColor;
			public readonly string FontName;
			public readonly byte FontStyle;
			public readonly byte FontSize;
			public readonly float Linespacing;
			public readonly Color32 Shadow;
			public readonly sbyte ShadowX;
			public readonly sbyte ShadowY;
			public readonly Color32 Outline;
			public readonly sbyte OutlineX;
			public readonly sbyte OutlineY;
			public readonly byte GradientColorDir;
			public readonly byte GradientColorMode;
			public readonly List<Color32> GradientColor;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				FontColor = ReadHelper.ReadColor32(binaryReader);
				FontName = shareData.GetShareData<string>(binaryReader, 0);
				FontStyle = ReadHelper.ReadByte(binaryReader);
				FontSize = ReadHelper.ReadByte(binaryReader);
				Linespacing = ReadHelper.ReadFloat(binaryReader);
				Shadow = ReadHelper.ReadColor32(binaryReader);
				ShadowX = ReadHelper.ReadSByte(binaryReader);
				ShadowY = ReadHelper.ReadSByte(binaryReader);
				Outline = ReadHelper.ReadColor32(binaryReader);
				OutlineX = ReadHelper.ReadSByte(binaryReader);
				OutlineY = ReadHelper.ReadSByte(binaryReader);
				GradientColorDir = ReadHelper.ReadByte(binaryReader);
				GradientColorMode = ReadHelper.ReadByte(binaryReader);
				GradientColor = shareData.GetShareData<List<Color32>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVWordStyle.bytes";
		}

		private static CSVWordStyle instance = null;			
		public static CSVWordStyle Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVWordStyle 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVWordStyle forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVWordStyle();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVWordStyle");

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
			shareData.ReadArrays<Color32>(binaryReader, 1, ReadHelper.ReadArray_ReadColor32);

			return shareData;
		}
	}

#else

    sealed public partial class CSVWordStyle : FCSVWordStyle
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVWordStyle.bytes";
		}

		private static CSVWordStyle instance = null;			
		public static CSVWordStyle Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVWordStyle 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVWordStyle forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVWordStyle();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVWordStyle");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}