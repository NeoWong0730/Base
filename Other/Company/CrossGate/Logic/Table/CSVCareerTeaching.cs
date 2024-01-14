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

	sealed public partial class CSVCareerTeaching : Framework.Table.TableBase<CSVCareerTeaching.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly List<uint> primary;
			public readonly List<uint> middle;
			public readonly List<uint> high;
			public readonly List<uint> state_desc;
			public readonly List<uint> damage_desc;
			public readonly List<uint> recovery_desc;
			public readonly List<uint> faq;
			public readonly List<uint> special_desc;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				primary = shareData.GetShareData<List<uint>>(binaryReader, 0);
				middle = shareData.GetShareData<List<uint>>(binaryReader, 0);
				high = shareData.GetShareData<List<uint>>(binaryReader, 0);
				state_desc = shareData.GetShareData<List<uint>>(binaryReader, 0);
				damage_desc = shareData.GetShareData<List<uint>>(binaryReader, 0);
				recovery_desc = shareData.GetShareData<List<uint>>(binaryReader, 0);
				faq = shareData.GetShareData<List<uint>>(binaryReader, 0);
				special_desc = shareData.GetShareData<List<uint>>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVCareerTeaching.bytes";
		}

		private static CSVCareerTeaching instance = null;			
		public static CSVCareerTeaching Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCareerTeaching 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCareerTeaching forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCareerTeaching();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCareerTeaching");

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

    sealed public partial class CSVCareerTeaching : FCSVCareerTeaching
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVCareerTeaching.bytes";
		}

		private static CSVCareerTeaching instance = null;			
		public static CSVCareerTeaching Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCareerTeaching 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCareerTeaching forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCareerTeaching();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCareerTeaching");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}