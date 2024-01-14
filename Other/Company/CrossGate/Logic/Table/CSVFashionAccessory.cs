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

	sealed public partial class CSVFashionAccessory : Framework.Table.TableBase<CSVFashionAccessory.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint AccName;
			public readonly uint AccDescribe;
			public readonly uint Acctype;
			public readonly List<uint> AccItem;
			public readonly string AccSlot;
			public readonly uint AccIcon;
			public readonly string model_show;
			public readonly string model;
			public readonly uint MaxColour;
			public readonly List<List<uint>> AccColour;
			public readonly List<List<uint>> itemNeed;
			public readonly List<List<uint>> itemNeed1;
			public readonly uint FashionScore;
			public bool Hide { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly uint Lock;
			public readonly uint LimitedTime;
			public readonly uint suitId;
			public readonly List<List<uint>> attr_id;
			public readonly uint score;
			public readonly uint is_overturn;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				AccName = ReadHelper.ReadUInt(binaryReader);
				AccDescribe = ReadHelper.ReadUInt(binaryReader);
				Acctype = ReadHelper.ReadUInt(binaryReader);
				AccItem = shareData.GetShareData<List<uint>>(binaryReader, 1);
				AccSlot = shareData.GetShareData<string>(binaryReader, 0);
				AccIcon = ReadHelper.ReadUInt(binaryReader);
				model_show = shareData.GetShareData<string>(binaryReader, 0);
				model = shareData.GetShareData<string>(binaryReader, 0);
				MaxColour = ReadHelper.ReadUInt(binaryReader);
				AccColour = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				itemNeed = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				itemNeed1 = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				FashionScore = ReadHelper.ReadUInt(binaryReader);
				Lock = ReadHelper.ReadUInt(binaryReader);
				LimitedTime = ReadHelper.ReadUInt(binaryReader);
				suitId = ReadHelper.ReadUInt(binaryReader);
				attr_id = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				score = ReadHelper.ReadUInt(binaryReader);
				is_overturn = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFashionAccessory.bytes";
		}

		private static CSVFashionAccessory instance = null;			
		public static CSVFashionAccessory Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFashionAccessory 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFashionAccessory forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFashionAccessory();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFashionAccessory");

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
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<uint>(binaryReader, 2, 1);

			return shareData;
		}
	}

#else

    sealed public partial class CSVFashionAccessory : FCSVFashionAccessory
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFashionAccessory.bytes";
		}

		private static CSVFashionAccessory instance = null;			
		public static CSVFashionAccessory Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFashionAccessory 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFashionAccessory forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFashionAccessory();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFashionAccessory");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}