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

	sealed public partial class CSVBullet : Framework.Table.TableBase<CSVBullet.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint show_hp_change;
			public readonly uint track_type;
			public readonly uint tie_point;
			public readonly uint position;
			public readonly uint is_forward;
			public readonly int parma1;
			public readonly uint bullet_change_type;
			public readonly int parma2;
			public readonly uint parma3;
			public readonly int position_offsetx;
			public readonly int position_offsety;
			public readonly int position_offsetz;
			public readonly uint effect_id;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				show_hp_change = ReadHelper.ReadUInt(binaryReader);
				track_type = ReadHelper.ReadUInt(binaryReader);
				tie_point = ReadHelper.ReadUInt(binaryReader);
				position = ReadHelper.ReadUInt(binaryReader);
				is_forward = ReadHelper.ReadUInt(binaryReader);
				parma1 = ReadHelper.ReadInt(binaryReader);
				bullet_change_type = ReadHelper.ReadUInt(binaryReader);
				parma2 = ReadHelper.ReadInt(binaryReader);
				parma3 = ReadHelper.ReadUInt(binaryReader);
				position_offsetx = ReadHelper.ReadInt(binaryReader);
				position_offsety = ReadHelper.ReadInt(binaryReader);
				position_offsetz = ReadHelper.ReadInt(binaryReader);
				effect_id = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVBullet.bytes";
		}

		private static CSVBullet instance = null;			
		public static CSVBullet Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBullet 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBullet forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBullet();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBullet");

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
			TableShareData shareData = null;

			return shareData;
		}
	}

#else

    sealed public partial class CSVBullet : FCSVBullet
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVBullet.bytes";
		}

		private static CSVBullet instance = null;			
		public static CSVBullet Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBullet 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBullet forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBullet();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBullet");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}