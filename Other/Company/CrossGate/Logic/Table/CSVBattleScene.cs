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

	sealed public partial class CSVBattleScene : Framework.Table.TableBase<CSVBattleScene.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly int battle_scene_pointx;
			public readonly int battle_scene_pointy;
			public readonly int battle_scene_pointz;
			public readonly int pith;
			public readonly int yaw;
			public readonly List<int> distance;
			public readonly List<int> far;
			public readonly int fov;
			public readonly int x_offset;
			public readonly int y_offset;
			public readonly int z_offset;
			public readonly uint position_type;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				battle_scene_pointx = ReadHelper.ReadInt(binaryReader);
				battle_scene_pointy = ReadHelper.ReadInt(binaryReader);
				battle_scene_pointz = ReadHelper.ReadInt(binaryReader);
				pith = ReadHelper.ReadInt(binaryReader);
				yaw = ReadHelper.ReadInt(binaryReader);
				distance = shareData.GetShareData<List<int>>(binaryReader, 0);
				far = shareData.GetShareData<List<int>>(binaryReader, 0);
				fov = ReadHelper.ReadInt(binaryReader);
				x_offset = ReadHelper.ReadInt(binaryReader);
				y_offset = ReadHelper.ReadInt(binaryReader);
				z_offset = ReadHelper.ReadInt(binaryReader);
				position_type = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVBattleScene.bytes";
		}

		private static CSVBattleScene instance = null;			
		public static CSVBattleScene Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBattleScene 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBattleScene forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBattleScene();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBattleScene");

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
			shareData.ReadArrays<int>(binaryReader, 0, ReadHelper.ReadArray_ReadInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVBattleScene : FCSVBattleScene
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVBattleScene.bytes";
		}

		private static CSVBattleScene instance = null;			
		public static CSVBattleScene Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBattleScene 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBattleScene forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBattleScene();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBattleScene");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}