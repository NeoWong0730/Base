using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System;
using static FileDataOperationManager;
using Lib.Core;

public class FileDataOperationWriteEntity : AEntity
{
    private Dictionary<Type, FileDataObjTypeInfo> _fileDataObjTypeInfoDic = new Dictionary<Type, FileDataObjTypeInfo>();

    private MemoryStream _stream;
    private BinaryWriter _binaryWriter;

    private string _filePath;

    public void DoWriter<T>(string filePath, T obj, Action<BinaryWriter> beginAction = null, bool isSetCapacity = false)
    {
        if (string.IsNullOrEmpty(filePath) || obj == null)
            return;

        using (MemoryStream stream = new MemoryStream())
        {
            using (BinaryWriter binaryWriter = new BinaryWriter(stream))
            {
                beginAction?.Invoke(binaryWriter);

                WriteData(binaryWriter, obj, typeof(T));

                byte[] writeBs = null;
                if (isSetCapacity)
                {
                    byte[] bs = stream.GetBuffer();
                    writeBs = new byte[stream.Length];
                    Buffer.BlockCopy(bs, 0, writeBs, 0, writeBs.Length);
                }
                else
                    writeBs = stream.GetBuffer();

                File.WriteAllBytes(filePath, writeBs);
            }
        }
    }

    #region 读取多个数据，要配套使用
    public void StartWriter(string filePath, Action<MemoryStream, BinaryWriter> startAction = null, int streamCapacity = 0)
    {
        if (string.IsNullOrEmpty(filePath))
            return;

        _filePath = filePath;

        _stream = streamCapacity <= 0 ? new MemoryStream() : new MemoryStream(streamCapacity);

        _binaryWriter = new BinaryWriter(_stream);

        startAction?.Invoke(_stream, _binaryWriter);
    }
    
    public void AddWriterData<T>(T obj, Action<MemoryStream, BinaryWriter> beginAction = null, Action<MemoryStream, BinaryWriter> endAction = null)
    {
        beginAction?.Invoke(_stream, _binaryWriter);

        WriteData(_binaryWriter, obj, typeof(T));

        endAction?.Invoke(_stream, _binaryWriter);
    }

    public void AddWriterData(byte[] bs, Action<BinaryWriter> beginAction = null)
    {
        if (bs == null || bs.Length == 0)
            return;

        beginAction?.Invoke(_binaryWriter);

        _binaryWriter.Write(bs);
    }

    public void EndWrite(Action<MemoryStream, BinaryWriter> beginAction = null, bool isSetCapacity = false)
    {
        if (_stream != null)
        {
            if (_binaryWriter != null)
                beginAction?.Invoke(_stream, _binaryWriter);

            byte[] writeBs = null;
            if (isSetCapacity)
            {
                byte[] bs = _stream.GetBuffer();
                writeBs = new byte[_stream.Length];
                Buffer.BlockCopy(bs, 0, writeBs, 0, writeBs.Length);
            }
            else
                writeBs = _stream.GetBuffer();

            File.WriteAllBytes(_filePath, writeBs);
        }

        _binaryWriter?.Close();
        _stream?.Close();

        _binaryWriter = null;
        _stream = null;
        _filePath = null;
    }
    #endregion

    #region 数据写入BinaryWriter
    public void WriteData(BinaryWriter binaryWriter, object val, Type objType)
    {
        if (objType.IsArray)
        {
            SetArray(binaryWriter, val, objType);
        }
        else if (objType.IsGenericType)
        {
            SetGeneric(binaryWriter, val, objType);
        }
        else if (objType.IsClass && objType.Name != CombatHelp.s_String)
        {
            SetClass(binaryWriter, val, objType);
        }
        else if (objType.IsValueType && !objType.IsEnum && !objType.IsPrimitive)  //struct
        {
            SetStruct(binaryWriter, objType.Name, val);
        }
        else
        {
            CombatHelp.WriteValueByType(binaryWriter, objType.Name, val);
        }
    }

    private void SetArray(BinaryWriter binaryWriter, object obj, Type objType)
    {
        binaryWriter.Write(obj == null ? false : true);

        if (obj == null)
            return;

        Type arrayType = objType.GetElementType();

        Array array = (Array)obj;

        int listCount = array.Length;

        ushort len = (ushort)listCount;
        binaryWriter.Write(len);
        if (len > 0)
        {
            for (int i = 0; i < listCount; i++)
            {
                WriteData(binaryWriter, array.GetValue(i), arrayType);
            }
        }
    }

    private void SetGeneric(BinaryWriter binaryWriter, object obj, Type objType)
    {
        binaryWriter.Write(obj != null);

        if (obj == null)
            return;

        Type genericType = objType.GetGenericTypeDefinition();
        if (genericType == typeof(List<>))
        {
            IList list = (IList)obj;

            int listCount = list.Count;

            ushort len = (ushort)listCount;
            binaryWriter.Write(len);
            if (len > 0)
            {
                Type genericArgumentType = objType.GetGenericArguments()[0];
                for (int i = 0; i < listCount; i++)
                {
                    WriteData(binaryWriter, list[i], genericArgumentType);
                }
            }
        }
    }

    private void SetClass(BinaryWriter binaryWriter, object obj, Type objType)
    {
        binaryWriter.Write(obj != null);

        if (objType.IsClass && obj == null)
            return;

        FileDataObjTypeInfo fileDataObjTypeInfo = FileDataOperationManager.GetSortFieldInfos(obj.GetType(), _fileDataObjTypeInfoDic);
        foreach (FieldInfo fieldInfo in fileDataObjTypeInfo.FieldInfos)
        {
            if (fieldInfo == null)
                continue;

            WriteData(binaryWriter, fieldInfo.GetValue(obj), fieldInfo.FieldType);
        }
    }

    private void SetStruct(BinaryWriter binaryWriter, string typeName, object val)
    {
        if (typeName == typeof(UnityEngine.Vector3).Name)
        {
            UnityEngine.Vector3 vector3 = (UnityEngine.Vector3)val;
            binaryWriter.Write(vector3.x);
            binaryWriter.Write(vector3.y);
            binaryWriter.Write(vector3.z);
        }
        else
        {
            DebugUtil.LogError($"暂时不支持读取类型：{typeName}");
        }
    }
    #endregion
}