using Lib.AssetLoader;
using Lib.Core;
using System.Collections.Generic;
using System.IO;

using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections;
using UnityEngine;

namespace Framework.Table
{
    public class FCSVCareer
    {
	    sealed public class Data
	    {
			public uint id { get; private set; }
			public uint name { get; private set; }
			public readonly List<uint> desc;
			public uint icon { get; private set; }
			public uint select_icon { get; private set; }
			public uint team_icon { get; private set; }
			public uint logo_icon { get; private set; }
			public uint pvp_icon { get; private set; }
			public readonly string profession_icon1;
			public readonly string profession_icon2;
			public uint open { get; private set; }
			public uint career { get; private set; }
			public readonly string model;
			public uint weapon { get; private set; }
			public uint test_battleid { get; private set; }
			public readonly List<uint> inti_skill;
			public readonly List<List<uint>> battle_skill;
			public readonly List<uint> passive_skill_show;
			public readonly List<uint> proud_initial;
			public readonly List<uint> currency_initial;
			public readonly List<uint> recommend_skill;
			public readonly List<List<uint>> proud_skill;
			public readonly List<List<uint>> currency_skill;
			public readonly List<List<uint>> learnable_skill;
			public uint recommend { get; private set; }
			public readonly List<List<uint>> add_point;
			public readonly List<uint> Capability_value;


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				name = ReadHelper.ReadUInt(binaryReader);
				desc = shareData.GetShareData<List<uint>>(binaryReader, 1);
				icon = ReadHelper.ReadUInt(binaryReader);
				select_icon = ReadHelper.ReadUInt(binaryReader);
				team_icon = ReadHelper.ReadUInt(binaryReader);
				logo_icon = ReadHelper.ReadUInt(binaryReader);
				pvp_icon = ReadHelper.ReadUInt(binaryReader);
				profession_icon1 = shareData.GetShareData<string>(binaryReader, 0);
				profession_icon2 = shareData.GetShareData<string>(binaryReader, 0);
				open = ReadHelper.ReadUInt(binaryReader);
				career = ReadHelper.ReadUInt(binaryReader);
				model = shareData.GetShareData<string>(binaryReader, 0);
				weapon = ReadHelper.ReadUInt(binaryReader);
				test_battleid = ReadHelper.ReadUInt(binaryReader);
				inti_skill = shareData.GetShareData<List<uint>>(binaryReader, 1);
				battle_skill = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				passive_skill_show = shareData.GetShareData<List<uint>>(binaryReader, 1);
				proud_initial = shareData.GetShareData<List<uint>>(binaryReader, 1);
				currency_initial = shareData.GetShareData<List<uint>>(binaryReader, 1);
				recommend_skill = shareData.GetShareData<List<uint>>(binaryReader, 1);
				proud_skill = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				currency_skill = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				learnable_skill = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				recommend = ReadHelper.ReadUInt(binaryReader);
				add_point = shareData.GetShareData<List<List<uint>>>(binaryReader, 2);
				Capability_value = shareData.GetShareData<List<uint>>(binaryReader, 1);

            }
	    }	

        protected Dictionary<uint, int> Datas;
        protected List<Data> DataList;

        public int Count { get { return DataList.Count; } }

        public void ReadByFilePath(string path)
        {            
            Stream stream = AssetMananger.Instance.LoadStream(path);
            BinaryReader binaryReader = new BinaryReader(stream);
            Read(path, binaryReader);
            binaryReader.Close();
            stream.Close();
        }

        public void Read(string path, BinaryReader binaryReader)
        {
            if (binaryReader == null)
            {
                DebugUtil.LogErrorFormat("{0} binaryReader为空", path);
                return;
            }

            if (Datas != null)
            {
                DebugUtil.LogErrorFormat("{0} 多次读取配置", path);
                return;
            }

            TableShareData shareData = OnReadShareData(binaryReader);

            //buff的实际长度
            int bufferSize = binaryReader.ReadInt32();

            //读取内容
            int count = binaryReader.ReadInt32();

            Datas = new Dictionary<uint, int>(count);
            DataList = new List<Data>(count);

            if (count <= 0)
            {
                DebugUtil.LogErrorFormat("{0} 是空表", path);
                return;
            }

            int entrySize = bufferSize / count;

            if (entrySize * count != bufferSize)
            {
                DebugUtil.LogErrorFormat("{0} 数据长度异常", path);
                return;
            }

            for (int i = 0; i < count; ++i)
            {
#if DEBUG_MODE
                try
                {                                        
                    uint id = ReadHelper.ReadUInt(binaryReader);
                    Data data = new Data(id, binaryReader, shareData);
                    if (Datas.ContainsKey(id))
                    {
                        DebugUtil.LogErrorFormat("表格 {1} 有重复id{0}", id.ToString(), path);
                    }
                    Datas[id] = i;
                    DataList.Add(data);
                }
                catch (System.Exception e)
                {
                    DebugUtil.LogErrorFormat("表格 {2} 第 {0} 行错误： {1}", (i + 1).ToString(), e.StackTrace, path);
                    break;
                }
#else
                uint id = ReadHelper.ReadUInt(binaryReader);
                Data data = new Data(id, binaryReader, shareData);
                Datas[id] = i;
                DataList.Add(data);
#endif
            }
        }

        public Data GetConfData(uint id)
        {
            if (Datas.TryGetValue(id, out int index))
                return GetByIndex(index);
            return null;
        }

        public Data GetByIndex(int index)
        {
            Data tableData = DataList[index];
            return tableData;
        }

        public bool TryGetValue(uint id, out Data data)
        {
            if (Datas.TryGetValue(id, out int index))
            {
                data = GetByIndex(index);
                return true;
            }

            data = null;
            return false;
        }

        public IReadOnlyList<Data> GetAll()
        {
            return DataList;
        }

        public Dictionary<uint, int>.KeyCollection GetKeys()
        {
            return Datas.Keys;
        }

        public bool ContainsKey(uint id)
        {
            return Datas.ContainsKey(id);
        }

        public void Clear()
        {
            Datas?.Clear();
            DataList?.Clear();            
        }

        public static TableShareData OnReadShareData(BinaryReader binaryReader)
        {
			TableShareData shareData = new TableShareData(3);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);
			shareData.ReadArray2s<uint>(binaryReader, 2, 1);

            return shareData;
        }
    }

    public class FCSVCareerAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FCSVCareer);
            }
        }
    
        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }
    
        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }
    
        public class Adapter : FCSVCareer, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;
    
            public Adapter()
            {
            
            }
    
            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }
    
            public ILTypeInstance ILInstance { get { return instance; } }
    
            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    return instance.ToString();
                }
                else
                    return instance.Type.FullName;
            }
        }
    }
}