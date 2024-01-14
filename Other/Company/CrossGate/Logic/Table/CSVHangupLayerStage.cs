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

	sealed public partial class CSVHangupLayerStage : Framework.Table.TableBase<CSVHangupLayerStage.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Hangupid;
			public readonly byte LayerStage;
			public readonly byte ActivateType;
			public readonly List<uint> ActivatePara;
			public readonly uint Name;
			public readonly List<byte> RecommendLv;
			public readonly byte MonseterLv;
			public readonly uint Mapid;
			public readonly uint EnemyGroupid;
			public readonly uint TelNpcid;
			public readonly List<uint> MonseterIcon;
			public readonly uint Dropid;
			public readonly uint TeamID;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Hangupid = ReadHelper.ReadUInt(binaryReader);
				LayerStage = ReadHelper.ReadByte(binaryReader);
				ActivateType = ReadHelper.ReadByte(binaryReader);
				ActivatePara = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Name = ReadHelper.ReadUInt(binaryReader);
				RecommendLv = shareData.GetShareData<List<byte>>(binaryReader, 1);
				MonseterLv = ReadHelper.ReadByte(binaryReader);
				Mapid = ReadHelper.ReadUInt(binaryReader);
				EnemyGroupid = ReadHelper.ReadUInt(binaryReader);
				TelNpcid = ReadHelper.ReadUInt(binaryReader);
				MonseterIcon = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Dropid = ReadHelper.ReadUInt(binaryReader);
				TeamID = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVHangupLayerStage.bytes";
		}

		private static CSVHangupLayerStage instance = null;			
		public static CSVHangupLayerStage Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVHangupLayerStage 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVHangupLayerStage forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVHangupLayerStage();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVHangupLayerStage");

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
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArrays<byte>(binaryReader, 1, ReadHelper.ReadArray_ReadByte);

			return shareData;
		}
	}

#else

    sealed public partial class CSVHangupLayerStage : FCSVHangupLayerStage
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVHangupLayerStage.bytes";
		}

		private static CSVHangupLayerStage instance = null;			
		public static CSVHangupLayerStage Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVHangupLayerStage 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVHangupLayerStage forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVHangupLayerStage();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVHangupLayerStage");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}