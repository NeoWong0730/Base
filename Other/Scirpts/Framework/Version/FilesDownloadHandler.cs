using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Framework
{
    public class FilesDownloadHandler : DownloadHandlerScript
    {
        string m_SavePath;
        FileStream fs = null;
        public long StartFileLen { get; private set; }
        public long DownloadedFileLen { get; private set; }

        /// <summary>
        /// ��ʼ�����ؾ��������ÿ�����ص���������Ϊ200kb
        /// </summary>
        /// <param name="filePath">���浽���ص��ļ�·��</param>
        public FilesDownloadHandler(string filePath) : base(new byte[1024 * 200])
        {
            m_SavePath = filePath;
            fs = new FileStream(m_SavePath, FileMode.OpenOrCreate, FileAccess.Write);
            StartFileLen = fs.Length;
            fs?.Seek(StartFileLen, SeekOrigin.Begin);
        }


        /// <summary>
        /// �������ȡ����ʱ��Ļص���ÿ֡����һ��
        /// </summary>
        /// <param name="data">���յ��������ֽ������ܳ���Ϊ���캯�������200kb���������е����ݶ����µ�</param>
        /// <param name="dataLength">���յ������ݳ��ȣ���ʾdata�ֽ����������ж����������½��յ��ģ���0-dataLength֮��������Ǹս��յ���</param>
        /// <returns>����trueΪ�������أ�����falseΪ�ж�����</returns>
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




        // �ӷ������յ��������ݲ�ͨ�� ReceiveData ���ݺ���á�
        protected override void CompleteContent()
        {
            //Debug.Log("LoggingDownloadHandler :: CompleteContent - DOWNLOAD COMPLETE!");

            fs?.Close();
            fs?.Dispose();
        }



        // �ӷ������յ� Content-Length ��ͷʱ���á�
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
}