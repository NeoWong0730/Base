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

	sealed public partial class CSVFamilyAuction : Framework.Table.TableBase<CSVFamilyAuction.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint ItemId;
			public readonly uint ItemNum;
			public bool IsSplit { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly uint GroupNum;
			public readonly uint AuctionCurrency;
			public readonly uint InitialCurrency;
			public readonly uint InitialSingleStep;
			public readonly uint InitialIsBind;
			public readonly uint InitialParameter;
			public readonly uint FixedCurrencyType;
			public readonly uint FixedCurrency;
			public readonly uint FixedIsBind;
			public readonly uint FixedParameter;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				ItemId = ReadHelper.ReadUInt(binaryReader);
				ItemNum = ReadHelper.ReadUInt(binaryReader);
				GroupNum = ReadHelper.ReadUInt(binaryReader);
				AuctionCurrency = ReadHelper.ReadUInt(binaryReader);
				InitialCurrency = ReadHelper.ReadUInt(binaryReader);
				InitialSingleStep = ReadHelper.ReadUInt(binaryReader);
				InitialIsBind = ReadHelper.ReadUInt(binaryReader);
				InitialParameter = ReadHelper.ReadUInt(binaryReader);
				FixedCurrencyType = ReadHelper.ReadUInt(binaryReader);
				FixedCurrency = ReadHelper.ReadUInt(binaryReader);
				FixedIsBind = ReadHelper.ReadUInt(binaryReader);
				FixedParameter = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyAuction.bytes";
		}

		private static CSVFamilyAuction instance = null;			
		public static CSVFamilyAuction Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyAuction 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyAuction forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyAuction();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyAuction");

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
			TableShareData shareData = null;

			return shareData;
		}
	}

#else

    sealed public partial class CSVFamilyAuction : FCSVFamilyAuction
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFamilyAuction.bytes";
		}

		private static CSVFamilyAuction instance = null;			
		public static CSVFamilyAuction Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFamilyAuction 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFamilyAuction forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFamilyAuction();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFamilyAuction");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}