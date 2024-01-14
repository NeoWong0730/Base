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

	sealed public partial class CSVCutSceneSubTitle : Framework.Table.TableBase<CSVCutSceneSubTitle.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint content;
			public readonly uint audio;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				content = ReadHelper.ReadUInt(binaryReader);
				audio = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVCutSceneSubTitle.bytes";
		}

		private static CSVCutSceneSubTitle instance = null;			
		public static CSVCutSceneSubTitle Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCutSceneSubTitle 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCutSceneSubTitle forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCutSceneSubTitle();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCutSceneSubTitle");

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

    sealed public partial class CSVCutSceneSubTitle : FCSVCutSceneSubTitle
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVCutSceneSubTitle.bytes";
		}

		private static CSVCutSceneSubTitle instance = null;			
		public static CSVCutSceneSubTitle Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCutSceneSubTitle 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCutSceneSubTitle forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCutSceneSubTitle();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCutSceneSubTitle");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}