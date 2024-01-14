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

	sealed public partial class CSVQuestionnaire : Framework.Table.TableBase<CSVQuestionnaire.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint title;
			public readonly uint tips;
			public readonly uint FunctionId;
			public readonly uint MailId;
			public readonly string QuestionURL;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				title = ReadHelper.ReadUInt(binaryReader);
				tips = ReadHelper.ReadUInt(binaryReader);
				FunctionId = ReadHelper.ReadUInt(binaryReader);
				MailId = ReadHelper.ReadUInt(binaryReader);
				QuestionURL = shareData.GetShareData<string>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVQuestionnaire.bytes";
		}

		private static CSVQuestionnaire instance = null;			
		public static CSVQuestionnaire Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVQuestionnaire 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVQuestionnaire forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVQuestionnaire();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVQuestionnaire");

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

    sealed public partial class CSVQuestionnaire : FCSVQuestionnaire
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVQuestionnaire.bytes";
		}

		private static CSVQuestionnaire instance = null;			
		public static CSVQuestionnaire Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVQuestionnaire 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVQuestionnaire forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVQuestionnaire();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVQuestionnaire");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}