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

	sealed public partial class CSVClassicBoss : Framework.Table.TableBase<CSVClassicBoss.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly byte UnlockType;
			public readonly uint CriminalID;
			public readonly uint TaskID;
			public readonly byte TaskLv;
			public readonly byte Lv;
			public readonly uint MapID;
			public readonly uint NPCID;
			public readonly uint Icon;
			public readonly int positionx;
			public readonly int positiony;
			public readonly int positionz;
			public readonly int rotationx;
			public readonly int rotationy;
			public readonly int rotationz;
			public readonly uint scale;
			public readonly uint IslandID;
			public readonly List<int> UIPosition;
			public readonly uint PreviousInformation;
			public readonly uint DropID;
			public readonly uint Navigation;
			public readonly uint TeamID;
			public readonly uint AutoVote;
			public readonly uint Score;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				UnlockType = ReadHelper.ReadByte(binaryReader);
				CriminalID = ReadHelper.ReadUInt(binaryReader);
				TaskID = ReadHelper.ReadUInt(binaryReader);
				TaskLv = ReadHelper.ReadByte(binaryReader);
				Lv = ReadHelper.ReadByte(binaryReader);
				MapID = ReadHelper.ReadUInt(binaryReader);
				NPCID = ReadHelper.ReadUInt(binaryReader);
				Icon = ReadHelper.ReadUInt(binaryReader);
				positionx = ReadHelper.ReadInt(binaryReader);
				positiony = ReadHelper.ReadInt(binaryReader);
				positionz = ReadHelper.ReadInt(binaryReader);
				rotationx = ReadHelper.ReadInt(binaryReader);
				rotationy = ReadHelper.ReadInt(binaryReader);
				rotationz = ReadHelper.ReadInt(binaryReader);
				scale = ReadHelper.ReadUInt(binaryReader);
				IslandID = ReadHelper.ReadUInt(binaryReader);
				UIPosition = shareData.GetShareData<List<int>>(binaryReader, 0);
				PreviousInformation = ReadHelper.ReadUInt(binaryReader);
				DropID = ReadHelper.ReadUInt(binaryReader);
				Navigation = ReadHelper.ReadUInt(binaryReader);
				TeamID = ReadHelper.ReadUInt(binaryReader);
				AutoVote = ReadHelper.ReadUInt(binaryReader);
				Score = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVClassicBoss.bytes";
		}

		private static CSVClassicBoss instance = null;			
		public static CSVClassicBoss Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVClassicBoss 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVClassicBoss forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVClassicBoss();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVClassicBoss");

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

    sealed public partial class CSVClassicBoss : FCSVClassicBoss
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVClassicBoss.bytes";
		}

		private static CSVClassicBoss instance = null;			
		public static CSVClassicBoss Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVClassicBoss 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVClassicBoss forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVClassicBoss();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVClassicBoss");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}