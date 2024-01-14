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

	sealed public partial class CSVCollectionProcess : Framework.Table.TableBase<CSVCollectionProcess.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly List<uint> collectionScheme;
			public readonly string collectingAction;
			public readonly uint collectingArms;
			public readonly uint collectionBirthEffect;
			public readonly uint collectionNoMayEffect;
			public readonly uint collectionMayEffect;
			public readonly uint collectingEffect;
			public readonly string collectingObjectAction;
			public readonly uint collectObjectEffect;
			public readonly uint collectedDialogue;
			public readonly string collectedAction;
			public readonly uint collectedEffect;
			public readonly string collectObjectHpAction;
			public readonly uint collectObjectHpEffect;
			public readonly string collectObjectDeathAction;
			public readonly uint collectObjectDeathEffect;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				collectionScheme = shareData.GetShareData<List<uint>>(binaryReader, 1);
				collectingAction = shareData.GetShareData<string>(binaryReader, 0);
				collectingArms = ReadHelper.ReadUInt(binaryReader);
				collectionBirthEffect = ReadHelper.ReadUInt(binaryReader);
				collectionNoMayEffect = ReadHelper.ReadUInt(binaryReader);
				collectionMayEffect = ReadHelper.ReadUInt(binaryReader);
				collectingEffect = ReadHelper.ReadUInt(binaryReader);
				collectingObjectAction = shareData.GetShareData<string>(binaryReader, 0);
				collectObjectEffect = ReadHelper.ReadUInt(binaryReader);
				collectedDialogue = ReadHelper.ReadUInt(binaryReader);
				collectedAction = shareData.GetShareData<string>(binaryReader, 0);
				collectedEffect = ReadHelper.ReadUInt(binaryReader);
				collectObjectHpAction = shareData.GetShareData<string>(binaryReader, 0);
				collectObjectHpEffect = ReadHelper.ReadUInt(binaryReader);
				collectObjectDeathAction = shareData.GetShareData<string>(binaryReader, 0);
				collectObjectDeathEffect = ReadHelper.ReadUInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVCollectionProcess.bytes";
		}

		private static CSVCollectionProcess instance = null;			
		public static CSVCollectionProcess Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCollectionProcess 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCollectionProcess forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCollectionProcess();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCollectionProcess");

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

    sealed public partial class CSVCollectionProcess : FCSVCollectionProcess
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVCollectionProcess.bytes";
		}

		private static CSVCollectionProcess instance = null;			
		public static CSVCollectionProcess Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVCollectionProcess 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVCollectionProcess forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVCollectionProcess();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVCollectionProcess");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}