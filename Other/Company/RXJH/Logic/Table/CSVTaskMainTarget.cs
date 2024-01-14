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

	sealed public partial class CSVTaskMainTarget : Framework.Table.TableBase<CSVTaskMainTarget.Data>
	{
	    sealed public partial class Data
	    {
			public readonly uint id; // 任务目标
			public readonly uint taskTargetDescribe; // 任务目标描述
			public readonly List<uint> taskTargetData; // 任务目标参数
			public readonly uint taskPath; // 执行任务寻路
			public readonly uint taskPathTarget; // 执行任务寻路目标
			public readonly uint taskPathData; // 执行任务行为参数
			public readonly uint appearUi; // 执行任务打开功能
			public readonly uint appearLan; // 任务目标执行时弹提示
			public readonly uint appearGuide; // 执行任务目标出现引导


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				taskTargetDescribe = ReadHelper.ReadUInt(binaryReader);
				taskTargetData = shareData.GetShareData<List<uint>>(binaryReader, 0);
				taskPath = ReadHelper.ReadUInt(binaryReader);
				taskPathTarget = ReadHelper.ReadUInt(binaryReader);
				taskPathData = ReadHelper.ReadUInt(binaryReader);
				appearUi = ReadHelper.ReadUInt(binaryReader);
				appearLan = ReadHelper.ReadUInt(binaryReader);
				appearGuide = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTaskMainTarget.bytes";
		}

		private static CSVTaskMainTarget instance = null;			
		public static CSVTaskMainTarget Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTaskMainTarget 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTaskMainTarget forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTaskMainTarget();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTaskMainTarget");

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

    sealed public partial class CSVTaskMainTarget : FCSVTaskMainTarget
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTaskMainTarget.bytes";
		}

		private static CSVTaskMainTarget instance = null;			
		public static CSVTaskMainTarget Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTaskMainTarget 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTaskMainTarget forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTaskMainTarget();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTaskMainTarget");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}