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

	sealed public partial class CSVCutScene : Framework.Table.TableBase<CSVCutScene.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly byte type;
			public bool blockSingleMusic { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly string path;
			public readonly uint nextId;
			public bool isShowBubble { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
			public readonly int weather;
			public readonly byte dayNight;
			public readonly byte forEnter;
			public readonly byte forExit;
			public readonly ushort fadeoutTime;
			public readonly int time;
			public readonly uint audioId;
			public bool isHighModel { get { return ReadHelper.GetBoolByIndex(boolArray0, 2); } }
			public bool isLoadMainPlayer { get { return ReadHelper.GetBoolByIndex(boolArray0, 3); } }
			public readonly List<string> hideThings;
			public readonly List<uint> mapID;
			public readonly List<uint> actions;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				type = ReadHelper.ReadByte(binaryReader);
				path = shareData.GetShareData<string>(binaryReader, 0);
				nextId = ReadHelper.ReadUInt(binaryReader);
				weather = ReadHelper.ReadInt(binaryReader);
				dayNight = ReadHelper.ReadByte(binaryReader);
				forEnter = ReadHelper.ReadByte(binaryReader);
				forExit = ReadHelper.ReadByte(binaryReader);
				fadeoutTime = ReadHelper.ReadUShort(binaryReader);
				time = ReadHelper.ReadInt(binaryReader);
				audioId = ReadHelper.ReadUInt(binaryReader);
				hideThings = shareData.GetShareData<List<string>>(binaryReader, 1);
				mapID = shareData.GetShareData<List<uint>>(binaryReader, 2);
				actions = shareData.GetShareData<List<uint>>(binaryReader, 2);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVCutScene.bytes";
		}

		private static CSVCutScene instance = null;			
		public static CSVCutScene Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCutScene 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCutScene forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCutScene();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCutScene");

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
			TableShareData shareData = new TableShareData(3);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadStringArrays(binaryReader, 1, 0);
			shareData.ReadArrays<uint>(binaryReader, 2, ReadHelper.ReadArray_ReadUInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVCutScene : FCSVCutScene
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVCutScene.bytes";
		}

		private static CSVCutScene instance = null;			
		public static CSVCutScene Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCutScene 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCutScene forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCutScene();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCutScene");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}