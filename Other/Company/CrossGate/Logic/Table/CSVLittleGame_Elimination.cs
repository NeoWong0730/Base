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

	sealed public partial class CSVLittleGame_Elimination : Framework.Table.TableBase<CSVLittleGame_Elimination.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly List<uint> Goal;
			public readonly uint Goaltips;
			public readonly List<uint> imageId;
			public readonly float time;
			public readonly string image_path;
			public readonly uint tips;
			public readonly uint gameDescribe;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Goal = shareData.GetShareData<List<uint>>(binaryReader, 1);
				Goaltips = ReadHelper.ReadUInt(binaryReader);
				imageId = shareData.GetShareData<List<uint>>(binaryReader, 1);
				time = ReadHelper.ReadFloat(binaryReader);
				image_path = shareData.GetShareData<string>(binaryReader, 0);
				tips = ReadHelper.ReadUInt(binaryReader);
				gameDescribe = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVLittleGame_Elimination.bytes";
		}

		private static CSVLittleGame_Elimination instance = null;			
		public static CSVLittleGame_Elimination Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVLittleGame_Elimination 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVLittleGame_Elimination forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVLittleGame_Elimination();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVLittleGame_Elimination");

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

    sealed public partial class CSVLittleGame_Elimination : FCSVLittleGame_Elimination
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVLittleGame_Elimination.bytes";
		}

		private static CSVLittleGame_Elimination instance = null;			
		public static CSVLittleGame_Elimination Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVLittleGame_Elimination 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVLittleGame_Elimination forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVLittleGame_Elimination();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVLittleGame_Elimination");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}