using Lib;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Framework
{
    public class FilesDownloader
    {
        public List<string> _relativePaths = null;
        public int index = 0;
        private UnityWebRequestAsyncOperation _requestAsyncOperation;

        public bool isFinish { get; private set; }
        public float Progress { get; private set; }
        public bool isError { get; private set; }

        public void StartDownloader(List<string> relativePaths)
        {
            _relativePaths = relativePaths;
            isFinish = false;
            DownloaderNext();
        }

        public void DownloaderNext()
        {
            while (index < _relativePaths.Count)
            {
                Progress = (float)index / (float)_relativePaths.Count;

                string relativePath = _relativePaths[index];
                string path = AssetPath.GetPersistentFullPath(relativePath);
                ++index;

                if (!File.Exists(path))
                {
                    string streamingPath = AssetPath.GetStreamingFullPath(relativePath);

                    DownloadHandlerFile downloadHandlerFile = new DownloadHandlerFile(path);
                    downloadHandlerFile.removeFileOnAbort = true;

                    UnityWebRequest request = UnityWebRequest.Get(new System.Uri(streamingPath));
                    request.downloadHandler = downloadHandlerFile;
                    request.disposeDownloadHandlerOnDispose = true;

                    _requestAsyncOperation = request.SendWebRequest();
                    _requestAsyncOperation.completed += AsyncOperation_completed;
                    return;
                }
            }

            if (index >= _relativePaths.Count)
            {
                isFinish = true;
                Progress = 1f;
                _relativePaths.Clear();
                return;
            }
        }

        private void AsyncOperation_completed(AsyncOperation obj)
        {
            UnityWebRequestAsyncOperation asyncOperation = obj as UnityWebRequestAsyncOperation;

            bool success = asyncOperation.webRequest.result == UnityWebRequest.Result.Success;
            string error = asyncOperation.webRequest.error;
            bool isCurrent = asyncOperation == _requestAsyncOperation;

            asyncOperation.webRequest.Dispose();

            if (!isCurrent)
            {
                DebugUtil.LogError("Unknow UnityWebRequest completed");
                if (!success)
                {
                    DebugUtil.LogError(error);
                }
            }
            else
            {
                _requestAsyncOperation = null;

                if (success)
                {
                    DownloaderNext();
                }
                else
                {
                    DebugUtil.LogError(error);
                    isFinish = true;
                    isError = true;
                    _relativePaths.Clear();
                }
            }
        }

        public void Close()
        {
            if (_requestAsyncOperation != null)
            {
                _requestAsyncOperation.completed -= AsyncOperation_completed;
                _requestAsyncOperation.webRequest.Dispose();
                _requestAsyncOperation = null;
            }
        }
    }

    public class AssetMananger : TSingleton<AssetMananger>
    {
        public static byte key = 1 << 5;

        //#if UNITY_EDITOR && !FORCE_AB
        //        private const int nMaxLoadingCountSlow = 1;
        //        private const int nMaxLoadingCountFast = 2;
        //#else
        //    private const int nMaxLoadingCountSlow = 4;
        //    private const int nMaxLoadingCountFast = 32;
        //#endif

        private const int nCapacity = 32;
        private const int nAssetCapacity = 1024;

        //       private int nMaxLoadingCount = nMaxLoadingCountFast;
        //       private bool bFastLoad = true;

        //       private bool bInit = false;
        public bool bFirstUsePersistentAssets;

        //public bool UseFastLoad
        //{
        //    set
        //    {
        //        bFastLoad = value;
        //        nMaxLoadingCount = bFastLoad ? nMaxLoadingCountFast : nMaxLoadingCountSlow;
        //        Application.backgroundLoadingPriority = bFastLoad ? ThreadPriority.High : ThreadPriority.Normal;
        //    }
        //}

        #region WriteAssets
        private List<System.IAsyncResult> mAsyncResults = new List<System.IAsyncResult>();
        public void WriteToPersistent(string filePath, byte[] array)
        {
            Debug.LogFormat("保存资源资源到Persistent {0}", filePath);
            string dirPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            FileStream fileStream = File.Create(filePath);
            mAsyncResults.Add(fileStream.BeginWrite(array, 0, array.Length, OnFileWriteCompleted, fileStream));
        }
        private void OnFileWriteCompleted(System.IAsyncResult ar)
        {
            FileStream fileStream = ar.AsyncState as FileStream;
            fileStream.EndWrite(ar);
            fileStream.Close();
            fileStream.Dispose();
        }
        #endregion        

        #region UnloadAssets
        /// <summary>
        /// 卸载没用的资源
        /// </summary>
        /// <param name="immediately">是否立即卸载</param>
        public void UnloadUnusedAssets(bool immediately = false)
        {
            if (immediately)
            {
                _UnloadAssets(false);
            }
        }

        private void _UnloadAssets(bool forceAll)
        {
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
        #endregion


        public string GetPathWithoutExtension(string assetPath)
        {
            string path = null;
            int index = assetPath.LastIndexOf(".");
            if (index >= 0)
            {
                path = assetPath.Remove(index);
            }
            else
            {
                path = assetPath;
            }
            return path;
        }

        private bool GetAssetPathKey(ref string assetPath)
        {
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                Debug.LogErrorFormat("asset path is empty !");
                return false;
            }

            string fileName = Path.GetFileNameWithoutExtension(assetPath);

            if (string.IsNullOrWhiteSpace(fileName))
            {
                Debug.LogErrorFormat("asset path {0}, file name is empty !", assetPath);
                return false;
            }

            assetPath = assetPath.Replace('\\', '/');
            assetPath = GetPathWithoutExtension(assetPath);
            assetPath = assetPath.ToLower();

            return true;
        }

        private string GetRealPath(string relativePath)
        {
#if UNITY_EDITOR
#if FORCE_ENCRYPT
            string path = AssetPath.GetStreamingFullPath(relativePath);
#else
            string path = string.Format("{0}/{1}", AssetPath.dataPath, relativePath);
#endif
#else
            string path = AssetPath.GetPersistentFullPath(relativePath);
            if (!File.Exists(path))
            {
                string streamingPath = AssetPath.GetStreamingFullPath(relativePath);
#if UNITY_ANDROID
                using (UnityWebRequest request = UnityWebRequest.Get(new System.Uri(streamingPath)))
                {
                    request.downloadHandler = new DownloadHandlerFile(path);
                    UnityWebRequestAsyncOperation mByteRequest = request.SendWebRequest();
                    while (!mByteRequest.isDone) 
                    {                        
                        request.downloadHandler.Dispose();
                    }
                }
#else
                path = streamingPath;
#endif
            }
#endif
            return path;
        }

        public Stream LoadFileStream(string relativePath, bool useCompression = true, string password = null)
        {
            string path = GetRealPath(relativePath);

#if UNITY_EDITOR && (!FORCE_ENCRYPT)
            useCompression = false;
#endif

            if (useCompression)
            {
                Unity.SharpZipLib.Zip.ZipInputStream zipInputStream = new Unity.SharpZipLib.Zip.ZipInputStream(File.OpenRead(path));

                if (!string.IsNullOrWhiteSpace(password))
                {
                    zipInputStream.Password = password;
                }

                zipInputStream.GetNextEntry();

                return zipInputStream;
            }
            else
            {
                return File.OpenRead(path);
            }
        }

        public byte[] LoadBytes(string relativePath, bool useCompression = true, string password = null)
        {
            string path = GetRealPath(relativePath);

#if UNITY_EDITOR && (!FORCE_ENCRYPT)
            useCompression = false;
#endif

            byte[] buffer = null;
            if (useCompression)
            {
                using (Stream fileStream = File.OpenRead(path))
                using (Unity.SharpZipLib.Zip.ZipInputStream zipInputStream = new Unity.SharpZipLib.Zip.ZipInputStream(fileStream))
                {
                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        zipInputStream.Password = password;
                    }

                    Unity.SharpZipLib.Zip.ZipEntry zipEntry = zipInputStream.GetNextEntry();
                    if (zipEntry != null)
                    {
                        buffer = new byte[zipInputStream.Length];
                        zipInputStream.Read(buffer, 0, (int)zipInputStream.Length);
                    }
                }
            }
            else
            {
                using (FileStream fileStream = File.OpenRead(path))
                {
                    buffer = new byte[fileStream.Length];
                    fileStream.Read(buffer, 0, (int)fileStream.Length);
                }
            }

            return buffer;
        }

        public Stream LoadStream(string relativePath, bool useCompression = true, string password = null)
        {
            byte[] buffer = LoadBytes(relativePath, useCompression, password);
            MemoryStream memoryStream = new MemoryStream(buffer);
            return memoryStream;
        }

        public FilesDownloader CopyToPersistent(List<string> relativePaths)
        {
            FilesDownloader filesDownloader = new FilesDownloader();
            filesDownloader.StartDownloader(relativePaths);
            return filesDownloader;
            //CoroutineManager.Instance.StartHandler(_CopyToPersistent(relativePaths, action));
        }
    }
}
