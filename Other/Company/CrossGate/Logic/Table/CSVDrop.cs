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

	sealed public partial class CSVDrop : Framework.Table.TableBase<CSVDrop.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint drop_id;
			public readonly List<uint> role;
			public readonly List<uint> career;
			public readonly List<uint> level;
			public readonly List<uint> world_level;
			public readonly uint equip_para;
			public readonly List<List<uint>> reward_show;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				drop_id = ReadHelper.ReadUInt(binaryReader);
				role = shareData.GetShareData<List<uint>>(binaryReader, 0);
				career = shareData.GetShareData<List<uint>>(binaryReader, 0);
				level = shareData.GetShareData<List<uint>>(binaryReader, 0);
				world_level = shareData.GetShareData<List<uint>>(binaryReader, 0);
				equip_para = ReadHelper.ReadUInt(binaryReader);
				reward_show = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVDrop.bytes";
		}

		private static CSVDrop instance = null;			
		public static CSVDrop Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVDrop 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVDrop forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVDrop();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVDrop");

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

    sealed public partial class CSVDrop : FCSVDrop
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVDrop.bytes";
		}

		private static CSVDrop instance = null;			
		public static CSVDrop Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVDrop 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVDrop forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVDrop();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVDrop");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}