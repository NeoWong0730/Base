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

	sealed public partial class CSVBattleMenuFunction : Framework.Table.TableBase<CSVBattleMenuFunction.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint lanId;
			public readonly uint iconId;
			public readonly uint functionId;
			public readonly uint typeId;
			public readonly int OrderId;
			public readonly int battleShow;
			public readonly int ResourcebattleShow;
			public readonly int ResourceMainShow;
			public readonly int isBattle;
			public readonly int isMain;
			public readonly int mainOrder;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				lanId = ReadHelper.ReadUInt(binaryReader);
				iconId = ReadHelper.ReadUInt(binaryReader);
				functionId = ReadHelper.ReadUInt(binaryReader);
				typeId = ReadHelper.ReadUInt(binaryReader);
				OrderId = ReadHelper.ReadInt(binaryReader);
				battleShow = ReadHelper.ReadInt(binaryReader);
				ResourcebattleShow = ReadHelper.ReadInt(binaryReader);
				ResourceMainShow = ReadHelper.ReadInt(binaryReader);
				isBattle = ReadHelper.ReadInt(binaryReader);
				isMain = ReadHelper.ReadInt(binaryReader);
				mainOrder = ReadHelper.ReadInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVBattleMenuFunction.bytes";
		}

		private static CSVBattleMenuFunction instance = null;			
		public static CSVBattleMenuFunction Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBattleMenuFunction 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBattleMenuFunction forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBattleMenuFunction();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBattleMenuFunction");

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

    sealed public partial class CSVBattleMenuFunction : FCSVBattleMenuFunction
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVBattleMenuFunction.bytes";
		}

		private static CSVBattleMenuFunction instance = null;			
		public static CSVBattleMenuFunction Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBattleMenuFunction 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBattleMenuFunction forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBattleMenuFunction();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBattleMenuFunction");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}