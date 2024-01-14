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

	sealed public partial class CSVConditionalGift : Framework.Table.TableBase<CSVConditionalGift.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint TypeAchievement;
			public readonly List<uint> ReachTypeAchievement;
			public readonly uint Titleid;
			public readonly uint Text;
			public readonly uint Second;
			public readonly uint Reward;
			public readonly uint PriceType;
			public readonly uint Price;
			public readonly uint Charge;
			public bool Functionid { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly uint TimeCd;
			public readonly string Discount;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				TypeAchievement = ReadHelper.ReadUInt(binaryReader);
				ReachTypeAchievement = shareData.GetShareData<List<uint>>(binaryReader, 1);
				Titleid = ReadHelper.ReadUInt(binaryReader);
				Text = ReadHelper.ReadUInt(binaryReader);
				Second = ReadHelper.ReadUInt(binaryReader);
				Reward = ReadHelper.ReadUInt(binaryReader);
				PriceType = ReadHelper.ReadUInt(binaryReader);
				Price = ReadHelper.ReadUInt(binaryReader);
				Charge = ReadHelper.ReadUInt(binaryReader);
				TimeCd = ReadHelper.ReadUInt(binaryReader);
				Discount = shareData.GetShareData<string>(binaryReader, 0);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVConditionalGift.bytes";
		}

		private static CSVConditionalGift instance = null;			
		public static CSVConditionalGift Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVConditionalGift 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVConditionalGift forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVConditionalGift();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVConditionalGift");

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
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);

			return shareData;
		}
	}

#else

    sealed public partial class CSVConditionalGift : FCSVConditionalGift
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVConditionalGift.bytes";
		}

		private static CSVConditionalGift instance = null;			
		public static CSVConditionalGift Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVConditionalGift 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVConditionalGift forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVConditionalGift();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVConditionalGift");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}