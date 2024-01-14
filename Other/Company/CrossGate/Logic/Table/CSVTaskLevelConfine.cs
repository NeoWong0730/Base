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

	sealed public partial class CSVTaskLevelConfine : Framework.Table.TableBase<CSVTaskLevelConfine.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public bool active { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly uint Way;
			public readonly uint Icon;
			public readonly string bg;
			public readonly uint Description;
			public readonly uint Uiid;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Way = ReadHelper.ReadUInt(binaryReader);
				Icon = ReadHelper.ReadUInt(binaryReader);
				bg = shareData.GetShareData<string>(binaryReader, 0);
				Description = ReadHelper.ReadUInt(binaryReader);
				Uiid = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTaskLevelConfine.bytes";
		}

		private static CSVTaskLevelConfine instance = null;			
		public static CSVTaskLevelConfine Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTaskLevelConfine 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTaskLevelConfine forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTaskLevelConfine();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTaskLevelConfine");

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

    sealed public partial class CSVTaskLevelConfine : FCSVTaskLevelConfine
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTaskLevelConfine.bytes";
		}

		private static CSVTaskLevelConfine instance = null;			
		public static CSVTaskLevelConfine Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTaskLevelConfine 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTaskLevelConfine forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTaskLevelConfine();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTaskLevelConfine");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}