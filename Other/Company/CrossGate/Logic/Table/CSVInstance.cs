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

	sealed public partial class CSVInstance : Framework.Table.TableBase<CSVInstance.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Type;
			public readonly uint TeamUp;
			public readonly uint Name;
			public readonly uint PlayType;
			public readonly uint pre_instance;
			public readonly List<uint> lv;
			public readonly uint limite_number;
			public readonly List<List<uint>> Ticket;
			public readonly uint FirstReward;
			public readonly uint Des;
			public readonly uint PreviousDes;
			public readonly uint FirstPassReward;
			public readonly string bg;
			public readonly uint DeductionLayers;
			public readonly uint AutoVote;
			public readonly int RecommondPoint;
			public readonly uint TeamID;
			public readonly uint DifficultyName;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Type = ReadHelper.ReadUInt(binaryReader);
				TeamUp = ReadHelper.ReadUInt(binaryReader);
				Name = ReadHelper.ReadUInt(binaryReader);
				PlayType = ReadHelper.ReadUInt(binaryReader);
				pre_instance = ReadHelper.ReadUInt(binaryReader);
				lv = shareData.GetShareData<List<uint>>(binaryReader, 1);
				limite_number = ReadHelper.ReadUInt(binaryReader);
				Ticket = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				FirstReward = ReadHelper.ReadUInt(binaryReader);
				Des = ReadHelper.ReadUInt(binaryReader);
				PreviousDes = ReadHelper.ReadUInt(binaryReader);
				FirstPassReward = ReadHelper.ReadUInt(binaryReader);
				bg = shareData.GetShareData<string>(binaryReader, 0);
				DeductionLayers = ReadHelper.ReadUInt(binaryReader);
				AutoVote = ReadHelper.ReadUInt(binaryReader);
				RecommondPoint = ReadHelper.ReadInt(binaryReader);
				TeamID = ReadHelper.ReadUInt(binaryReader);
				DifficultyName = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVInstance.bytes";
		}

		private static CSVInstance instance = null;			
		public static CSVInstance Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVInstance 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVInstance forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVInstance();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVInstance");

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
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<uint>(binaryReader, 2, 1);

			return shareData;
		}
	}

#else

    sealed public partial class CSVInstance : FCSVInstance
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVInstance.bytes";
		}

		private static CSVInstance instance = null;			
		public static CSVInstance Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVInstance 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVInstance forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVInstance();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVInstance");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}