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

	sealed public partial class CSVBuff : Framework.Table.TableBase<CSVBuff.Data>
	{
	    sealed public class Data
	    {
			public readonly uint id;
			public readonly uint success_probability;
			public readonly uint effect_buff;
			public readonly int buff_effective;
			public readonly uint buff_state;
			public readonly uint is_show;
			public readonly uint icon;
			public readonly uint hud_icon;
			public bool show_buff_duration { get { return ReadHelper.GetBoolByIndex(boolArray0, 0); } }
			public readonly uint name;
			public readonly uint desc;
			public readonly uint duration_type;
			public readonly uint buff_type;
			public readonly List<List<uint>> effective_attr;
			public readonly uint clear_trigger;
			public readonly uint effect_parma;
			public readonly uint effect_parma1;
			public readonly uint effectAdd;
			public readonly uint effectProc;
			public readonly uint effecDel;
			public readonly uint skill_id;
			public readonly uint hp_skill_id;
			public readonly uint ui_param1;
			public readonly uint ui_param2;
			public readonly uint hud_icon_num;
		private readonly byte boolArray0;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				success_probability = ReadHelper.ReadUInt(binaryReader);
				effect_buff = ReadHelper.ReadUInt(binaryReader);
				buff_effective = ReadHelper.ReadInt(binaryReader);
				buff_state = ReadHelper.ReadUInt(binaryReader);
				is_show = ReadHelper.ReadUInt(binaryReader);
				icon = ReadHelper.ReadUInt(binaryReader);
				hud_icon = ReadHelper.ReadUInt(binaryReader);
				name = ReadHelper.ReadUInt(binaryReader);
				desc = ReadHelper.ReadUInt(binaryReader);
				duration_type = ReadHelper.ReadUInt(binaryReader);
				buff_type = ReadHelper.ReadUInt(binaryReader);
				effective_attr = shareData.GetShareData<List<List<uint>>>(binaryReader, 1);
				clear_trigger = ReadHelper.ReadUInt(binaryReader);
				effect_parma = ReadHelper.ReadUInt(binaryReader);
				effect_parma1 = ReadHelper.ReadUInt(binaryReader);
				effectAdd = ReadHelper.ReadUInt(binaryReader);
				effectProc = ReadHelper.ReadUInt(binaryReader);
				effecDel = ReadHelper.ReadUInt(binaryReader);
				skill_id = ReadHelper.ReadUInt(binaryReader);
				hp_skill_id = ReadHelper.ReadUInt(binaryReader);
				ui_param1 = ReadHelper.ReadUInt(binaryReader);
				ui_param2 = ReadHelper.ReadUInt(binaryReader);
				hud_icon_num = ReadHelper.ReadUInt(binaryReader);

			boolArray0 = ReadHelper.ReadByte(binaryReader);
            }
	    }	

		public static string ConfigPath()
		{
			return "Config/Table/CSVBuff.bytes";
		}

		private static CSVBuff instance = null;			
		public static CSVBuff Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBuff 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBuff forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBuff();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBuff");

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

    sealed public partial class CSVBuff : FCSVBuff
    {
        public static string ConfigPath()
		{
			return "Config/Table/CSVBuff.bytes";
		}

		private static CSVBuff instance = null;			
		public static CSVBuff Instance
		{
			get
			{
#if DEBUG_MODE
				if (instance == null)
				{
					DebugUtil.LogError("配置表CSVBuff 尚未构建实例, 请添加配置表加载到 CSVRegister");
				}
#endif
				return instance;
			}
		}

        public static void Load(bool forceReload = false)
        {
			DebugUtil.LogFormat(ELogType.eTable, "加载CSVBuff forceReload = {0}", forceReload);

            if (instance == null)
            {
                instance = new CSVBuff();
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
            DebugUtil.Log(ELogType.eTable, "卸载CSVBuff");

            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }
    }
    
#endif
}