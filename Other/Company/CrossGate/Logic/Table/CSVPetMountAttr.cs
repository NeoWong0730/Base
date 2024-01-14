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

	sealed public partial class CSVPetMountAttr : Framework.Table.TableBase<CSVPetMountAttr.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint group_id;
			public readonly List<List<int>> base_attr;
			public readonly List<List<int>> base_attr_min;
			public readonly uint gear_param;
			public readonly uint quality;
			public readonly uint score;
			public readonly List<int> strengthen_score;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				group_id = ReadHelper.ReadUInt(binaryReader);
				base_attr = shareData.GetShareData<List<List<int>>>(binaryReader, 1);
				base_attr_min = shareData.GetShareData<List<List<int>>>(binaryReader, 1);
				gear_param = ReadHelper.ReadUInt(binaryReader);
				quality = ReadHelper.ReadUInt(binaryReader);
				score = ReadHelper.ReadUInt(binaryReader);
				strengthen_score = shareData.GetShareData<List<int>>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVPetMountAttr.bytes";
		}

		private static CSVPetMountAttr instance = null;			
		public static CSVPetMountAttr Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetMountAttr 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetMountAttr forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetMountAttr();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetMountAttr");

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
			shareData.ReadArrays<int>(binaryReader, 0, ReadHelper.ReadArray_ReadInt);
			shareData.ReadArray2s<int>(binaryReader, 1, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVPetMountAttr : FCSVPetMountAttr
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVPetMountAttr.bytes";
		}

		private static CSVPetMountAttr instance = null;			
		public static CSVPetMountAttr Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVPetMountAttr 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVPetMountAttr forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVPetMountAttr();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVPetMountAttr");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}