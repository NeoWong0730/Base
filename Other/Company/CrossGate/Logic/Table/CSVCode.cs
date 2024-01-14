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

	sealed public partial class CSVCode : Framework.Table.TableBase<CSVCode.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint condition;
			public readonly uint CodeOptions;
			public readonly uint CodeDialogue;
			public readonly uint CorrectCode;
			public readonly uint CorrectDialogue;
			public readonly byte CodeResults;
			public readonly List<uint> CodeResultsID;
			public readonly uint WrongCode;
			public readonly List<List<uint>> ClueDialogue;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				condition = ReadHelper.ReadUInt(binaryReader);
				CodeOptions = ReadHelper.ReadUInt(binaryReader);
				CodeDialogue = ReadHelper.ReadUInt(binaryReader);
				CorrectCode = ReadHelper.ReadUInt(binaryReader);
				CorrectDialogue = ReadHelper.ReadUInt(binaryReader);
				CodeResults = ReadHelper.ReadByte(binaryReader);
				CodeResultsID = shareData.GetShareData<List<uint>>(binaryReader, 0);
				WrongCode = ReadHelper.ReadUInt(binaryReader);
				ClueDialogue = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVCode.bytes";
		}

		private static CSVCode instance = null;			
		public static CSVCode Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCode 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCode forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCode();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCode");

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

    sealed public partial class CSVCode : FCSVCode
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVCode.bytes";
		}

		private static CSVCode instance = null;			
		public static CSVCode Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCode 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCode forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCode();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCode");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}