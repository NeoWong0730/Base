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

	sealed public partial class CSVMonthCard : Framework.Table.TableBase<CSVMonthCard.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Change_Id;
			public readonly uint First_Change_Id;
			public readonly uint Present_Change_Id;
			public readonly uint Continue_Day;
			public readonly uint Pirviege_Title;
			public readonly uint Return_Display;
			public readonly List<List<uint>> First_Pirviege_Des;
			public readonly List<List<uint>> Pirviege_Des;
			public readonly uint Pet_Extra_Seal_successrate;
			public readonly uint Extra_Deal;
			public readonly uint Extra_Singin;
			public readonly List<uint> Extra_Giftbag;
			public bool Offline_onhook { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly uint World_statement;
			public bool With_bank { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
			public readonly uint pre_order_time;
			public bool pocket_kitchen { get { return ReadHelper.GetBoolByIndex(boolArray0, 2); } }
			public bool give_Pirviege { get { return ReadHelper.GetBoolByIndex(boolArray0, 3); } }
			public readonly string Show_Icon;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Change_Id = ReadHelper.ReadUInt(binaryReader);
				First_Change_Id = ReadHelper.ReadUInt(binaryReader);
				Present_Change_Id = ReadHelper.ReadUInt(binaryReader);
				Continue_Day = ReadHelper.ReadUInt(binaryReader);
				Pirviege_Title = ReadHelper.ReadUInt(binaryReader);
				Return_Display = ReadHelper.ReadUInt(binaryReader);
				First_Pirviege_Des = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				Pirviege_Des = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				Pet_Extra_Seal_successrate = ReadHelper.ReadUInt(binaryReader);
				Extra_Deal = ReadHelper.ReadUInt(binaryReader);
				Extra_Singin = ReadHelper.ReadUInt(binaryReader);
				Extra_Giftbag = shareData.GetShareData<List<uint>>(binaryReader, 1);
				World_statement = ReadHelper.ReadUInt(binaryReader);
				pre_order_time = ReadHelper.ReadUInt(binaryReader);
				Show_Icon = shareData.GetShareData<string>(binaryReader, 0);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVMonthCard.bytes";
		}

		private static CSVMonthCard instance = null;			
		public static CSVMonthCard Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVMonthCard 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVMonthCard forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVMonthCard();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVMonthCard");

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

    sealed public partial class CSVMonthCard : FCSVMonthCard
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVMonthCard.bytes";
		}

		private static CSVMonthCard instance = null;			
		public static CSVMonthCard Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVMonthCard 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVMonthCard forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVMonthCard();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVMonthCard");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}