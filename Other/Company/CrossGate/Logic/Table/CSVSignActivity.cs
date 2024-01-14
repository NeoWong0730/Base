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

	sealed public partial class CSVSignActivity : Framework.Table.TableBase<CSVSignActivity.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Activiyid;
			public readonly uint itemId;
			public readonly uint day;
			public readonly string Image1;
			public readonly string Image2;
			public readonly string Image3;
			public readonly string Image4;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Activiyid = ReadHelper.ReadUInt(binaryReader);
				itemId = ReadHelper.ReadUInt(binaryReader);
				day = ReadHelper.ReadUInt(binaryReader);
				Image1 = shareData.GetShareData<string>(binaryReader, 0);
				Image2 = shareData.GetShareData<string>(binaryReader, 0);
				Image3 = shareData.GetShareData<string>(binaryReader, 0);
				Image4 = shareData.GetShareData<string>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVSignActivity.bytes";
		}

		private static CSVSignActivity instance = null;			
		public static CSVSignActivity Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSignActivity 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSignActivity forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSignActivity();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSignActivity");

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

    sealed public partial class CSVSignActivity : FCSVSignActivity
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVSignActivity.bytes";
		}

		private static CSVSignActivity instance = null;			
		public static CSVSignActivity Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSignActivity 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSignActivity forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSignActivity();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSignActivity");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}