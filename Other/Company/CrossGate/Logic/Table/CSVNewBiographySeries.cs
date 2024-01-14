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

	sealed public partial class CSVNewBiographySeries : Framework.Table.TableBase<CSVNewBiographySeries.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint sort_id;
			public readonly List<uint> instance_id;
			public readonly uint Name;
			public readonly uint icon;
			public readonly string model;
			public readonly uint action_id;
			public readonly uint shadow;
			public readonly uint positionx;
			public readonly uint positiony;
			public readonly uint positionz;
			public readonly uint rotationx;
			public readonly uint rotationy;
			public readonly uint rotationz;
			public readonly uint scale;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				sort_id = ReadHelper.ReadUInt(binaryReader);
				instance_id = shareData.GetShareData<List<uint>>(binaryReader, 1);
				Name = ReadHelper.ReadUInt(binaryReader);
				icon = ReadHelper.ReadUInt(binaryReader);
				model = shareData.GetShareData<string>(binaryReader, 0);
				action_id = ReadHelper.ReadUInt(binaryReader);
				shadow = ReadHelper.ReadUInt(binaryReader);
				positionx = ReadHelper.ReadUInt(binaryReader);
				positiony = ReadHelper.ReadUInt(binaryReader);
				positionz = ReadHelper.ReadUInt(binaryReader);
				rotationx = ReadHelper.ReadUInt(binaryReader);
				rotationy = ReadHelper.ReadUInt(binaryReader);
				rotationz = ReadHelper.ReadUInt(binaryReader);
				scale = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVNewBiographySeries.bytes";
		}

		private static CSVNewBiographySeries instance = null;			
		public static CSVNewBiographySeries Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVNewBiographySeries 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVNewBiographySeries forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVNewBiographySeries();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVNewBiographySeries");

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
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVNewBiographySeries : FCSVNewBiographySeries
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVNewBiographySeries.bytes";
		}

		private static CSVNewBiographySeries instance = null;			
		public static CSVNewBiographySeries Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVNewBiographySeries 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVNewBiographySeries forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVNewBiographySeries();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVNewBiographySeries");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}