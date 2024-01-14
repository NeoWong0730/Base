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

	sealed public partial class CSVTaskType : Framework.Table.TableBase<CSVTaskType.Data>
	{
	    sealed public partial class Data
	    {
			public readonly uint id; // 任务类型
			public readonly uint typeName; // 任务类型名称
			public readonly uint acceptTipe; // 任务类型接受提示
			public readonly uint submitTipe; // 任务类型提交时提示
			public readonly uint typeIcon; // 任务类型图标
			public readonly uint typeSeqencing; // 任务类型显示优先级
			public readonly uint typeOpen; // 任务类型功能开启ID


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				typeName = ReadHelper.ReadUInt(binaryReader);
				acceptTipe = ReadHelper.ReadUInt(binaryReader);
				submitTipe = ReadHelper.ReadUInt(binaryReader);
				typeIcon = ReadHelper.ReadUInt(binaryReader);
				typeSeqencing = ReadHelper.ReadUInt(binaryReader);
				typeOpen = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTaskType.bytes";
		}

		private static CSVTaskType instance = null;			
		public static CSVTaskType Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTaskType 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTaskType forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTaskType();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTaskType");

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

    sealed public partial class CSVTaskType : FCSVTaskType
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTaskType.bytes";
		}

		private static CSVTaskType instance = null;			
		public static CSVTaskType Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTaskType 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTaskType forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTaskType();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTaskType");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}