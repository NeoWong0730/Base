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

	sealed public partial class CSVEffect : Framework.Table.TableBase<CSVEffect.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint pre_load;
			public readonly uint max_effect;
			public readonly uint tie_point;
			public readonly uint isrotate;
			public readonly int rotation_x;
			public readonly int rotation_y;
			public readonly int rotation_z;
			public readonly uint fx_starttime;
			public readonly uint fx_duration;
			public readonly int position_offsetx;
			public readonly int position_offsety;
			public readonly int position_offsetz;
			public readonly List<int> position_type_rotation;
			public readonly string effects_path;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				pre_load = ReadHelper.ReadUInt(binaryReader);
				max_effect = ReadHelper.ReadUInt(binaryReader);
				tie_point = ReadHelper.ReadUInt(binaryReader);
				isrotate = ReadHelper.ReadUInt(binaryReader);
				rotation_x = ReadHelper.ReadInt(binaryReader);
				rotation_y = ReadHelper.ReadInt(binaryReader);
				rotation_z = ReadHelper.ReadInt(binaryReader);
				fx_starttime = ReadHelper.ReadUInt(binaryReader);
				fx_duration = ReadHelper.ReadUInt(binaryReader);
				position_offsetx = ReadHelper.ReadInt(binaryReader);
				position_offsety = ReadHelper.ReadInt(binaryReader);
				position_offsetz = ReadHelper.ReadInt(binaryReader);
				position_type_rotation = shareData.GetShareData<List<int>>(binaryReader, 1);
				effects_path = shareData.GetShareData<string>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVEffect.bytes";
		}

		private static CSVEffect instance = null;			
		public static CSVEffect Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVEffect 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVEffect forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVEffect();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVEffect");

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
			shareData.ReadArrays<int>(binaryReader, 1, ReadHelper.ReadArray_ReadInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVEffect : FCSVEffect
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVEffect.bytes";
		}

		private static CSVEffect instance = null;			
		public static CSVEffect Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVEffect 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVEffect forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVEffect();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVEffect");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}