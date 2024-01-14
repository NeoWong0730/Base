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

	sealed public partial class CSVDomesticationTask : Framework.Table.TableBase<CSVDomesticationTask.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly int name;
			public readonly int type;
			public readonly List<List<uint>> quality;
			public readonly List<uint> condition;
			public readonly List<uint> duration;
			public readonly List<List<uint>> addition_point;
			public readonly int race;
			public readonly int addition_race;
			public readonly List<uint> skill;
			public readonly int addition_skill;
			public readonly List<uint> reward;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				name = ReadHelper.ReadInt(binaryReader);
				type = ReadHelper.ReadInt(binaryReader);
				quality = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				condition = shareData.GetShareData<List<uint>>(binaryReader, 0);
				duration = shareData.GetShareData<List<uint>>(binaryReader, 0);
				addition_point = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				race = ReadHelper.ReadInt(binaryReader);
				addition_race = ReadHelper.ReadInt(binaryReader);
				skill = shareData.GetShareData<List<uint>>(binaryReader, 0);
				addition_skill = ReadHelper.ReadInt(binaryReader);
				reward = shareData.GetShareData<List<uint>>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVDomesticationTask.bytes";
		}

		private static CSVDomesticationTask instance = null;			
		public static CSVDomesticationTask Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVDomesticationTask 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVDomesticationTask forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVDomesticationTask();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVDomesticationTask");

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

    sealed public partial class CSVDomesticationTask : FCSVDomesticationTask
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVDomesticationTask.bytes";
		}

		private static CSVDomesticationTask instance = null;			
		public static CSVDomesticationTask Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVDomesticationTask 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVDomesticationTask forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVDomesticationTask();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVDomesticationTask");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}