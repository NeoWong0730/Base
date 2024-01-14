using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Framework
{
    // 本来像通过判断文件的最后一次写入时间判断是否被外部人为的修改，但是发现居然可以人为的修改上次写入时间。
    public class UnityFileVersionChecker
    {
        // 一般是RoleId
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

        // 返回文件md5
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

        // 登录包的时候， 判断local/remote以及文件是否被认为修改
        // !fileExist || localVersion != remoteVersion || localMd5 != fileMd5, 则重新请求新数据
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