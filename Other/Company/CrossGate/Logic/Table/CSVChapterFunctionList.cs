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

	sealed public partial class CSVChapterFunctionList : Framework.Table.TableBase<CSVChapterFunctionList.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Chapter;
			public readonly uint SysIcon;
			public bool IsFunction { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly uint ItemDisplay;
			public readonly uint SystemType;
			public readonly uint SystemLanguage;
			public readonly uint FunctionForecast;
			public readonly uint FunctionId;
			public readonly uint InterfaceDes;
			public readonly List<string> Picture;
			public readonly List<uint> PictureDescribe;
			public readonly List<uint> ShowRegionalTask;
			public readonly List<uint> OtherRegionalTask;
			public readonly uint Type;
			public readonly List<uint> JumpInterface;
			public readonly uint TirggerLv;
			public readonly List<List<uint>> TirggerTask;
			public readonly uint Display;
			public readonly uint AccOrder;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Chapter = ReadHelper.ReadUInt(binaryReader);
				SysIcon = ReadHelper.ReadUInt(binaryReader);
				ItemDisplay = ReadHelper.ReadUInt(binaryReader);
				SystemType = ReadHelper.ReadUInt(binaryReader);
				SystemLanguage = ReadHelper.ReadUInt(binaryReader);
				FunctionForecast = ReadHelper.ReadUInt(binaryReader);
				FunctionId = ReadHelper.ReadUInt(binaryReader);
				InterfaceDes = ReadHelper.ReadUInt(binaryReader);
				Picture = shareData.GetShareData<List<string>>(binaryReader, 1);
				PictureDescribe = shareData.GetShareData<List<uint>>(binaryReader, 2);
				ShowRegionalTask = shareData.GetShareData<List<uint>>(binaryReader, 2);
				OtherRegionalTask = shareData.GetShareData<List<uint>>(binaryReader, 2);
				Type = ReadHelper.ReadUInt(binaryReader);
				JumpInterface = shareData.GetShareData<List<uint>>(binaryReader, 2);
				TirggerLv = ReadHelper.ReadUInt(binaryReader);
				TirggerTask = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				Display = ReadHelper.ReadUInt(binaryReader);
				AccOrder = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVChapterFunctionList.bytes";
		}

		private static CSVChapterFunctionList instance = null;			
		public static CSVChapterFunctionList Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVChapterFunctionList 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVChapterFunctionList forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVChapterFunctionList();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVChapterFunctionList");

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
			TableShareData shareData = new TableShareData(4);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadStringArrays(binaryReader, 1, 0);
			shareData.ReadArrays<uint>(binaryReader, 2, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<uint>(binaryReader, 3, 2);

			return shareData;
		}
	}

#else

    sealed public partial class CSVChapterFunctionList : FCSVChapterFunctionList
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVChapterFunctionList.bytes";
		}

		private static CSVChapterFunctionList instance = null;			
		public static CSVChapterFunctionList Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVChapterFunctionList 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVChapterFunctionList forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVChapterFunctionList();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVChapterFunctionList");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}