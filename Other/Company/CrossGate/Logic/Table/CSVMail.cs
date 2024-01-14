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

	sealed public partial class CSVMail : Framework.Table.TableBase<CSVMail.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly string MailTitle;
			public readonly string MailContent;
			public readonly List<List<int>> Enclosure;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				MailTitle = shareData.GetShareData<string>(binaryReader, 0);
				MailContent = shareData.GetShareData<string>(binaryReader, 0);
				Enclosure = shareData.GetShareData<List<List<int>>>(binaryReader, 2);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVMail.bytes";
		}

		private static CSVMail instance = null;			
		public static CSVMail Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVMail 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVMail forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVMail();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVMail");

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
			TableShareData shareData = new TableShareData(3);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<int>(binaryReader, 1, ReadHelper.ReadArray_ReadInt);
			shareData.ReadArray2s<int>(binaryReader, 2, 1);

			return shareData;
		}
	}

#else

    sealed public partial class CSVMail : FCSVMail
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVMail.bytes";
		}

		private static CSVMail instance = null;			
		public static CSVMail Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVMail 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVMail forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVMail();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVMail");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}