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

	sealed public partial class CSVGoddessSelect : Framework.Table.TableBase<CSVGoddessSelect.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint StageLan;
			public readonly uint QuestionLan;
			public readonly uint StageId1;
			public readonly uint lanid1;
			public readonly uint StageId2;
			public readonly uint lanid2;
			public readonly uint StageId3;
			public readonly uint lanid3;
			public readonly uint StageId4;
			public readonly uint lanid4;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				StageLan = ReadHelper.ReadUInt(binaryReader);
				QuestionLan = ReadHelper.ReadUInt(binaryReader);
				StageId1 = ReadHelper.ReadUInt(binaryReader);
				lanid1 = ReadHelper.ReadUInt(binaryReader);
				StageId2 = ReadHelper.ReadUInt(binaryReader);
				lanid2 = ReadHelper.ReadUInt(binaryReader);
				StageId3 = ReadHelper.ReadUInt(binaryReader);
				lanid3 = ReadHelper.ReadUInt(binaryReader);
				StageId4 = ReadHelper.ReadUInt(binaryReader);
				lanid4 = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVGoddessSelect.bytes";
		}

		private static CSVGoddessSelect instance = null;			
		public static CSVGoddessSelect Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVGoddessSelect 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVGoddessSelect forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVGoddessSelect();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVGoddessSelect");

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
			TableShareData shareData = null;

			return shareData;
		}
	}

#else

    sealed public partial class CSVGoddessSelect : FCSVGoddessSelect
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVGoddessSelect.bytes";
		}

		private static CSVGoddessSelect instance = null;			
		public static CSVGoddessSelect Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVGoddessSelect 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVGoddessSelect forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVGoddessSelect();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVGoddessSelect");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}