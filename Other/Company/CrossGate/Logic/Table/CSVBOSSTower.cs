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

	sealed public partial class CSVBOSSTower : Framework.Table.TableBase<CSVBOSSTower.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint name;
			public readonly uint bossName;
			public readonly uint levelGrade_id;
			public readonly uint order;
			public readonly uint npc_id;
			public readonly uint map_id;
			public readonly string towerPicture;
			public readonly string bg;
			public readonly List<uint> text_id;
			public readonly List<uint> textDetails_id;
			public readonly List<uint> team_id;
			public readonly string model_show;
			public readonly uint action_show_id;
			public readonly int positionx;
			public readonly int positiony;
			public readonly int positionz;
			public readonly int rotationx;
			public readonly int rotationy;
			public readonly int rotationz;
			public readonly int scale;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				name = ReadHelper.ReadUInt(binaryReader);
				bossName = ReadHelper.ReadUInt(binaryReader);
				levelGrade_id = ReadHelper.ReadUInt(binaryReader);
				order = ReadHelper.ReadUInt(binaryReader);
				npc_id = ReadHelper.ReadUInt(binaryReader);
				map_id = ReadHelper.ReadUInt(binaryReader);
				towerPicture = shareData.GetShareData<string>(binaryReader, 0);
				bg = shareData.GetShareData<string>(binaryReader, 0);
				text_id = shareData.GetShareData<List<uint>>(binaryReader, 1);
				textDetails_id = shareData.GetShareData<List<uint>>(binaryReader, 1);
				team_id = shareData.GetShareData<List<uint>>(binaryReader, 1);
				model_show = shareData.GetShareData<string>(binaryReader, 0);
				action_show_id = ReadHelper.ReadUInt(binaryReader);
				positionx = ReadHelper.ReadInt(binaryReader);
				positiony = ReadHelper.ReadInt(binaryReader);
				positionz = ReadHelper.ReadInt(binaryReader);
				rotationx = ReadHelper.ReadInt(binaryReader);
				rotationy = ReadHelper.ReadInt(binaryReader);
				rotationz = ReadHelper.ReadInt(binaryReader);
				scale = ReadHelper.ReadInt(binaryReader);

            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVBOSSTower.bytes";
		}

		private static CSVBOSSTower instance = null;			
		public static CSVBOSSTower Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBOSSTower 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBOSSTower forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBOSSTower();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBOSSTower");

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

    sealed public partial class CSVBOSSTower : FCSVBOSSTower
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVBOSSTower.bytes";
		}

		private static CSVBOSSTower instance = null;			
		public static CSVBOSSTower Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBOSSTower 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBOSSTower forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBOSSTower();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBOSSTower");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}