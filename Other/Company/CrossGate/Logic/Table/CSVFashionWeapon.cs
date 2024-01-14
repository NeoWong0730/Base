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

	sealed public partial class CSVFashionWeapon : Framework.Table.TableBase<CSVFashionWeapon.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint WeaponName;
			public readonly uint WeaponDescribe;
			public readonly uint WeaponQuality;
			public readonly List<uint> FashionItem;
			public readonly List<List<uint>> WeaponColour;
			public readonly List<List<uint>> itemNeed;
			public readonly uint ColorSwitch;
			public readonly List<List<uint>> itemNeed1;
			public readonly List<uint> Tag;
			public readonly uint FashionScore;
			public bool Hide { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly uint Lock;
			public readonly uint LimitedTime;
			public readonly uint MaxColour;
			public readonly uint suitId;
			public readonly List<List<uint>> attr_id;
			public readonly uint score;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				WeaponName = ReadHelper.ReadUInt(binaryReader);
				WeaponDescribe = ReadHelper.ReadUInt(binaryReader);
				WeaponQuality = ReadHelper.ReadUInt(binaryReader);
				FashionItem = shareData.GetShareData<List<uint>>(binaryReader, 0);
				WeaponColour = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				itemNeed = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				ColorSwitch = ReadHelper.ReadUInt(binaryReader);
				itemNeed1 = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				Tag = shareData.GetShareData<List<uint>>(binaryReader, 0);
				FashionScore = ReadHelper.ReadUInt(binaryReader);
				Lock = ReadHelper.ReadUInt(binaryReader);
				LimitedTime = ReadHelper.ReadUInt(binaryReader);
				MaxColour = ReadHelper.ReadUInt(binaryReader);
				suitId = ReadHelper.ReadUInt(binaryReader);
				attr_id = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				score = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVFashionWeapon.bytes";
		}

		private static CSVFashionWeapon instance = null;			
		public static CSVFashionWeapon Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFashionWeapon 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFashionWeapon forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFashionWeapon();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFashionWeapon");

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
			shareData.ReadArray2s<uint>(binaryReader, 1, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVFashionWeapon : FCSVFashionWeapon
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVFashionWeapon.bytes";
		}

		private static CSVFashionWeapon instance = null;			
		public static CSVFashionWeapon Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVFashionWeapon 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVFashionWeapon forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVFashionWeapon();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVFashionWeapon");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}