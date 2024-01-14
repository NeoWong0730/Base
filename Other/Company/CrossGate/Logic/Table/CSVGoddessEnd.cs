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

	sealed public partial class CSVGoddessEnd : Framework.Table.TableBase<CSVGoddessEnd.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly List<uint> stageId;
			public readonly uint endingName;
			public readonly uint endingLan;
			public readonly string endingTexture;
			public readonly uint topicId;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				stageId = shareData.GetShareData<List<uint>>(binaryReader, 1);
				endingName = ReadHelper.ReadUInt(binaryReader);
				endingLan = ReadHelper.ReadUInt(binaryReader);
				endingTexture = shareData.GetShareData<string>(binaryReader, 0);
				topicId = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVGoddessEnd.bytes";
		}

		private static CSVGoddessEnd instance = null;			
		public static CSVGoddessEnd Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVGoddessEnd 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVGoddessEnd forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVGoddessEnd();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVGoddessEnd");

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

    sealed public partial class CSVGoddessEnd : FCSVGoddessEnd
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVGoddessEnd.bytes";
		}

		private static CSVGoddessEnd instance = null;			
		public static CSVGoddessEnd Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVGoddessEnd 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVGoddessEnd forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVGoddessEnd();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVGoddessEnd");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}