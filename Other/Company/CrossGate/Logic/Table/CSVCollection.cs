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

	sealed public partial class CSVCollection : Framework.Table.TableBase<CSVCollection.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint collectionTask;
			public readonly List<uint> level_collection;
			public readonly uint cost;
			public readonly List<List<uint>> collectionExpendItem;
			public readonly uint CollectionLimit;
			public readonly uint CollectionLimitTips;
			public readonly uint CollectionGroup;
			public readonly uint ICollectionType;
			public readonly uint ICollectionNum;
			public readonly uint collectionPrivateFewTimes;
			public readonly uint collectionPrivateHitPoints;
			public readonly uint collectionUseNumRandom;
			public readonly uint areaShowMinimum;
			public readonly uint areaShowMaximum;
			public readonly uint collectionName;
			public readonly uint CollectionTips;
			public readonly uint CollectionIconTips;
			public readonly uint collectionProgress;
			public readonly List<uint> collectionAnimator;
			public readonly uint collectionFun_icon;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				collectionTask = ReadHelper.ReadUInt(binaryReader);
				level_collection = shareData.GetShareData<List<uint>>(binaryReader, 0);
				cost = ReadHelper.ReadUInt(binaryReader);
				collectionExpendItem = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				CollectionLimit = ReadHelper.ReadUInt(binaryReader);
				CollectionLimitTips = ReadHelper.ReadUInt(binaryReader);
				CollectionGroup = ReadHelper.ReadUInt(binaryReader);
				ICollectionType = ReadHelper.ReadUInt(binaryReader);
				ICollectionNum = ReadHelper.ReadUInt(binaryReader);
				collectionPrivateFewTimes = ReadHelper.ReadUInt(binaryReader);
				collectionPrivateHitPoints = ReadHelper.ReadUInt(binaryReader);
				collectionUseNumRandom = ReadHelper.ReadUInt(binaryReader);
				areaShowMinimum = ReadHelper.ReadUInt(binaryReader);
				areaShowMaximum = ReadHelper.ReadUInt(binaryReader);
				collectionName = ReadHelper.ReadUInt(binaryReader);
				CollectionTips = ReadHelper.ReadUInt(binaryReader);
				CollectionIconTips = ReadHelper.ReadUInt(binaryReader);
				collectionProgress = ReadHelper.ReadUInt(binaryReader);
				collectionAnimator = shareData.GetShareData<List<uint>>(binaryReader, 0);
				collectionFun_icon = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVCollection.bytes";
		}

		private static CSVCollection instance = null;			
		public static CSVCollection Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCollection 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCollection forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCollection();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCollection");

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

    sealed public partial class CSVCollection : FCSVCollection
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVCollection.bytes";
		}

		private static CSVCollection instance = null;			
		public static CSVCollection Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCollection 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCollection forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCollection();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCollection");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}