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

	sealed public partial class CSVBattlePassRewardDisplay : Framework.Table.TableBase<CSVBattlePassRewardDisplay.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint Job_Id;
			public readonly List<string> Show_Item;
			public readonly List<uint> Hero_Id;
			public readonly uint Fashionid;
			public readonly uint Reward_Name;
			public readonly List<string> Show_Id;
			public readonly List<string> Show_height;
			public readonly List<uint> Item_Size;
			public readonly List<List<string>> spin_coordinate;
			public readonly uint spin_speed;
			public readonly uint spin_axle;
			public readonly List<uint> Ornaments_Id;
			public readonly uint Weapon_Id;
			public readonly uint equip_id;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				Job_Id = ReadHelper.ReadUInt(binaryReader);
				Show_Item = shareData.GetShareData<List<string>>(binaryReader, 1);
				Hero_Id = shareData.GetShareData<List<uint>>(binaryReader, 2);
				Fashionid = ReadHelper.ReadUInt(binaryReader);
				Reward_Name = ReadHelper.ReadUInt(binaryReader);
				Show_Id = shareData.GetShareData<List<string>>(binaryReader, 1);
				Show_height = shareData.GetShareData<List<string>>(binaryReader, 1);
				Item_Size = shareData.GetShareData<List<uint>>(binaryReader, 2);
				spin_coordinate = shareData.GetShareData<List<List<string>>>(binaryReader, 3);
				spin_speed = ReadHelper.ReadUInt(binaryReader);
				spin_axle = ReadHelper.ReadUInt(binaryReader);
				Ornaments_Id = shareData.GetShareData<List<uint>>(binaryReader, 2);
				Weapon_Id = ReadHelper.ReadUInt(binaryReader);
				equip_id = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVBattlePassRewardDisplay.bytes";
		}

		private static CSVBattlePassRewardDisplay instance = null;			
		public static CSVBattlePassRewardDisplay Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBattlePassRewardDisplay 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBattlePassRewardDisplay forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBattlePassRewardDisplay();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBattlePassRewardDisplay");

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
			TableShareData shareData = new TableShareData(4);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadStringArrays(binaryReader, 1, 0);
			shareData.ReadArrays<uint>(binaryReader, 2, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<string>(binaryReader, 3, 1);

			return shareData;
		}
	}

#else

    sealed public partial class CSVBattlePassRewardDisplay : FCSVBattlePassRewardDisplay
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVBattlePassRewardDisplay.bytes";
		}

		private static CSVBattlePassRewardDisplay instance = null;			
		public static CSVBattlePassRewardDisplay Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBattlePassRewardDisplay 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBattlePassRewardDisplay forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBattlePassRewardDisplay();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBattlePassRewardDisplay");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}