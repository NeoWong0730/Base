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

	sealed public partial class CSVChargeList : Framework.Table.TableBase<CSVChargeList.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly string Ks_Apple;
			public readonly uint PlatformId;
			public readonly uint GoodsType;
			public readonly uint LanguageId;
			public readonly string Describe;
			public readonly uint RechargeType;
			public readonly uint RechargeCurrency;
			public readonly uint RechargeDiamond;
			public readonly uint ReturnMoneyId;
			public readonly uint ReturnMoneyNum;
			public readonly uint MoneyId;
			public readonly uint MoneyNum;
			public readonly uint MoneyItem;
			public readonly uint MoneyItemNum;
			public readonly List<List<uint>> MoneyCoin;
			public bool Is_Activate_Firstcharge { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public bool Is_Charge_Exp { get { return ReadHelper.GetBoolByIndex(boolArray0, 1); } }
			public readonly uint Charge_Exp;
			public readonly uint Act_Charge_Exp;
			public readonly string BackGround;
			public readonly uint CashCoupon;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Ks_Apple = shareData.GetShareData<string>(binaryReader, 0);
				PlatformId = ReadHelper.ReadUInt(binaryReader);
				GoodsType = ReadHelper.ReadUInt(binaryReader);
				LanguageId = ReadHelper.ReadUInt(binaryReader);
				Describe = shareData.GetShareData<string>(binaryReader, 0);
				RechargeType = ReadHelper.ReadUInt(binaryReader);
				RechargeCurrency = ReadHelper.ReadUInt(binaryReader);
				RechargeDiamond = ReadHelper.ReadUInt(binaryReader);
				ReturnMoneyId = ReadHelper.ReadUInt(binaryReader);
				ReturnMoneyNum = ReadHelper.ReadUInt(binaryReader);
				MoneyId = ReadHelper.ReadUInt(binaryReader);
				MoneyNum = ReadHelper.ReadUInt(binaryReader);
				MoneyItem = ReadHelper.ReadUInt(binaryReader);
				MoneyItemNum = ReadHelper.ReadUInt(binaryReader);
				MoneyCoin = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				Charge_Exp = ReadHelper.ReadUInt(binaryReader);
				Act_Charge_Exp = ReadHelper.ReadUInt(binaryReader);
				BackGround = shareData.GetShareData<string>(binaryReader, 0);
				CashCoupon = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVChargeList.bytes";
		}

		private static CSVChargeList instance = null;			
		public static CSVChargeList Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVChargeList 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVChargeList forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVChargeList();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVChargeList");

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

    sealed public partial class CSVChargeList : FCSVChargeList
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVChargeList.bytes";
		}

		private static CSVChargeList instance = null;			
		public static CSVChargeList Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVChargeList 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVChargeList forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVChargeList();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVChargeList");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}