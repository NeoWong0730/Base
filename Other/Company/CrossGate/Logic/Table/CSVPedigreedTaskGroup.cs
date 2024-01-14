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

	sealed public partial class CSVPedigreedTaskGroup : Framework.Table.TableBase<CSVPedigreedTaskGroup.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Task_Des;
			public readonly List<uint> ReachTypeAchievement;
			public readonly List<uint> Change_UI;
			public readonly uint Tip;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Task_Des = ReadHelper.ReadUInt(binaryReader);
				ReachTypeAchievement = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Change_UI = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Tip = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVPedigreedTaskGroup.bytes";
		}

		private static CSVPedigreedTaskGroup instance = null;			
		public static CSVPedigreedTaskGroup Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPedigreedTaskGroup 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPedigreedTaskGroup forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPedigreedTaskGroup();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPedigreedTaskGroup");

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

    sealed public partial class CSVPedigreedTaskGroup : FCSVPedigreedTaskGroup
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVPedigreedTaskGroup.bytes";
		}

		private static CSVPedigreedTaskGroup instance = null;			
		public static CSVPedigreedTaskGroup Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPedigreedTaskGroup 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPedigreedTaskGroup forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPedigreedTaskGroup();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPedigreedTaskGroup");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}