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

	sealed public partial class CSVCluePhasedTasksGroup : Framework.Table.TableBase<CSVCluePhasedTasksGroup.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint PhasedTasksName;
			public readonly uint PhasedTasksDes;
			public readonly List<uint> SubTask;
			public readonly uint DisplayClueType;
			public readonly uint DisplayCluePara;
			public readonly uint TaskUnableReceiveTip;
			public bool Display { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				PhasedTasksName = ReadHelper.ReadUInt(binaryReader);
				PhasedTasksDes = ReadHelper.ReadUInt(binaryReader);
				SubTask = shareData.GetShareData<List<uint>>(binaryReader, 0);
				DisplayClueType = ReadHelper.ReadUInt(binaryReader);
				DisplayCluePara = ReadHelper.ReadUInt(binaryReader);
				TaskUnableReceiveTip = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVCluePhasedTasksGroup.bytes";
		}

		private static CSVCluePhasedTasksGroup instance = null;			
		public static CSVCluePhasedTasksGroup Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCluePhasedTasksGroup 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCluePhasedTasksGroup forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCluePhasedTasksGroup();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCluePhasedTasksGroup");

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

    sealed public partial class CSVCluePhasedTasksGroup : FCSVCluePhasedTasksGroup
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVCluePhasedTasksGroup.bytes";
		}

		private static CSVCluePhasedTasksGroup instance = null;			
		public static CSVCluePhasedTasksGroup Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCluePhasedTasksGroup 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCluePhasedTasksGroup forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCluePhasedTasksGroup();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCluePhasedTasksGroup");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}