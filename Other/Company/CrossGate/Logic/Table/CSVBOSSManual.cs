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

	sealed public partial class CSVBOSSManual : Framework.Table.TableBase<CSVBOSSManual.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint camp_id;
			public readonly uint head_icon;
			public readonly uint headFrame_id;
			public readonly int head_scale;
			public readonly List<float> head_postion;
			public readonly uint manual_id;
			public readonly uint BOSS_name;
			public readonly uint BOSS_title;
			public readonly uint detailPage_name;
			public readonly uint detailPage_age;
			public readonly uint detailPage_character;
			public readonly uint detailPage_interests;
			public readonly uint detailPage_weakness;
			public readonly uint detailPage_skill;
			public readonly uint detailPage_introduction;
			public readonly int positionx;
			public readonly int positiony;
			public readonly int positionz;
			public readonly int rotationx;
			public readonly int rotationy;
			public readonly int rotationz;
			public readonly int scale;
			public readonly uint BOSSUnlocked_drop;
			public readonly uint BOSSFirstKilled_drop;
			public readonly List<uint> biography;
			public readonly List<uint> biography_drop;
			public readonly uint unlockedLevel;
			public readonly List<uint> activatedNPC_id;
			public readonly uint unlockedNotification;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				camp_id = ReadHelper.ReadUInt(binaryReader);
				head_icon = ReadHelper.ReadUInt(binaryReader);
				headFrame_id = ReadHelper.ReadUInt(binaryReader);
				head_scale = ReadHelper.ReadInt(binaryReader);
				head_postion = shareData.GetShareData<List<float>>(binaryReader, 0);
				manual_id = ReadHelper.ReadUInt(binaryReader);
				BOSS_name = ReadHelper.ReadUInt(binaryReader);
				BOSS_title = ReadHelper.ReadUInt(binaryReader);
				detailPage_name = ReadHelper.ReadUInt(binaryReader);
				detailPage_age = ReadHelper.ReadUInt(binaryReader);
				detailPage_character = ReadHelper.ReadUInt(binaryReader);
				detailPage_interests = ReadHelper.ReadUInt(binaryReader);
				detailPage_weakness = ReadHelper.ReadUInt(binaryReader);
				detailPage_skill = ReadHelper.ReadUInt(binaryReader);
				detailPage_introduction = ReadHelper.ReadUInt(binaryReader);
				positionx = ReadHelper.ReadInt(binaryReader);
				positiony = ReadHelper.ReadInt(binaryReader);
				positionz = ReadHelper.ReadInt(binaryReader);
				rotationx = ReadHelper.ReadInt(binaryReader);
				rotationy = ReadHelper.ReadInt(binaryReader);
				rotationz = ReadHelper.ReadInt(binaryReader);
				scale = ReadHelper.ReadInt(binaryReader);
				BOSSUnlocked_drop = ReadHelper.ReadUInt(binaryReader);
				BOSSFirstKilled_drop = ReadHelper.ReadUInt(binaryReader);
				biography = shareData.GetShareData<List<uint>>(binaryReader, 1);
				biography_drop = shareData.GetShareData<List<uint>>(binaryReader, 1);
				unlockedLevel = ReadHelper.ReadUInt(binaryReader);
				activatedNPC_id = shareData.GetShareData<List<uint>>(binaryReader, 1);
				unlockedNotification = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVBOSSManual.bytes";
		}

		private static CSVBOSSManual instance = null;			
		public static CSVBOSSManual Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBOSSManual 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBOSSManual forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBOSSManual();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBOSSManual");

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
			shareData.ReadArrays<float>(binaryReader, 0, ReadHelper.ReadArray_ReadFloat);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVBOSSManual : FCSVBOSSManual
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVBOSSManual.bytes";
		}

		private static CSVBOSSManual instance = null;			
		public static CSVBOSSManual Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBOSSManual 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBOSSManual forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBOSSManual();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBOSSManual");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}