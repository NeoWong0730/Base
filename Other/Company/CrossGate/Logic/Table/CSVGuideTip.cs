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

	sealed public partial class CSVGuideTip : Framework.Table.TableBase<CSVGuideTip.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint tip_type;
			public readonly uint Model;
			public readonly List<uint> Motion;
			public readonly List<float> Position;
			public readonly List<float> Rotation;
			public readonly List<float> Scale;
			public readonly uint image_name;
			public readonly uint tip_scale;
			public readonly float tip_time;
			public readonly List<float> tip_size;
			public readonly List<float> tip_pos;
			public readonly uint tip_content;
			public readonly List<float> tip_anchors;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				tip_type = ReadHelper.ReadUInt(binaryReader);
				Model = ReadHelper.ReadUInt(binaryReader);
				Motion = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Position = shareData.GetShareData<List<float>>(binaryReader, 1);
				Rotation = shareData.GetShareData<List<float>>(binaryReader, 1);
				Scale = shareData.GetShareData<List<float>>(binaryReader, 1);
				image_name = ReadHelper.ReadUInt(binaryReader);
				tip_scale = ReadHelper.ReadUInt(binaryReader);
				tip_time = ReadHelper.ReadFloat(binaryReader);
				tip_size = shareData.GetShareData<List<float>>(binaryReader, 1);
				tip_pos = shareData.GetShareData<List<float>>(binaryReader, 1);
				tip_content = ReadHelper.ReadUInt(binaryReader);
				tip_anchors = shareData.GetShareData<List<float>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVGuideTip.bytes";
		}

		private static CSVGuideTip instance = null;			
		public static CSVGuideTip Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVGuideTip 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVGuideTip forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVGuideTip();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVGuideTip");

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
			shareData.ReadArrays<float>(binaryReader, 1, ReadHelper.ReadArray_ReadFloat);

			return shareData;
		}
	}

#else

    sealed public partial class CSVGuideTip : FCSVGuideTip
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVGuideTip.bytes";
		}

		private static CSVGuideTip instance = null;			
		public static CSVGuideTip Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVGuideTip 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVGuideTip forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVGuideTip();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVGuideTip");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}