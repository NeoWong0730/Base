//CSVTaskLanguage|CSVWordStyle
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

	sealed public partial class CSVTaskCategory : Framework.Table.TableBase<CSVTaskCategory.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint name;
			public readonly uint simpleName;
			public bool WhetherTips { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly uint taskTypeTips;
			public readonly List<uint> taskTypeTipsColour;
			public readonly uint typeImage;
			public readonly uint wordStyle;
			public readonly int priority;
			public readonly uint traceLimit;
			public readonly uint funcOpenId;
			public readonly uint iconId;
			public readonly uint lightIconId;
			public bool MainPanelShow { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
			public bool TaskPanelShow { get { return ReadHelper.GetBoolByIndex(boolArray0, 2); } }
			public readonly uint TaskButtonType;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				name = ReadHelper.ReadUInt(binaryReader);
				simpleName = ReadHelper.ReadUInt(binaryReader);
				taskTypeTips = ReadHelper.ReadUInt(binaryReader);
				taskTypeTipsColour = shareData.GetShareData<List<uint>>(binaryReader, 0);
				typeImage = ReadHelper.ReadUInt(binaryReader);
				wordStyle = ReadHelper.ReadUInt(binaryReader);
				priority = ReadHelper.ReadInt(binaryReader);
				traceLimit = ReadHelper.ReadUInt(binaryReader);
				funcOpenId = ReadHelper.ReadUInt(binaryReader);
				iconId = ReadHelper.ReadUInt(binaryReader);
				lightIconId = ReadHelper.ReadUInt(binaryReader);
				TaskButtonType = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTaskCategory.bytes";
		}

		private static CSVTaskCategory instance = null;			
		public static CSVTaskCategory Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTaskCategory 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTaskCategory forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTaskCategory();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTaskCategory");

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

    sealed public partial class CSVTaskCategory : FCSVTaskCategory
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTaskCategory.bytes";
		}

		private static CSVTaskCategory instance = null;			
		public static CSVTaskCategory Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTaskCategory 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTaskCategory forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTaskCategory();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTaskCategory");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}