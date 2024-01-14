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

	sealed public partial class CSVMapInfo : Framework.Table.TableBase<CSVMapInfo.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint level;
			public readonly uint taskid;
			public readonly List<int> map_node;
			public readonly uint name;
			public readonly uint des;
			public readonly string mapIcon;
			public readonly string path;
			public readonly int length;
			public readonly int width;
			public readonly string picture;
			public readonly uint room_pointx;
			public readonly uint room_pointy;
			public readonly uint room_pointz;
			public readonly uint sound_bgm;
			public readonly List<int> monster_id;
			public readonly List<int> map_lv;
			public readonly int fog_type;
			public readonly int minimap_showtype;
			public readonly string minimap_res;
			public readonly List<int> minimap_size;
			public readonly List<int> ui_size;
			public readonly List<float> ui_pos;
			public readonly float ui_length;
			public readonly float ui_width;
			public readonly uint can_pvp;
			public readonly uint use_scence;
			public readonly uint weather;
			public readonly List<int> remove_weather;
			public readonly byte EnemySwitch;
			public readonly List<int> map_size;
			public readonly uint is_show;
			public bool AutoHangup { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public bool DisablePromptBox { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
			public bool PromptForMapUnlocking { get { return ReadHelper.GetBoolByIndex(boolArray0, 2); } }
			public readonly uint PromptName;
			public readonly uint PromptContent;
			public readonly List<uint> MapResource;
			public bool MinimapPathFinding { get { return ReadHelper.GetBoolByIndex(boolArray0, 3); } }
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				level = ReadHelper.ReadUInt(binaryReader);
				taskid = ReadHelper.ReadUInt(binaryReader);
				map_node = shareData.GetShareData<List<int>>(binaryReader, 1);
				name = ReadHelper.ReadUInt(binaryReader);
				des = ReadHelper.ReadUInt(binaryReader);
				mapIcon = shareData.GetShareData<string>(binaryReader, 0);
				path = shareData.GetShareData<string>(binaryReader, 0);
				length = ReadHelper.ReadInt(binaryReader);
				width = ReadHelper.ReadInt(binaryReader);
				picture = shareData.GetShareData<string>(binaryReader, 0);
				room_pointx = ReadHelper.ReadUInt(binaryReader);
				room_pointy = ReadHelper.ReadUInt(binaryReader);
				room_pointz = ReadHelper.ReadUInt(binaryReader);
				sound_bgm = ReadHelper.ReadUInt(binaryReader);
				monster_id = shareData.GetShareData<List<int>>(binaryReader, 1);
				map_lv = shareData.GetShareData<List<int>>(binaryReader, 1);
				fog_type = ReadHelper.ReadInt(binaryReader);
				minimap_showtype = ReadHelper.ReadInt(binaryReader);
				minimap_res = shareData.GetShareData<string>(binaryReader, 0);
				minimap_size = shareData.GetShareData<List<int>>(binaryReader, 1);
				ui_size = shareData.GetShareData<List<int>>(binaryReader, 1);
				ui_pos = shareData.GetShareData<List<float>>(binaryReader, 2);
				ui_length = ReadHelper.ReadFloat(binaryReader);
				ui_width = ReadHelper.ReadFloat(binaryReader);
				can_pvp = ReadHelper.ReadUInt(binaryReader);
				use_scence = ReadHelper.ReadUInt(binaryReader);
				weather = ReadHelper.ReadUInt(binaryReader);
				remove_weather = shareData.GetShareData<List<int>>(binaryReader, 1);
				EnemySwitch = ReadHelper.ReadByte(binaryReader);
				map_size = shareData.GetShareData<List<int>>(binaryReader, 1);
				is_show = ReadHelper.ReadUInt(binaryReader);
				PromptName = ReadHelper.ReadUInt(binaryReader);
				PromptContent = ReadHelper.ReadUInt(binaryReader);
				MapResource = shareData.GetShareData<List<uint>>(binaryReader, 3);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVMapInfo.bytes";
		}

		private static CSVMapInfo instance = null;			
		public static CSVMapInfo Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVMapInfo 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVMapInfo forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVMapInfo();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVMapInfo");

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
			TableShareData shareData = new TableShareData(4);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<int>(binaryReader, 1, ReadHelper.ReadArray_ReadInt);
			shareData.ReadArrays<float>(binaryReader, 2, ReadHelper.ReadArray_ReadFloat);
			shareData.ReadArrays<uint>(binaryReader, 3, ReadHelper.ReadArray_ReadUInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVMapInfo : FCSVMapInfo
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVMapInfo.bytes";
		}

		private static CSVMapInfo instance = null;			
		public static CSVMapInfo Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVMapInfo 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVMapInfo forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVMapInfo();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVMapInfo");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}