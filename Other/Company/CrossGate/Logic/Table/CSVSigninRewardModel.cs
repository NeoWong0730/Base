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

	sealed public partial class CSVSigninRewardModel : Framework.Table.TableBase<CSVSigninRewardModel.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly string model;
			public readonly uint action_id;
			public readonly uint weapon_id;
			public readonly int positionx;
			public readonly int positiony;
			public readonly int positionz;
			public readonly int rotationx;
			public readonly int rotationy;
			public readonly int rotationz;
			public readonly uint scale;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				model = shareData.GetShareData<string>(binaryReader, 0);
				action_id = ReadHelper.ReadUInt(binaryReader);
				weapon_id = ReadHelper.ReadUInt(binaryReader);
				positionx = ReadHelper.ReadInt(binaryReader);
				positiony = ReadHelper.ReadInt(binaryReader);
				positionz = ReadHelper.ReadInt(binaryReader);
				rotationx = ReadHelper.ReadInt(binaryReader);
				rotationy = ReadHelper.ReadInt(binaryReader);
				rotationz = ReadHelper.ReadInt(binaryReader);
				scale = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVSigninRewardModel.bytes";
		}

		private static CSVSigninRewardModel instance = null;			
		public static CSVSigninRewardModel Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSigninRewardModel 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSigninRewardModel forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSigninRewardModel();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSigninRewardModel");

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
			shareData.ReadStrings(binaryReader, 0);

			return shareData;
		}
	}

#else

    sealed public partial class CSVSigninRewardModel : FCSVSigninRewardModel
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVSigninRewardModel.bytes";
		}

		private static CSVSigninRewardModel instance = null;			
		public static CSVSigninRewardModel Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVSigninRewardModel 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVSigninRewardModel forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVSigninRewardModel();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVSigninRewardModel");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}