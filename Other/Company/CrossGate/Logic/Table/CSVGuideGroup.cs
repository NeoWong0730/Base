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

	sealed public partial class CSVGuideGroup : Framework.Table.TableBase<CSVGuideGroup.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint order;
			public readonly uint mode;
			public readonly List<uint> career_condition;
			public readonly List<uint> lv;
			public readonly List<uint> instance_limited;
			public readonly uint task_limited;
			public readonly uint task_finish;
			public readonly uint tasktarget_finish;
			public readonly uint function_id;
			public readonly uint UI_id;
			public readonly List<string> Tab_path;
			public readonly uint enemy_id;
			public readonly List<uint> fight_time;
			public readonly uint repeat;
			public readonly List<List<uint>> award;
			public readonly List<List<uint>> link_id;
			public readonly List<List<int>> condition;
			public readonly uint file;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				order = ReadHelper.ReadUInt(binaryReader);
				mode = ReadHelper.ReadUInt(binaryReader);
				career_condition = shareData.GetShareData<List<uint>>(binaryReader, 1);
				lv = shareData.GetShareData<List<uint>>(binaryReader, 1);
				instance_limited = shareData.GetShareData<List<uint>>(binaryReader, 1);
				task_limited = ReadHelper.ReadUInt(binaryReader);
				task_finish = ReadHelper.ReadUInt(binaryReader);
				tasktarget_finish = ReadHelper.ReadUInt(binaryReader);
				function_id = ReadHelper.ReadUInt(binaryReader);
				UI_id = ReadHelper.ReadUInt(binaryReader);
				Tab_path = shareData.GetShareData<List<string>>(binaryReader, 2);
				enemy_id = ReadHelper.ReadUInt(binaryReader);
				fight_time = shareData.GetShareData<List<uint>>(binaryReader, 1);
				repeat = ReadHelper.ReadUInt(binaryReader);
				award = shareData.GetShareData<List<List<uint>>>(binaryReader, 4);
				link_id = shareData.GetShareData<List<List<uint>>>(binaryReader, 4);
				condition = shareData.GetShareData<List<List<int>>>(binaryReader, 5);
				file = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVGuideGroup.bytes";
		}

		private static CSVGuideGroup instance = null;			
		public static CSVGuideGroup Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVGuideGroup 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVGuideGroup forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVGuideGroup();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVGuideGroup");

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
			TableShareData shareData = new TableShareData(6);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadStringArrays(binaryReader, 2, 0);
			shareData.ReadArrays<int>(binaryReader, 3, ReadHelper.ReadArray_ReadInt);
			shareData.ReadArray2s<uint>(binaryReader, 4, 1);
			shareData.ReadArray2s<int>(binaryReader, 5, 3);

			return shareData;
		}
	}

#else

    sealed public partial class CSVGuideGroup : FCSVGuideGroup
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVGuideGroup.bytes";
		}

		private static CSVGuideGroup instance = null;			
		public static CSVGuideGroup Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVGuideGroup 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVGuideGroup forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVGuideGroup();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVGuideGroup");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}