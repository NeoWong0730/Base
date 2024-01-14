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

	sealed public partial class CSVGuideArrow : Framework.Table.TableBase<CSVGuideArrow.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly float arrow_time;
			public readonly List<float> arrow_pos;
			public readonly List<float> arrow_rotation;
			public readonly List<float> arrow_scale;
			public readonly List<float> arrow_anchors;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				arrow_time = ReadHelper.ReadFloat(binaryReader);
				arrow_pos = shareData.GetShareData<List<float>>(binaryReader, 0);
				arrow_rotation = shareData.GetShareData<List<float>>(binaryReader, 0);
				arrow_scale = shareData.GetShareData<List<float>>(binaryReader, 0);
				arrow_anchors = shareData.GetShareData<List<float>>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVGuideArrow.bytes";
		}

		private static CSVGuideArrow instance = null;			
		public static CSVGuideArrow Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVGuideArrow 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVGuideArrow forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVGuideArrow();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVGuideArrow");

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
			shareData.ReadArrays<float>(binaryReader, 0, ReadHelper.ReadArray_ReadFloat);

			return shareData;
		}
	}

#else

    sealed public partial class CSVGuideArrow : FCSVGuideArrow
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVGuideArrow.bytes";
		}

		private static CSVGuideArrow instance = null;			
		public static CSVGuideArrow Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVGuideArrow 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVGuideArrow forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVGuideArrow();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVGuideArrow");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}