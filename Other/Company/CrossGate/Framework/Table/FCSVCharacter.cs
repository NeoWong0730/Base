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
    public class FCSVCharacter
    {
	    sealed public class Data
	    {
			public uint id { get; private set; }
			public uint active { get; private set; }
			public uint name { get; private set; }
			public uint desc { get; private set; }
			public uint sex { get; private set; }
			public uint headid { get; private set; }
			public uint name_icon { get; private set; }
			public readonly string model;
			public uint create_char_audio { get; private set; }
			public readonly string create_char_timeline;
			public readonly string model_show;
			public uint show_weapon_id { get; private set; }
			public uint inti_attr { get; private set; }
			public uint inti_level { get; private set; }
			public uint create { get; private set; }
			public readonly List<uint> auto_battle;
			public int LeftLocationX { get; private set; }
			public int LeftLocationY { get; private set; }
			public int LeftLocationZ { get; private set; }
			public int LeftLocationRotateX { get; private set; }
			public int LeftLocationRotateY { get; private set; }
			public int LeftLocationRotateZ { get; private set; }
			public int LeftLocationMirrorImage { get; private set; }
			public int RightLocationX { get; private set; }
			public int RightLocationY { get; private set; }
			public int RightLocationZ { get; private set; }
			public int RightLocationRotateX { get; private set; }
			public int RightLocationRotateY { get; private set; }
			public int RightLocationRotateZ { get; private set; }
			public int RightLocationMirrorImage { get; private set; }
			public readonly List<uint> signPositionShifting;
			public int BubbleLocationX { get; private set; }
			public int BubbleLocationY { get; private set; }
			public int BubbleLocationZ { get; private set; }
			public int BubbleLocationRotateX { get; private set; }
			public int BubbleLocationRotateY { get; private set; }
			public int BubbleLocationRotateZ { get; private set; }
			public int BubbleLocationMirrorImage { get; private set; }


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				active = ReadHelper.ReadUInt(binaryReader);
				name = ReadHelper.ReadUInt(binaryReader);
				desc = ReadHelper.ReadUInt(binaryReader);
				sex = ReadHelper.ReadUInt(binaryReader);
				headid = ReadHelper.ReadUInt(binaryReader);
				name_icon = ReadHelper.ReadUInt(binaryReader);
				model = shareData.GetShareData<string>(binaryReader, 0);
				create_char_audio = ReadHelper.ReadUInt(binaryReader);
				create_char_timeline = shareData.GetShareData<string>(binaryReader, 0);
				model_show = shareData.GetShareData<string>(binaryReader, 0);
				show_weapon_id = ReadHelper.ReadUInt(binaryReader);
				inti_attr = ReadHelper.ReadUInt(binaryReader);
				inti_level = ReadHelper.ReadUInt(binaryReader);
				create = ReadHelper.ReadUInt(binaryReader);
				auto_battle = shareData.GetShareData<List<uint>>(binaryReader, 1);
				LeftLocationX = ReadHelper.ReadInt(binaryReader);
				LeftLocationY = ReadHelper.ReadInt(binaryReader);
				LeftLocationZ = ReadHelper.ReadInt(binaryReader);
				LeftLocationRotateX = ReadHelper.ReadInt(binaryReader);
				LeftLocationRotateY = ReadHelper.ReadInt(binaryReader);
				LeftLocationRotateZ = ReadHelper.ReadInt(binaryReader);
				LeftLocationMirrorImage = ReadHelper.ReadInt(binaryReader);
				RightLocationX = ReadHelper.ReadInt(binaryReader);
				RightLocationY = ReadHelper.ReadInt(binaryReader);
				RightLocationZ = ReadHelper.ReadInt(binaryReader);
				RightLocationRotateX = ReadHelper.ReadInt(binaryReader);
				RightLocationRotateY = ReadHelper.ReadInt(binaryReader);
				RightLocationRotateZ = ReadHelper.ReadInt(binaryReader);
				RightLocationMirrorImage = ReadHelper.ReadInt(binaryReader);
				signPositionShifting = shareData.GetShareData<List<uint>>(binaryReader, 1);
				BubbleLocationX = ReadHelper.ReadInt(binaryReader);
				BubbleLocationY = ReadHelper.ReadInt(binaryReader);
				BubbleLocationZ = ReadHelper.ReadInt(binaryReader);
				BubbleLocationRotateX = ReadHelper.ReadInt(binaryReader);
				BubbleLocationRotateY = ReadHelper.ReadInt(binaryReader);
				BubbleLocationRotateZ = ReadHelper.ReadInt(binaryReader);
				BubbleLocationMirrorImage = ReadHelper.ReadInt(binaryReader);

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
			TableShareData shareData = new TableShareData(2);
			shareData.ReadStrings(binaryReader, 0);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);

            return shareData;
        }
    }

    public class FCSVCharacterAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FCSVCharacter);
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
    
        public class Adapter : FCSVCharacter, CrossBindingAdaptorType
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