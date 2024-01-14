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

	sealed public partial class CSVCommodityCategory : Framework.Table.TableBase<CSVCommodityCategory.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint name;
			public readonly uint seqencing;
			public readonly uint iconid;
			public readonly uint icon;
			public bool show { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public bool turn_page { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
			public bool bargain { get { return ReadHelper.GetBoolByIndex(boolArray0, 2); } }
			public readonly List<uint> subclass;
			public readonly List<uint> subclass_name;
			public bool current_subclass { get { return ReadHelper.GetBoolByIndex(boolArray0, 3); } }
			public readonly uint list;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				name = ReadHelper.ReadUInt(binaryReader);
				seqencing = ReadHelper.ReadUInt(binaryReader);
				iconid = ReadHelper.ReadUInt(binaryReader);
				icon = ReadHelper.ReadUInt(binaryReader);
				subclass = shareData.GetShareData<List<uint>>(binaryReader, 0);
				subclass_name = shareData.GetShareData<List<uint>>(binaryReader, 0);
				list = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVCommodityCategory.bytes";
		}

		private static CSVCommodityCategory instance = null;			
		public static CSVCommodityCategory Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCommodityCategory 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCommodityCategory forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCommodityCategory();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCommodityCategory");

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

    sealed public partial class CSVCommodityCategory : FCSVCommodityCategory
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVCommodityCategory.bytes";
		}

		private static CSVCommodityCategory instance = null;			
		public static CSVCommodityCategory Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCommodityCategory 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCommodityCategory forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCommodityCategory();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCommodityCategory");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}