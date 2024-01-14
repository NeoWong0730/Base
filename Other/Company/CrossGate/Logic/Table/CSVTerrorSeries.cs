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

	sealed public partial class CSVTerrorSeries : Framework.Table.TableBase<CSVTerrorSeries.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Name;
			public readonly uint limite_number;
			public readonly List<uint> line_task;
			public readonly List<List<uint>> line_task1;
			public readonly uint instance_des;
			public readonly List<uint> line_name;
			public readonly List<uint> line_icon;
			public readonly List<uint> task_name;
			public readonly List<uint> task_des;
			public readonly List<uint> award_item;
			public readonly List<List<uint>> award_chance;
			public readonly uint TeamTargetID;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Name = ReadHelper.ReadUInt(binaryReader);
				limite_number = ReadHelper.ReadUInt(binaryReader);
				line_task = shareData.GetShareData<List<uint>>(binaryReader, 0);
				line_task1 = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				instance_des = ReadHelper.ReadUInt(binaryReader);
				line_name = shareData.GetShareData<List<uint>>(binaryReader, 0);
				line_icon = shareData.GetShareData<List<uint>>(binaryReader, 0);
				task_name = shareData.GetShareData<List<uint>>(binaryReader, 0);
				task_des = shareData.GetShareData<List<uint>>(binaryReader, 0);
				award_item = shareData.GetShareData<List<uint>>(binaryReader, 0);
				award_chance = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				TeamTargetID = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTerrorSeries.bytes";
		}

		private static CSVTerrorSeries instance = null;			
		public static CSVTerrorSeries Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTerrorSeries 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTerrorSeries forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTerrorSeries();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTerrorSeries");

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
			TableShareData shareData = new TableShareData(2);
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<uint>(binaryReader, 1, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVTerrorSeries : FCSVTerrorSeries
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTerrorSeries.bytes";
		}

		private static CSVTerrorSeries instance = null;			
		public static CSVTerrorSeries Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTerrorSeries 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTerrorSeries forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTerrorSeries();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTerrorSeries");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}