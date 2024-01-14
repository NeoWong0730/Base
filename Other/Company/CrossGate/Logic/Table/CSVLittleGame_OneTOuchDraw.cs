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

	sealed public partial class CSVLittleGame_OneTOuchDraw : Framework.Table.TableBase<CSVLittleGame_OneTOuchDraw.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly List<int> validId;
			public readonly int startId;
			public readonly List<int> imageId;
			public readonly float time;
			public readonly string image_path;
			public readonly uint tips;
			public readonly uint gameDescribe;
			public readonly string linePath;
			public readonly uint ArrowId;
			public readonly string imagepath;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				validId = shareData.GetShareData<List<int>>(binaryReader, 1);
				startId = ReadHelper.ReadInt(binaryReader);
				imageId = shareData.GetShareData<List<int>>(binaryReader, 1);
				time = ReadHelper.ReadFloat(binaryReader);
				image_path = shareData.GetShareData<string>(binaryReader, 0);
				tips = ReadHelper.ReadUInt(binaryReader);
				gameDescribe = ReadHelper.ReadUInt(binaryReader);
				linePath = shareData.GetShareData<string>(binaryReader, 0);
				ArrowId = ReadHelper.ReadUInt(binaryReader);
				imagepath = shareData.GetShareData<string>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVLittleGame_OneTOuchDraw.bytes";
		}

		private static CSVLittleGame_OneTOuchDraw instance = null;			
		public static CSVLittleGame_OneTOuchDraw Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVLittleGame_OneTOuchDraw 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVLittleGame_OneTOuchDraw forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVLittleGame_OneTOuchDraw();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVLittleGame_OneTOuchDraw");

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
			shareData.ReadArrays<int>(binaryReader, 1, ReadHelper.ReadArray_ReadInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVLittleGame_OneTOuchDraw : FCSVLittleGame_OneTOuchDraw
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVLittleGame_OneTOuchDraw.bytes";
		}

		private static CSVLittleGame_OneTOuchDraw instance = null;			
		public static CSVLittleGame_OneTOuchDraw Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVLittleGame_OneTOuchDraw 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVLittleGame_OneTOuchDraw forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVLittleGame_OneTOuchDraw();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVLittleGame_OneTOuchDraw");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}