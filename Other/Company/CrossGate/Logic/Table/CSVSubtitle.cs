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

	sealed public partial class CSVSubtitle : Framework.Table.TableBase<CSVSubtitle.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly string picModel;
			public readonly uint subtitleModel;
			public bool lens { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly List<uint> content;
			public readonly uint voiceModel;
			public readonly uint musicModel;
			public readonly uint wordNumber;
			public readonly uint retainTime;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				picModel = shareData.GetShareData<string>(binaryReader, 0);
				subtitleModel = ReadHelper.ReadUInt(binaryReader);
				content = shareData.GetShareData<List<uint>>(binaryReader, 1);
				voiceModel = ReadHelper.ReadUInt(binaryReader);
				musicModel = ReadHelper.ReadUInt(binaryReader);
				wordNumber = ReadHelper.ReadUInt(binaryReader);
				retainTime = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVSubtitle.bytes";
		}

		private static CSVSubtitle instance = null;			
		public static CSVSubtitle Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSubtitle 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSubtitle forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSubtitle();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSubtitle");

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
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVSubtitle : FCSVSubtitle
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVSubtitle.bytes";
		}

		private static CSVSubtitle instance = null;			
		public static CSVSubtitle Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSubtitle 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSubtitle forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSubtitle();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSubtitle");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}