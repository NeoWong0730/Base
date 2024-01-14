#define CSV_VERSION_1_5

using Lib;
using System.Collections.Generic;
using System.IO;

namespace Framework
{
    public class TableShareData
    {
        private object[] shareDatas;
        public TableShareData(int count)
        {
            shareDatas = new object[count];
        }

        public void ReadStrings(BinaryReader binaryReader, int writeIndex)
        {
            int count = binaryReader.ReadInt32();
            string[] data = new string[count];

            for (int i = 0; i < count; ++i)
            {
                data[i] = ReadHelper.ReadString(binaryReader);
            }

            shareDatas[writeIndex] = data;
        }

        public void ReadStringArrays(BinaryReader binaryReader, int writeIndex, int shareDataIndex)
        {
            int count = binaryReader.ReadInt32();

            List<string>[] data = new List<string>[count];
            string[] shareData = (string[])(shareDatas[shareDataIndex]);

            for (int i = 0; i < count; ++i)
            {
                ushort arrayCount = binaryReader.ReadUInt16();

                if (arrayCount > 0)
                {
                    List<string> arr = new List<string>(arrayCount);

                    for (int j = 0; j < arrayCount; ++j)
                    {
                        int index = ReadHelper.ReadInt(binaryReader);
#if DEBUG_MODE
                        if (index >= shareData.Length)
                        {
                            DebugUtil.LogErrorFormat("{0} {1} {2}", i, index, shareData.Length);
                            arr.Add(null);
                            continue;
                        }
#endif
                        arr.Add(shareData[index]);
                    }

                    data[i] = arr;
                }
                else
                {
                    data[i] = null;
                }
            }

            shareDatas[writeIndex] = data;
        }

        public void ReadArrays<T>(BinaryReader binaryReader, int writeIndex, ReadHelper.ReadFunc<List<T>> func)
        {
            int count = binaryReader.ReadInt32();
            List<T>[] data = new List<T>[count];

            for (int i = 0; i < count; ++i)
            {
                data[i] = func(binaryReader);
            }

            shareDatas[writeIndex] = data;
        }

        public void ReadArray2s<T>(BinaryReader binaryReader, int writeIndex, int shareDataIndex)
        {
            int count = binaryReader.ReadInt32();

            List<List<T>>[] data = new List<List<T>>[count];
            List<T>[] shareData = (List<T>[])(shareDatas[shareDataIndex]);

            for (int i = 0; i < count; ++i)
            {
                ushort arrayCount = binaryReader.ReadUInt16();

                if (arrayCount > 0)
                {
                    List<List<T>> arr = new List<List<T>>(arrayCount);

                    for (int j = 0; j < arrayCount; ++j)
                    {
                        int index = ReadHelper.ReadInt(binaryReader);

#if DEBUG_MODE
                        if (index >= shareData.Length)
                        {
                            DebugUtil.LogErrorFormat("{0} {1} {2}", i, index, shareData.Length);
                            arr.Add(null);
                            continue;
                        }
#endif

                        arr.Add(shareData[index]);
                    }

                    data[i] = arr;
                }
                else
                {
                    data[i] = null;
                }
            }

            shareDatas[writeIndex] = data;
        }

        public T GetShareData<T>(BinaryReader binaryReader, int shareDataIndex)
        {
            int index = binaryReader.ReadInt32();
            T[] shareData = (T[])(shareDatas[shareDataIndex]);

#if DEBUG_MODE
            if (index >= shareData.Length)
            {
                DebugUtil.LogErrorFormat("{0} {1}", index, shareData.Length);
                return default(T);
            }
#endif
            return shareData[index];
        }
    }
    
    public static class TableDebugUtil
    {
#if UNITY_EDITOR
        public static HashSet<string> gClassNames;        
        public static bool bCollectGetAll = false;
#endif
        
        public static void BegineCollectGetAll()
        {
#if UNITY_EDITOR
            gClassNames = new HashSet<string>();
            bCollectGetAll = true;
#endif
        }

        public static HashSet<string> EndCollectGetAll()
        {
#if UNITY_EDITOR
            bCollectGetAll = false;
            HashSet<string> tmp = gClassNames;
            gClassNames = null;
            return tmp;
#else
            return null;
#endif
        }
    }

#if CSV_VERSION_1_5
    public class TableBase<TTableData> where TTableData : class
    {
        protected Dictionary<uint, int> Datas;
        protected List<TTableData> DataList;
        protected TableShareData _shareData;
        protected BinaryReader _binaryReader;
        protected int _entrySize;
        protected bool _allLoaded = false;
        protected int _loadCount = 0;

        private System.Func<uint, BinaryReader, TableShareData, TTableData> _onCreate;

        public int Count { get { return DataList.Count; } }

        public void ReadByFilePath(string path, System.Func<uint, BinaryReader, TableShareData, TTableData> creator, System.Func<BinaryReader, TableShareData> readShareData)
        {
            _onCreate = creator;

            Stream stream = AssetMananger.Instance.LoadStream(path);
            BinaryReader binaryReader = new BinaryReader(stream);
            Read(path, binaryReader, creator, readShareData);
            binaryReader.Close();
            stream.Close();
        }

        public void Read(string path, BinaryReader binaryReader, System.Func<uint, BinaryReader, TableShareData, TTableData> creator, System.Func<BinaryReader, TableShareData> readShareData)
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

            _shareData = readShareData(binaryReader);

            //buff的实际长度
            int bufferSize = binaryReader.ReadInt32();

            //读取内容
            int count = binaryReader.ReadInt32();

            Datas = new Dictionary<uint, int>(count);
            DataList = new List<TTableData>(count);

            if (count <= 0)
            {
                DebugUtil.LogErrorFormat("{0} 是空表", path);
                return;
            }

            _entrySize = bufferSize / count;

            if (_entrySize * count != bufferSize)
            {
                DebugUtil.LogErrorFormat("{0} 数据长度异常", path);
                return;
            }

            byte[] buffer = binaryReader.ReadBytes(bufferSize);
            _binaryReader = new BinaryReader(new MemoryStream(buffer));

            for (int i = 0; i < count; ++i)
            {
#if DEBUG_MODE
                try
                {
                    //_binaryReader.BaseStream.Seek(_entrySize * i, SeekOrigin.Begin);
                    uint id = System.BitConverter.ToUInt32(buffer, _entrySize * i); //ReadHelper.ReadUInt(_binaryReader);//
                    if (Datas.ContainsKey(id))
                    {
                        DebugUtil.LogErrorFormat("表格 {1} 有重复id{0}", id.ToString(), path);
                    }
                    Datas[id] = i;
                    DataList.Add(null);
                }
                catch (System.Exception e)
                {
                    DebugUtil.LogErrorFormat("表格 {2} 第 {0} 行错误： {1}", (i + 1).ToString(), e.StackTrace, path);
                    break;
                }
#else
                //_binaryReader.BaseStream.Seek(_entrySize * i, SeekOrigin.Begin);
                uint id = System.BitConverter.ToUInt32(buffer, _entrySize * i); //ReadHelper.ReadUInt(_binaryReader);//
                Datas[id] = i;
                DataList.Add(null);
#endif
            }
        }

        public TTableData GetConfData(uint id)
        {
            if (Datas.TryGetValue(id, out int index))
                return GetByIndex(index);
            return null;
        }

        public TTableData GetByIndex(int index)
        {
            TTableData tableData = DataList[index];
            if (!_allLoaded && tableData == null)
            {
                _binaryReader.BaseStream.Seek(_entrySize * index, SeekOrigin.Begin);
                tableData = _onCreate(ReadHelper.ReadUInt(_binaryReader), _binaryReader, _shareData);
                if(tableData != null)
                {
                    ++_loadCount;
                    if (_loadCount >= Count)
                    {
                        _allLoaded = true;
                        _shareData = null;
                        _binaryReader?.Close();
                        _binaryReader = null;
                    }
                }
                DataList[index] = tableData;
            }
            return tableData;
        }

        public bool TryGetValue(uint id, out TTableData data)
        {
            if (Datas.TryGetValue(id, out int index))
            {
                data = GetByIndex(index);
                return true;
            }

            data = null;
            return false;
        }

        public IReadOnlyList<TTableData> GetAll()
        {
            if (!_allLoaded)
            {
                for (int i = 0, len = Count; i < len; ++i)
                {
                    GetByIndex(i);
                }
                _allLoaded = true;
                _shareData = null;
                _binaryReader?.Close();
                _binaryReader = null;

#if UNITY_EDITOR
                if (TableDebugUtil.bCollectGetAll)
                {
                    string s = ToString();
                    if (!TableDebugUtil.gClassNames.Contains(s))
                    {
                        TableDebugUtil.gClassNames.Add(s);
                    }
                }
#endif
            }

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
            _shareData = null;
            _binaryReader?.Close();
            _binaryReader = null;
            _entrySize = 0;
            _allLoaded = false;
            _onCreate = null;
            _loadCount = 0;
        }

    }
#elif CSV_VERSION_1_4
    public class TableBase<TTableData>
    {
        protected Dictionary<uint, TTableData> Datas;
        protected List<TTableData> DataList;

        public int Count { get; private set; }
        public void ReadByFilePath(string path, System.Func<uint, BinaryReader, TableShareData, TTableData> creator, System.Func<BinaryReader, TableShareData> readShareData)
        {
            Stream stream = AssetMananger.Instance.LoadStream(path);
            BinaryReader binaryReader = new BinaryReader(stream);
            Read(path, binaryReader, creator, readShareData);           
            binaryReader.Close();            
            stream.Close();
        }

        public void Read(string path, BinaryReader binaryReader, System.Func<uint, BinaryReader, TableShareData, TTableData> creator, System.Func<BinaryReader, TableShareData> readShareData)
        {
            if (binaryReader == null)
                return;

            if (Datas != null)
            {
                DebugUtil.LogErrorFormat("{0} 多次读取配置", path);
                return;
            }

            TableShareData shareData = readShareData(binaryReader);

            //buff的实际长度
            int dataSize = binaryReader.ReadInt32();

            //读取内容
            int count = binaryReader.ReadInt32();
            Datas = new Dictionary<uint, TTableData>(count);
            DataList = new List<TTableData>(count);

            //byte[] buffs = binaryReader.ReadBytes(dataSize);
            //int size = dataSize / count;
            
            for (int i = 0; i < count; ++i)
            {
#if DEBUG_MODE
                try
                {
                    uint id = ReadHelper.ReadUInt(binaryReader);
                    TTableData data = creator(id, binaryReader, shareData);
                    if (Datas.ContainsKey(id))
                    {
                        DebugUtil.LogErrorFormat("表格 {1} 有重复id{0}", id.ToString(), path);
                    }
                    Datas[id] = data;
                    DataList.Add(data);
                }
                catch (System.Exception e)
                {
                    DebugUtil.LogErrorFormat("表格 {2} 第 {0} 行错误： {1}", (i + 1).ToString(), e.StackTrace, path);
                    break;
                }
#else
                uint id = ReadHelper.ReadUInt(binaryReader);
                TTableData data = creator(id, binaryReader, shareData);
                Datas[id] = data;
                DataList.Add(data);
#endif
            }

            Count = DataList.Count;
            shareData = null;
        }

        public TTableData GetConfData(uint id)
        {
            Datas.TryGetValue(id, out TTableData data);
            return data;
        }

        public TTableData GetByIndex(int index)
        {
            TTableData tableData = DataList[index];
            return tableData;
        }

        public bool TryGetValue(uint id, out TTableData data)
        {
            return Datas.TryGetValue(id, out data);
        }

        public bool ContainsKey(uint id)
        {
            return Datas.ContainsKey(id);
        }

        public IReadOnlyList<TTableData> GetAll()
        {
            return DataList;
        }

        public Dictionary<uint, TTableData>.KeyCollection GetKeys()
        {
            return Datas.Keys;
        }

        public void Clear()
        {
            Datas.Clear();
            DataList.Clear();
        }
    }
#else

    public class TableBase<TTableData>
    {
        protected Dictionary<uint, TTableData> Datas;
        protected TTableData[] DataList;

        public TTableData this[int index]
        {
            get
            {
                return DataList[index];
            }
        }

        public int Count { get; private set; }

        public void ReadByFilePath(string path, System.Func<uint, BinaryReader, string[], TTableData> creator)
        {
            Stream stream = AssetMananger.Instance.LoadStream(path);
            BinaryReader binaryReader = new BinaryReader(stream);
            Read(path, binaryReader, creator);
            binaryReader.Dispose();
            binaryReader.Close();
            stream.Dispose();
            stream.Close();
        }

        public void Read(string path, BinaryReader binaryReader, System.Func<uint, BinaryReader, string[], TTableData> creator)
        {
            if (binaryReader == null)
                return;

            if (Datas != null)
            {
                DebugUtil.LogErrorFormat("{0} 多次读取配置", path);
                return;
            }

            //先读取所有的字符串
            string[] strings = null;
            int stringCount = binaryReader.ReadInt32();
            if (stringCount > 0)
            {
                strings = new string[stringCount];
                for (int i = 0; i < stringCount; ++i)
                {
                    strings[i] = ReadHelper.ReadString(binaryReader);
                }
            }

            //读取内容
            int count = binaryReader.ReadInt32();
            Datas = new Dictionary<uint, TTableData>(count);
            DataList = new TTableData[count];

            for (int i = 0; i < count; ++i)
            {
#if DEBUG_MODE
                try
                {                    
                    uint id = ReadHelper.ReadUInt(binaryReader); 
                    TTableData data = creator(id, binaryReader, strings);
                    if (Datas.ContainsKey(id))
                    {
                        DebugUtil.LogErrorFormat("表格 {1} 有重复id{0}", id.ToString(), path);
                    }
                    Datas[id] = data;
                    DataList[i] = data;
                }
                catch (System.Exception e)
                {
                    DebugUtil.LogErrorFormat("表格 {2} 第 {0} 行错误： {1}", (i + 1).ToString(), e.StackTrace, path);
                    break;
                }
#else
                uint id = ReadHelper.ReadUInt(binaryReader);
                TTableData data = creator(id, binaryReader, strings);
                Datas[id] = data;
                DataList[i] = data;
#endif
            }

            Count = DataList.Length;
            strings = null;
        }

        public TTableData GetConfData(uint id)
        {
            Datas.TryGetValue(id, out TTableData data);
            return data;
        }

        public bool ContainsKey(uint id)
        {
            return Datas.ContainsKey(id);
        }

        public bool TryGetValue(uint id, out TTableData data)
        {
            return Datas.TryGetValue(id, out data);
        }

        [System.Obsolete("USE []")]
        public Dictionary<uint, TTableData> GetDictData()
        {
            return Datas;
        }

        [System.Obsolete("USE []")]
        public Dictionary<uint, TTableData>.Enumerator GetEnumerator()
        {
            return Datas.GetEnumerator();
        }

        public void Clear()
        {
            Datas.Clear();
        }
    }
#endif
        }