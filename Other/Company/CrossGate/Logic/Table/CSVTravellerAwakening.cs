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

	sealed public partial class CSVTravellerAwakening : Framework.Table.TableBase<CSVTravellerAwakening.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint NameId;
			public readonly uint StepsId;
			public readonly List<List<uint>> add_attr;
			public readonly List<uint> show_attr_name;
			public readonly List<uint> show_attr_value;
			public readonly List<uint> ActProject;
			public readonly List<List<uint>> ActCondition;
			public readonly List<List<uint>> Award;
			public readonly uint ActTitle;
			public readonly uint BuffID;
			public readonly uint PassiveInfoID;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				NameId = ReadHelper.ReadUInt(binaryReader);
				StepsId = ReadHelper.ReadUInt(binaryReader);
				add_attr = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				show_attr_name = shareData.GetShareData<List<uint>>(binaryReader, 0);
				show_attr_value = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ActProject = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ActCondition = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				Award = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				ActTitle = ReadHelper.ReadUInt(binaryReader);
				BuffID = ReadHelper.ReadUInt(binaryReader);
				PassiveInfoID = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVTravellerAwakening.bytes";
		}

		private static CSVTravellerAwakening instance = null;			
		public static CSVTravellerAwakening Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTravellerAwakening 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTravellerAwakening forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTravellerAwakening();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTravellerAwakening");

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

    sealed public partial class CSVTravellerAwakening : FCSVTravellerAwakening
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVTravellerAwakening.bytes";
		}

		private static CSVTravellerAwakening instance = null;			
		public static CSVTravellerAwakening Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVTravellerAwakening 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVTravellerAwakening forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVTravellerAwakening();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVTravellerAwakening");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}