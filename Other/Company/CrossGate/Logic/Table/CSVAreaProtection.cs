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

	sealed public partial class CSVAreaProtection : Framework.Table.TableBase<CSVAreaProtection.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly string name;
			public readonly uint eventName_id;
			public readonly uint eventDescription_id;
			public readonly uint entrustDescription_id;
			public readonly uint dialogue_id;
			public readonly List<uint> task_id_array;
			public readonly List<uint> drop_id_array;
			public readonly List<List<uint>> openTime;
			public readonly uint eventLv;
			public readonly uint map_id;
			public readonly List<float> event_position;
			public readonly List<float> eventName_position;
			public readonly List<float> eventDescription_position;
			public readonly uint eventName_direction;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				name = shareData.GetShareData<string>(binaryReader, 0);
				eventName_id = ReadHelper.ReadUInt(binaryReader);
				eventDescription_id = ReadHelper.ReadUInt(binaryReader);
				entrustDescription_id = ReadHelper.ReadUInt(binaryReader);
				dialogue_id = ReadHelper.ReadUInt(binaryReader);
				task_id_array = shareData.GetShareData<List<uint>>(binaryReader, 1);
				drop_id_array = shareData.GetShareData<List<uint>>(binaryReader, 1);
				openTime = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				eventLv = ReadHelper.ReadUInt(binaryReader);
				map_id = ReadHelper.ReadUInt(binaryReader);
				event_position = shareData.GetShareData<List<float>>(binaryReader, 2);
				eventName_position = shareData.GetShareData<List<float>>(binaryReader, 2);
				eventDescription_position = shareData.GetShareData<List<float>>(binaryReader, 2);
				eventName_direction = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVAreaProtection.bytes";
		}

		private static CSVAreaProtection instance = null;			
		public static CSVAreaProtection Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAreaProtection 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAreaProtection forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAreaProtection();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAreaProtection");

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
			TableShareData shareData = new TableShareData(4);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArrays<float>(binaryReader, 2, ReadHelper.ReadArray_ReadFloat);
			shareData.ReadArray2s<uint>(binaryReader, 3, 1);

			return shareData;
		}
	}

#else

    sealed public partial class CSVAreaProtection : FCSVAreaProtection
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVAreaProtection.bytes";
		}

		private static CSVAreaProtection instance = null;			
		public static CSVAreaProtection Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAreaProtection 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAreaProtection forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAreaProtection();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAreaProtection");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}