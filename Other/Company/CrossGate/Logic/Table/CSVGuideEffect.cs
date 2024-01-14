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

	sealed public partial class CSVGuideEffect : Framework.Table.TableBase<CSVGuideEffect.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly string effect;
			public readonly string prefab_path;
			public readonly uint type;
			public readonly float effect_time;
			public readonly List<float> effect_pos;
			public readonly List<float> effect_rotation;
			public readonly List<float> effect_size;
			public readonly List<float> effect_scale;
			public readonly List<float> effect_anchors;
			public readonly List<float> effect_Pivot;
			public readonly uint tip_content;
			public readonly uint arrow_direction;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				effect = shareData.GetShareData<string>(binaryReader, 0);
				prefab_path = shareData.GetShareData<string>(binaryReader, 0);
				type = ReadHelper.ReadUInt(binaryReader);
				effect_time = ReadHelper.ReadFloat(binaryReader);
				effect_pos = shareData.GetShareData<List<float>>(binaryReader, 1);
				effect_rotation = shareData.GetShareData<List<float>>(binaryReader, 1);
				effect_size = shareData.GetShareData<List<float>>(binaryReader, 1);
				effect_scale = shareData.GetShareData<List<float>>(binaryReader, 1);
				effect_anchors = shareData.GetShareData<List<float>>(binaryReader, 1);
				effect_Pivot = shareData.GetShareData<List<float>>(binaryReader, 1);
				tip_content = ReadHelper.ReadUInt(binaryReader);
				arrow_direction = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVGuideEffect.bytes";
		}

		private static CSVGuideEffect instance = null;			
		public static CSVGuideEffect Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVGuideEffect 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVGuideEffect forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVGuideEffect();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVGuideEffect");

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
			shareData.ReadArrays<float>(binaryReader, 1, ReadHelper.ReadArray_ReadFloat);

			return shareData;
		}
	}

#else

    sealed public partial class CSVGuideEffect : FCSVGuideEffect
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVGuideEffect.bytes";
		}

		private static CSVGuideEffect instance = null;			
		public static CSVGuideEffect Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVGuideEffect 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVGuideEffect forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVGuideEffect();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVGuideEffect");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}