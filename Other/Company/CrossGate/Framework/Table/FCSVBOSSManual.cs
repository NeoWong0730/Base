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
    public class FCSVBOSSManual
    {
	    sealed public class Data
	    {
			public uint id { get; private set; }
			public uint camp_id { get; private set; }
			public uint head_icon { get; private set; }
			public uint headFrame_id { get; private set; }
			public int head_scale { get; private set; }
			public readonly List<float> head_postion;
			public uint manual_id { get; private set; }
			public uint BOSS_name { get; private set; }
			public uint BOSS_title { get; private set; }
			public uint detailPage_name { get; private set; }
			public uint detailPage_age { get; private set; }
			public uint detailPage_character { get; private set; }
			public uint detailPage_interests { get; private set; }
			public uint detailPage_weakness { get; private set; }
			public uint detailPage_skill { get; private set; }
			public uint detailPage_introduction { get; private set; }
			public int positionx { get; private set; }
			public int positiony { get; private set; }
			public int positionz { get; private set; }
			public int rotationx { get; private set; }
			public int rotationy { get; private set; }
			public int rotationz { get; private set; }
			public int scale { get; private set; }
			public uint BOSSUnlocked_drop { get; private set; }
			public uint BOSSFirstKilled_drop { get; private set; }
			public readonly List<uint> biography;
			public readonly List<uint> biography_drop;
			public uint unlockedLevel { get; private set; }
			public readonly List<uint> activatedNPC_id;
			public uint unlockedNotification { get; private set; }


            public Data(uint id, BinaryReader binaryReader, TableShareData shareData)
            {
                this.id = id;
				camp_id = ReadHelper.ReadUInt(binaryReader);
				head_icon = ReadHelper.ReadUInt(binaryReader);
				headFrame_id = ReadHelper.ReadUInt(binaryReader);
				head_scale = ReadHelper.ReadInt(binaryReader);
				head_postion = shareData.GetShareData<List<float>>(binaryReader, 0);
				manual_id = ReadHelper.ReadUInt(binaryReader);
				BOSS_name = ReadHelper.ReadUInt(binaryReader);
				BOSS_title = ReadHelper.ReadUInt(binaryReader);
				detailPage_name = ReadHelper.ReadUInt(binaryReader);
				detailPage_age = ReadHelper.ReadUInt(binaryReader);
				detailPage_character = ReadHelper.ReadUInt(binaryReader);
				detailPage_interests = ReadHelper.ReadUInt(binaryReader);
				detailPage_weakness = ReadHelper.ReadUInt(binaryReader);
				detailPage_skill = ReadHelper.ReadUInt(binaryReader);
				detailPage_introduction = ReadHelper.ReadUInt(binaryReader);
				positionx = ReadHelper.ReadInt(binaryReader);
				positiony = ReadHelper.ReadInt(binaryReader);
				positionz = ReadHelper.ReadInt(binaryReader);
				rotationx = ReadHelper.ReadInt(binaryReader);
				rotationy = ReadHelper.ReadInt(binaryReader);
				rotationz = ReadHelper.ReadInt(binaryReader);
				scale = ReadHelper.ReadInt(binaryReader);
				BOSSUnlocked_drop = ReadHelper.ReadUInt(binaryReader);
				BOSSFirstKilled_drop = ReadHelper.ReadUInt(binaryReader);
				biography = shareData.GetShareData<List<uint>>(binaryReader, 1);
				biography_drop = shareData.GetShareData<List<uint>>(binaryReader, 1);
				unlockedLevel = ReadHelper.ReadUInt(binaryReader);
				activatedNPC_id = shareData.GetShareData<List<uint>>(binaryReader, 1);
				unlockedNotification = ReadHelper.ReadUInt(binaryReader);

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
			shareData.ReadArrays<float>(binaryReader, 0, ReadHelper.ReadArray_ReadFloat);
			shareData.ReadArrays<uint>(binaryReader, 1, ReadHelper.ReadArray_ReadUInt);

            return shareData;
        }
    }

    public class FCSVBOSSManualAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FCSVBOSSManual);
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
    
        public class Adapter : FCSVBOSSManual, CrossBindingAdaptorType
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