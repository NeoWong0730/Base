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

	sealed public partial class CSVAdventureCriminal : Framework.Table.TableBase<CSVAdventureCriminal.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint name;
			public readonly uint isUrgent;
			public readonly uint preTaskId;
			public readonly uint preLevel;
			public readonly uint simpleDes;
			public readonly uint detailedDes;
			public readonly uint acceptTaskId;
			public readonly List<uint> nodeTask;
			public readonly uint finishTaskId;
			public readonly string greyImage;
			public readonly string image;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				name = ReadHelper.ReadUInt(binaryReader);
				isUrgent = ReadHelper.ReadUInt(binaryReader);
				preTaskId = ReadHelper.ReadUInt(binaryReader);
				preLevel = ReadHelper.ReadUInt(binaryReader);
				simpleDes = ReadHelper.ReadUInt(binaryReader);
				detailedDes = ReadHelper.ReadUInt(binaryReader);
				acceptTaskId = ReadHelper.ReadUInt(binaryReader);
				nodeTask = shareData.GetShareData<List<uint>>(binaryReader, 1);
				finishTaskId = ReadHelper.ReadUInt(binaryReader);
				greyImage = shareData.GetShareData<string>(binaryReader, 0);
				image = shareData.GetShareData<string>(binaryReader, 0);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVAdventureCriminal.bytes";
		}

		private static CSVAdventureCriminal instance = null;			
		public static CSVAdventureCriminal Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAdventureCriminal 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAdventureCriminal forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAdventureCriminal();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAdventureCriminal");

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

    sealed public partial class CSVAdventureCriminal : FCSVAdventureCriminal
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVAdventureCriminal.bytes";
		}

		private static CSVAdventureCriminal instance = null;			
		public static CSVAdventureCriminal Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVAdventureCriminal 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVAdventureCriminal forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVAdventureCriminal();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVAdventureCriminal");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}