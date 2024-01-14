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

	sealed public partial class CSVFoodAtlas : Framework.Table.TableBase<CSVFoodAtlas.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint access1;
			public readonly List<List<uint>> parameter1;
			public readonly uint desc1;
			public readonly uint access2;
			public readonly List<List<uint>> parameter2;
			public readonly uint desc2;
			public readonly uint access3;
			public readonly List<List<uint>> parameter3;
			public readonly uint desc3;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				access1 = ReadHelper.ReadUInt(binaryReader);
				parameter1 = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				desc1 = ReadHelper.ReadUInt(binaryReader);
				access2 = ReadHelper.ReadUInt(binaryReader);
				parameter2 = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				desc2 = ReadHelper.ReadUInt(binaryReader);
				access3 = ReadHelper.ReadUInt(binaryReader);
				parameter3 = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				desc3 = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFoodAtlas.bytes";
		}

		private static CSVFoodAtlas instance = null;			
		public static CSVFoodAtlas Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFoodAtlas 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFoodAtlas forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFoodAtlas();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFoodAtlas");

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

    sealed public partial class CSVFoodAtlas : FCSVFoodAtlas
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFoodAtlas.bytes";
		}

		private static CSVFoodAtlas instance = null;			
		public static CSVFoodAtlas Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFoodAtlas 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFoodAtlas forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFoodAtlas();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFoodAtlas");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}