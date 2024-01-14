using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Framework
{
    // ������ͨ���ж��ļ������һ��д��ʱ���ж��Ƿ��ⲿ��Ϊ���޸ģ����Ƿ��־�Ȼ������Ϊ���޸��ϴ�д��ʱ�䡣
    public class UnityFileVersionChecker
    {
        // һ����RoleId
        public string Key { get; private set; }

        // "{key}_TaskFinishListVersion"
        public string FileVersionFormat { get; private set; }

        // "{key}_TaskFinishList.txt"
        public string FilePathFormat { get; private set; }

        // "{key}_TaskFinishListMd5"
        public string FileMd5Format { get; private set; }

        public string FileVersionKey { get; private set; }
        public string FullPath { get; private set; }
        public string FileMd5Key { get; private set; }

        public int FileVersion { get; private set; }
        public string FileMd5 { get; private set; }

        public UnityFileVersionChecker() { }

        public UnityFileVersionChecker(string key, string fileVersionFormat, string filePathFormat, string fileMd5Format)
        {
            this.Reset(key, fileVersionFormat, filePathFormat, fileMd5Format);
        }

        public UnityFileVersionChecker Reset(string key, string fileVersionFormat, string filePathFormat, string fileMd5Format)
        {
            this.Key = key;
            this.FileVersionFormat = fileVersionFormat;
            this.FilePathFormat = filePathFormat;
            this.FileMd5Format = fileMd5Format;

            this.FileVersionKey = string.Format(this.FileVersionFormat, key);
            this.FullPath = string.Format(this.FilePathFormat, key);
            this.FileMd5Key = string.Format(this.FileMd5Format, key);
            return this;
        }

        // �����ļ�md5
        private bool TryLoad(out string md5)
        {
            this.FileVersion = PlayerPrefs.GetInt(this.FileVersionKey, -1);
            this.FileMd5 = PlayerPrefs.GetString(this.FileMd5Key, null);

            md5 = null;
            if (File.Exists(this.FullPath))
            {
                md5 = FrameworkTool.GetFileMD5(this.FullPath);
                return true;
            }

            return false;
        }

        // ��¼����ʱ�� �ж�local/remote�Լ��ļ��Ƿ���Ϊ�޸�
        // !fileExist || localVersion != remoteVersion || localMd5 != fileMd5, ����������������
        public bool TryRequest(int remoteFileVersion, Action requestAction, Action<string> onLoaded)
        {
            bool fileExist = this.TryLoad(out string newMd5);
            if (!fileExist || this.FileVersion != remoteFileVersion || this.FileMd5 != newMd5)
            {
                requestAction?.Invoke();
            }
            else
            {
                using (StreamReader sr = new StreamReader(this.FullPath, Encoding.UTF8))
                {
                    string content = sr.ReadToEnd();
                    onLoaded?.Invoke(content);
                }
            }
            return fileExist;
        }

        public void TrySave(int remoteVersion, Func<string> onStore)
        {
            if (!File.Exists(this.FullPath))
            {
                using (File.Create(this.FullPath))
                {
                    /*Let using dispose*/
                }
            }

            string content = onStore?.Invoke();
            using (StreamWriter sw = new StreamWriter(this.FullPath, false, Encoding.UTF8))
            {
                sw.Write(content);
            }

            this.FileVersion = remoteVersion;
            this.FileMd5 = FrameworkTool.GetFileMD5(this.FullPath);

            PlayerPrefs.SetInt(this.FileVersionKey, this.FileVersion);
            PlayerPrefs.SetString(this.FileMd5Key, this.FileMd5);
        }
    }
}