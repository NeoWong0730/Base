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

	sealed public partial class CSVActivityMainnUi : Framework.Table.TableBase<CSVActivityMainnUi.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly List<uint> Activity_Id;
			public readonly uint UiId;
			public readonly List<uint> UiParam;
			public readonly List<uint> IntIcon;
			public readonly List<uint> IconTitle;
			public readonly uint Function_Id;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Activity_Id = shareData.GetShareData<List<uint>>(binaryReader, 0);
				UiId = ReadHelper.ReadUInt(binaryReader);
				UiParam = shareData.GetShareData<List<uint>>(binaryReader, 0);
				IntIcon = shareData.GetShareData<List<uint>>(binaryReader, 0);
				IconTitle = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Function_Id = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVActivityMainnUi.bytes";
		}

		private static CSVActivityMainnUi instance = null;			
		public static CSVActivityMainnUi Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVActivityMainnUi 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVActivityMainnUi forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVActivityMainnUi();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVActivityMainnUi");

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

    sealed public partial class CSVActivityMainnUi : FCSVActivityMainnUi
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVActivityMainnUi.bytes";
		}

		private static CSVActivityMainnUi instance = null;			
		public static CSVActivityMainnUi Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVActivityMainnUi 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVActivityMainnUi forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVActivityMainnUi();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVActivityMainnUi");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}