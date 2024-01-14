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

	sealed public partial class CSVTitle : Framework.Table.TableBase<CSVTitle.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint titleLan;
			public readonly uint professionId;
			public readonly uint professionChange;
			public readonly uint titleShowLan;
			public readonly List<Color32> titleShow;
			public readonly uint titleShowIcon;
			public readonly uint titleShowEffect;
			public readonly uint titleShowClass;
			public readonly int titleType;
			public readonly int titleTypeNum;
			public readonly int titleTypeLimit;
			public readonly List<uint> titleSeries;
			public readonly uint titleGetType;
			public readonly uint titleGetLan;
			public readonly List<uint> titleGet;
			public readonly List<uint> titleGo;
			public readonly int titleLimitTimeType;
			public readonly int titleLimitTime;
			public readonly int titleOrder;
			public readonly List<List<uint>> titleProperty;
			public readonly int titlePoint;
			public readonly int isShow;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				titleLan = ReadHelper.ReadUInt(binaryReader);
				professionId = ReadHelper.ReadUInt(binaryReader);
				professionChange = ReadHelper.ReadUInt(binaryReader);
				titleShowLan = ReadHelper.ReadUInt(binaryReader);
				titleShow = shareData.GetShareData<List<Color32>>(binaryReader, 0);
				titleShowIcon = ReadHelper.ReadUInt(binaryReader);
				titleShowEffect = ReadHelper.ReadUInt(binaryReader);
				titleShowClass = ReadHelper.ReadUInt(binaryReader);
				titleType = ReadHelper.ReadInt(binaryReader);
				titleTypeNum = ReadHelper.ReadInt(binaryReader);
				titleTypeLimit = ReadHelper.ReadInt(binaryReader);
				titleSeries = shareData.GetShareData<List<uint>>(binaryReader, 1);
				titleGetType = ReadHelper.ReadUInt(binaryReader);
				titleGetLan = ReadHelper.ReadUInt(binaryReader);
				titleGet = shareData.GetShareData<List<uint>>(binaryReader, 1);
				titleGo = shareData.GetShareData<List<uint>>(binaryReader, 1);
				titleLimitTimeType = ReadHelper.ReadInt(binaryReader);
				titleLimitTime = ReadHelper.ReadInt(binaryReader);
				titleOrder = ReadHelper.ReadInt(binaryReader);
				titleProperty = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				titlePoint = ReadHelper.ReadInt(binaryReader);
				isShow = ReadHelper.ReadInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTitle.bytes";
		}

		private static CSVTitle instance = null;			
		public static CSVTitle Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTitle 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTitle forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTitle();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTitle");

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
			shareData.ReadArrays<Color32>(binaryReader, 0, ReadHelper.ReadArray_ReadColor32);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<uint>(binaryReader, 2, 1);

			return shareData;
		}
	}

#else

    sealed public partial class CSVTitle : FCSVTitle
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTitle.bytes";
		}

		private static CSVTitle instance = null;			
		public static CSVTitle Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTitle 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTitle forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTitle();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTitle");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}