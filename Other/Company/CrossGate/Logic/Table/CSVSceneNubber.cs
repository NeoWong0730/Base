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

	sealed public partial class CSVSceneNubber : Framework.Table.TableBase<CSVSceneNubber.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly List<int> zozeId;
			public readonly uint minimumMove;
			public readonly List<List<uint>> moveId;
			public readonly uint material;
			public readonly List<int> camera;
			public readonly List<int> consultPosition;
			public readonly List<int> dialogueParameter;
			public readonly uint waitTime;
			public readonly uint skipTime;
			public readonly uint returnTime;
			public readonly uint openTime;
			public readonly uint timeDelay;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				zozeId = shareData.GetShareData<List<int>>(binaryReader, 0);
				minimumMove = ReadHelper.ReadUInt(binaryReader);
				moveId = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				material = ReadHelper.ReadUInt(binaryReader);
				camera = shareData.GetShareData<List<int>>(binaryReader, 0);
				consultPosition = shareData.GetShareData<List<int>>(binaryReader, 0);
				dialogueParameter = shareData.GetShareData<List<int>>(binaryReader, 0);
				waitTime = ReadHelper.ReadUInt(binaryReader);
				skipTime = ReadHelper.ReadUInt(binaryReader);
				returnTime = ReadHelper.ReadUInt(binaryReader);
				openTime = ReadHelper.ReadUInt(binaryReader);
				timeDelay = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVSceneNubber.bytes";
		}

		private static CSVSceneNubber instance = null;			
		public static CSVSceneNubber Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSceneNubber 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSceneNubber forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSceneNubber();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSceneNubber");

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
			shareData.ReadArrays<int>(binaryReader, 0, ReadHelper.ReadArray_ReadInt);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<uint>(binaryReader, 2, 1);

			return shareData;
		}
	}

#else

    sealed public partial class CSVSceneNubber : FCSVSceneNubber
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVSceneNubber.bytes";
		}

		private static CSVSceneNubber instance = null;			
		public static CSVSceneNubber Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSceneNubber 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSceneNubber forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSceneNubber();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSceneNubber");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}