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

	sealed public partial class CSVSeekItem : Framework.Table.TableBase<CSVSeekItem.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint taskName;
			public readonly uint taskDescribe;
			public readonly uint spaceTips;
			public readonly uint errorTips;
			public readonly uint trueTips;
			public readonly List<uint> itemId;
			public readonly uint showItem;
			public readonly uint effectType;
			public readonly List<uint> showId;
			public readonly List<uint> showScale;
			public readonly List<uint> showRotate;
			public readonly List<List<int>> showSeat;
			public readonly List<int> camera;
			public readonly List<int> consultPosition;
			public readonly List<int> dialogueParameter;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				taskName = ReadHelper.ReadUInt(binaryReader);
				taskDescribe = ReadHelper.ReadUInt(binaryReader);
				spaceTips = ReadHelper.ReadUInt(binaryReader);
				errorTips = ReadHelper.ReadUInt(binaryReader);
				trueTips = ReadHelper.ReadUInt(binaryReader);
				itemId = shareData.GetShareData<List<uint>>(binaryReader, 0);
				showItem = ReadHelper.ReadUInt(binaryReader);
				effectType = ReadHelper.ReadUInt(binaryReader);
				showId = shareData.GetShareData<List<uint>>(binaryReader, 0);
				showScale = shareData.GetShareData<List<uint>>(binaryReader, 0);
				showRotate = shareData.GetShareData<List<uint>>(binaryReader, 0);
				showSeat = shareData.GetShareData<List<List<int>>>(binaryReader, 2);
				camera = shareData.GetShareData<List<int>>(binaryReader, 1);
				consultPosition = shareData.GetShareData<List<int>>(binaryReader, 1);
				dialogueParameter = shareData.GetShareData<List<int>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVSeekItem.bytes";
		}

		private static CSVSeekItem instance = null;			
		public static CSVSeekItem Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSeekItem 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSeekItem forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSeekItem();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSeekItem");

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
			TableShareData shareData = new TableShareData(3);
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArrays<int>(binaryReader, 1, ReadHelper.ReadArray_ReadInt);
			shareData.ReadArray2s<int>(binaryReader, 2, 1);

			return shareData;
		}
	}

#else

    sealed public partial class CSVSeekItem : FCSVSeekItem
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVSeekItem.bytes";
		}

		private static CSVSeekItem instance = null;			
		public static CSVSeekItem Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSeekItem 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSeekItem forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSeekItem();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSeekItem");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}