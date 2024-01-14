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

	sealed public partial class CSVDailyActivity : Framework.Table.TableBase<CSVDailyActivity.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint ActiveName;
			public readonly uint ActiveDes;
			public readonly uint OutputDisplay;
			public readonly uint AvtiveIcon;
			public readonly uint Active_lan;
			public readonly uint ActiveOrder;
			public readonly uint OpeningLevel;
			public readonly uint HideLevel;
			public readonly uint FunctionOpenid;
			public readonly uint OpeningImpose;
			public readonly uint limite;
			public readonly uint Times;
			public readonly List<uint> Play_Lv;
			public readonly List<uint> ActivityNum;
			public readonly List<uint> ActivityNumMax;
			public readonly uint OpeningMode1;
			public readonly List<uint> OpeningMode2;
			public readonly uint ActiveType;
			public readonly List<List<int>> OpeningTime;
			public readonly uint Duration;
			public readonly List<List<int>> NoticeTime;
			public readonly uint NoticeLong;
			public readonly uint Playing;
			public readonly uint ResetTime;
			public bool IsFamilyActive { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly uint Npcid;
			public readonly uint Uiid;
			public readonly uint UiidSonId;
			public readonly uint WayDesc;
			public readonly uint WayIcon;
			public readonly List<uint> Reward;
			public readonly List<uint> Reward_int;
			public readonly uint IsShow;
			public readonly uint special_des_lan;
			public readonly uint DragonRecommend;
			public readonly uint IsEasy;
			public readonly List<uint> ConcealOpeningMode2;
			public readonly List<List<int>> ConcealOpeningTime;
			public readonly uint ConcealDuration;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				ActiveName = ReadHelper.ReadUInt(binaryReader);
				ActiveDes = ReadHelper.ReadUInt(binaryReader);
				OutputDisplay = ReadHelper.ReadUInt(binaryReader);
				AvtiveIcon = ReadHelper.ReadUInt(binaryReader);
				Active_lan = ReadHelper.ReadUInt(binaryReader);
				ActiveOrder = ReadHelper.ReadUInt(binaryReader);
				OpeningLevel = ReadHelper.ReadUInt(binaryReader);
				HideLevel = ReadHelper.ReadUInt(binaryReader);
				FunctionOpenid = ReadHelper.ReadUInt(binaryReader);
				OpeningImpose = ReadHelper.ReadUInt(binaryReader);
				limite = ReadHelper.ReadUInt(binaryReader);
				Times = ReadHelper.ReadUInt(binaryReader);
				Play_Lv = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ActivityNum = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ActivityNumMax = shareData.GetShareData<List<uint>>(binaryReader, 0);
				OpeningMode1 = ReadHelper.ReadUInt(binaryReader);
				OpeningMode2 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ActiveType = ReadHelper.ReadUInt(binaryReader);
				OpeningTime = shareData.GetShareData<List<List<int>>>(binaryReader, 2);
				Duration = ReadHelper.ReadUInt(binaryReader);
				NoticeTime = shareData.GetShareData<List<List<int>>>(binaryReader, 2);
				NoticeLong = ReadHelper.ReadUInt(binaryReader);
				Playing = ReadHelper.ReadUInt(binaryReader);
				ResetTime = ReadHelper.ReadUInt(binaryReader);
				Npcid = ReadHelper.ReadUInt(binaryReader);
				Uiid = ReadHelper.ReadUInt(binaryReader);
				UiidSonId = ReadHelper.ReadUInt(binaryReader);
				WayDesc = ReadHelper.ReadUInt(binaryReader);
				WayIcon = ReadHelper.ReadUInt(binaryReader);
				Reward = shareData.GetShareData<List<uint>>(binaryReader, 0);
				Reward_int = shareData.GetShareData<List<uint>>(binaryReader, 0);
				IsShow = ReadHelper.ReadUInt(binaryReader);
				special_des_lan = ReadHelper.ReadUInt(binaryReader);
				DragonRecommend = ReadHelper.ReadUInt(binaryReader);
				IsEasy = ReadHelper.ReadUInt(binaryReader);
				ConcealOpeningMode2 = shareData.GetShareData<List<uint>>(binaryReader, 0);
				ConcealOpeningTime = shareData.GetShareData<List<List<int>>>(binaryReader, 2);
				ConcealDuration = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVDailyActivity.bytes";
		}

		private static CSVDailyActivity instance = null;			
		public static CSVDailyActivity Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVDailyActivity 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVDailyActivity forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVDailyActivity();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVDailyActivity");

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
			shareData.ReadArrays<uint>(binaryReader, 0, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArrays<int>(binaryReader, 1, ReadHelper.ReadArray_ReadInt);
			shareData.ReadArray2s<int>(binaryReader, 2, 1);

			return shareData;
		}
	}

#else

    sealed public partial class CSVDailyActivity : FCSVDailyActivity
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVDailyActivity.bytes";
		}

		private static CSVDailyActivity instance = null;			
		public static CSVDailyActivity Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVDailyActivity 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVDailyActivity forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVDailyActivity();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVDailyActivity");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}