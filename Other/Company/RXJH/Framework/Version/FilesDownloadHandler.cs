using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class FilesDownloadHandler : DownloadHandlerScript
{
    string m_SavePath;
    FileStream fs = null;
    public long StartFileLen { get; private set; }
    public long DownloadedFileLen { get; private set; }

    /// <summary>
    /// 初始化下载句柄，定义每次下载的数据上限为200kb
    /// </summary>
    /// <param name="filePath">保存到本地的文件路径</param>
    public FilesDownloadHandler(string filePath) : base(new byte[1024 * 200])
    {
        m_SavePath = filePath;
        fs = new FileStream(m_SavePath, FileMode.OpenOrCreate, FileAccess.Write);
        StartFileLen = fs.Length;
        fs?.Seek(StartFileLen, SeekOrigin.Begin);
    }


    /// <summary>
    /// 从网络获取数据时候的回调，每帧调用一次
    /// </summary>
    /// <param name="data">接收到的数据字节流，总长度为构造函数定义的200kb，并非所有的数据都是新的</param>
    /// <param name="dataLength">接收到的数据长度，表示data字节流数组中有多少数据是新接收到的，即0-dataLength之间的数据是刚接收到的</param>
    /// <returns>返回true为继续下载，返回false为中断下载</returns>
    protected override bool ReceiveData(byte[] data, int dataLength)
    {
        if (data == null || data.Length < 1)
        {
            Debug.Log("LoggingDownloadHandler :: ReceiveData - received a null/empty buffer");
            return false;
        }

        fs?.Write(data, 0, dataLength);
        DownloadedFileLen += dataLength;
        //Debug.Log(string.Format("LoggingDownloadHandler :: ReceiveData - received {0} bytes", dataLength));
        return true;
    }




    // 从服务器收到所有数据并通过 ReceiveData 传递后调用。
    protected override void CompleteContent()
    {
        //Debug.Log("LoggingDownloadHandler :: CompleteContent - DOWNLOAD COMPLETE!");

        fs?.Close();
        fs?.Dispose();
    }



    // 从服务器收到 Content-Length 标头时调用。
    protected override void ReceiveContentLengthHeader(ulong contentLength)
    {
        //Debug.Log(string.Format("LoggingDownloadHandler :: ReceiveContentLength - length {0}", contentLength));
    }

    public void ErrorDispose(bool reset)
    {
        //Debug.Log("ErrorDispose" + dispose);
        if (reset)
        {
            fs?.SetLength(StartFileLen);
            fs?.Close();
            fs?.Dispose();
            Dispose();
        }
        else
        {
            fs?.Close();
            fs?.Dispose();
            Dispose();
        }
    }
}