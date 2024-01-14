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

	sealed public partial class CSVCharacter : Framework.Table.TableBase<CSVCharacter.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint active;
			public readonly uint name;
			public readonly uint desc;
			public readonly uint sex;
			public readonly uint headid;
			public readonly uint name_icon;
			public readonly string model;
			public readonly uint create_char_audio;
			public readonly string create_char_timeline;
			public readonly string model_show;
			public readonly uint show_weapon_id;
			public readonly uint inti_attr;
			public readonly uint inti_level;
			public readonly uint create;
			public readonly List<uint> auto_battle;
			public readonly int LeftLocationX;
			public readonly int LeftLocationY;
			public readonly int LeftLocationZ;
			public readonly int LeftLocationRotateX;
			public readonly int LeftLocationRotateY;
			public readonly int LeftLocationRotateZ;
			public readonly int LeftLocationMirrorImage;
			public readonly int RightLocationX;
			public readonly int RightLocationY;
			public readonly int RightLocationZ;
			public readonly int RightLocationRotateX;
			public readonly int RightLocationRotateY;
			public readonly int RightLocationRotateZ;
			public readonly int RightLocationMirrorImage;
			public readonly List<uint> signPositionShifting;
			public readonly int BubbleLocationX;
			public readonly int BubbleLocationY;
			public readonly int BubbleLocationZ;
			public readonly int BubbleLocationRotateX;
			public readonly int BubbleLocationRotateY;
			public readonly int BubbleLocationRotateZ;
			public readonly int BubbleLocationMirrorImage;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				active = ReadHelper.ReadUInt(binaryReader);
				name = ReadHelper.ReadUInt(binaryReader);
				desc = ReadHelper.ReadUInt(binaryReader);
				sex = ReadHelper.ReadUInt(binaryReader);
				headid = ReadHelper.ReadUInt(binaryReader);
				name_icon = ReadHelper.ReadUInt(binaryReader);
				model = shareData.GetShareData<string>(binaryReader, 0);
				create_char_audio = ReadHelper.ReadUInt(binaryReader);
				create_char_timeline = shareData.GetShareData<string>(binaryReader, 0);
				model_show = shareData.GetShareData<string>(binaryReader, 0);
				show_weapon_id = ReadHelper.ReadUInt(binaryReader);
				inti_attr = ReadHelper.ReadUInt(binaryReader);
				inti_level = ReadHelper.ReadUInt(binaryReader);
				create = ReadHelper.ReadUInt(binaryReader);
				auto_battle = shareData.GetShareData<List<uint>>(binaryReader, 1);
				LeftLocationX = ReadHelper.ReadInt(binaryReader);
				LeftLocationY = ReadHelper.ReadInt(binaryReader);
				LeftLocationZ = ReadHelper.ReadInt(binaryReader);
				LeftLocationRotateX = ReadHelper.ReadInt(binaryReader);
				LeftLocationRotateY = ReadHelper.ReadInt(binaryReader);
				LeftLocationRotateZ = ReadHelper.ReadInt(binaryReader);
				LeftLocationMirrorImage = ReadHelper.ReadInt(binaryReader);
				RightLocationX = ReadHelper.ReadInt(binaryReader);
				RightLocationY = ReadHelper.ReadInt(binaryReader);
				RightLocationZ = ReadHelper.ReadInt(binaryReader);
				RightLocationRotateX = ReadHelper.ReadInt(binaryReader);
				RightLocationRotateY = ReadHelper.ReadInt(binaryReader);
				RightLocationRotateZ = ReadHelper.ReadInt(binaryReader);
				RightLocationMirrorImage = ReadHelper.ReadInt(binaryReader);
				signPositionShifting = shareData.GetShareData<List<uint>>(binaryReader, 1);
				BubbleLocationX = ReadHelper.ReadInt(binaryReader);
				BubbleLocationY = ReadHelper.ReadInt(binaryReader);
				BubbleLocationZ = ReadHelper.ReadInt(binaryReader);
				BubbleLocationRotateX = ReadHelper.ReadInt(binaryReader);
				BubbleLocationRotateY = ReadHelper.ReadInt(binaryReader);
				BubbleLocationRotateZ = ReadHelper.ReadInt(binaryReader);
				BubbleLocationMirrorImage = ReadHelper.ReadInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVCharacter.bytes";
		}

		private static CSVCharacter instance = null;			
		public static CSVCharacter Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCharacter 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCharacter forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCharacter();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCharacter");

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

    sealed public partial class CSVCharacter : FCSVCharacter
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVCharacter.bytes";
		}

		private static CSVCharacter instance = null;			
		public static CSVCharacter Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCharacter 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCharacter forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCharacter();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCharacter");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}