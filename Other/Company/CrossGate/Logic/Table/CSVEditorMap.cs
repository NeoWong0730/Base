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

	sealed public partial class CSVEditorMap : Framework.Table.TableBase<CSVEditorMap.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint width;
			public readonly uint height;
			public readonly List<uint> telIds;
			public readonly List<uint> telPosXs;
			public readonly List<uint> telPosYs;
			public readonly List<float> telOffXs;
			public readonly List<float> telOffYs;
			public readonly List<uint> telRangeXs;
			public readonly List<uint> telRangeYs;
			public readonly List<uint> telCondIds;
			public readonly List<uint> npcIds;
			public readonly List<List<uint>> npcPosXs;
			public readonly List<List<uint>> npcPosYs;
			public readonly List<uint> npcWidths;
			public readonly List<uint> npcHeights;
			public readonly List<int> npcOffXs;
			public readonly List<int> npcOffYs;
			public readonly List<int> npcRotaXs;
			public readonly List<int> npcRotaYs;
			public readonly List<int> npcRotaZs;
			public readonly List<uint> monstergpIds;
			public readonly List<List<uint>> monstergpPosXs;
			public readonly List<List<uint>> monstergpPosYs;
			public readonly List<List<uint>> FightSafeAreaSize;
			public readonly List<List<uint>> FightSafeAreaOffset;
			public readonly List<uint> FightSafeAreaCamp;
			public readonly List<List<uint>> FightSafeAreaPos;
			public readonly List<List<uint>> FightBlockSize;
			public readonly List<List<uint>> FightBlockOffset;
			public readonly List<uint> FightBlockCamp;
			public readonly List<List<uint>> FightBlockPos;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				width = ReadHelper.ReadUInt(binaryReader);
				height = ReadHelper.ReadUInt(binaryReader);
				telIds = shareData.GetShareData<List<uint>>(binaryReader, 0);
				telPosXs = shareData.GetShareData<List<uint>>(binaryReader, 0);
				telPosYs = shareData.GetShareData<List<uint>>(binaryReader, 0);
				telOffXs = shareData.GetShareData<List<float>>(binaryReader, 1);
				telOffYs = shareData.GetShareData<List<float>>(binaryReader, 1);
				telRangeXs = shareData.GetShareData<List<uint>>(binaryReader, 0);
				telRangeYs = shareData.GetShareData<List<uint>>(binaryReader, 0);
				telCondIds = shareData.GetShareData<List<uint>>(binaryReader, 0);
				npcIds = shareData.GetShareData<List<uint>>(binaryReader, 0);
				npcPosXs = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				npcPosYs = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				npcWidths = shareData.GetShareData<List<uint>>(binaryReader, 0);
				npcHeights = shareData.GetShareData<List<uint>>(binaryReader, 0);
				npcOffXs = shareData.GetShareData<List<int>>(binaryReader, 2);
				npcOffYs = shareData.GetShareData<List<int>>(binaryReader, 2);
				npcRotaXs = shareData.GetShareData<List<int>>(binaryReader, 2);
				npcRotaYs = shareData.GetShareData<List<int>>(binaryReader, 2);
				npcRotaZs = shareData.GetShareData<List<int>>(binaryReader, 2);
				monstergpIds = shareData.GetShareData<List<uint>>(binaryReader, 0);
				monstergpPosXs = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				monstergpPosYs = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				FightSafeAreaSize = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				FightSafeAreaOffset = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				FightSafeAreaCamp = shareData.GetShareData<List<uint>>(binaryReader, 0);
				FightSafeAreaPos = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				FightBlockSize = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				FightBlockOffset = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);
				FightBlockCamp = shareData.GetShareData<List<uint>>(binaryReader, 0);
				FightBlockPos = shareData.GetShareData<List<List<uint>>>(binaryReader, 3);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVEditorMap.bytes";
		}

		private static CSVEditorMap instance = null;			
		public static CSVEditorMap Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVEditorMap 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVEditorMap forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVEditorMap();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVEditorMap");

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
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArrays<float>(binaryReader, 1, ReadHelper.ReadArray_ReadFloat);
			shareData.ReadArrays<int>(binaryReader, 2, ReadHelper.ReadArray_ReadInt);
			shareData.ReadArray2s<uint>(binaryReader, 3, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVEditorMap : FCSVEditorMap
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVEditorMap.bytes";
		}

		private static CSVEditorMap instance = null;			
		public static CSVEditorMap Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVEditorMap 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVEditorMap forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVEditorMap();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVEditorMap");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}