using Lib.AssetLoader;
using Lib.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Framework
{
    public enum EWaitState
    {
        Invalid = 0,
        WaitOtherAppHotFix,//等待热更端结束
        ActionHotFix,//执行热更
        ReHotFix_RemoteVersion,//其他进程正在运行的版本低于当前即将热更版本
        Game,
    }

    public class WaitAppHotFixManager : TSingleton<WaitAppHotFixManager>
    {
        private string hotFixFlag;
        private EWaitState eState = EWaitState.Invalid;
        private string appProcessDir;
        private List<string> m_NeedDeleteProcessPath = new List<string>();

        int frameCount = 0;
        int frameRate = 20;
        bool nextFrameAction;

        public void OnEnter()
        {
            string dir = AssetPath.GetPersistentFullPath(AssetPath.sProcessDir);
            appProcessDir = string.Format("{0}/{1}_pid.txt", dir, AppManager.ProcessID.ToString());
            if (File.Exists(appProcessDir))
                File.WriteAllText(appProcessDir, EAppState.WaitOtherAppHotFix.ToString());

            eState = EWaitState.ActionHotFix;
            hotFixFlag = string.Format("{0}/{1}", Application.persistentDataPath, AssetPath.sHotFixFlag);
            HotFixProcessInfo(out int flag);

            if (File.Exists(hotFixFlag))
            {
                if (GetGameProcessCount() > 1)
                {
                    eState = EWaitState.WaitOtherAppHotFix;
                }
                else
                    ClearHotFixFlag();
            }
            else if (GetGameProcessCount() > 1)
            {
                eState = EWaitState.ReHotFix_RemoteVersion;
            }
            UpdateState();
        }

        public void OnExit()
        {
            if (File.Exists(appProcessDir))
                File.WriteAllText(appProcessDir, AppManager.ProcessID.ToString());
            UI_Box.Destroy();
        }

        public void OnUpdate()
        {
            frameCount++;
            if (frameCount % frameRate == 0)
            {
                if (eState == EWaitState.WaitOtherAppHotFix)
                {
                    HotFixProcessInfo(out int flag);
                    if (File.Exists(hotFixFlag))
                        return;
                    if(!nextFrameAction)
                    {
                        nextFrameAction = true;
                        return;
                    }
                    if (flag == 1 || nextFrameAction)
                    {
                        nextFrameAction = false;
                        SendMessageToOtherProcess(false);
                        DebugUtil.Log(ELogType.eNone, "OnUpdate nextFrameAction{0}");
                    }
                }
                else
                {
                    nextFrameAction = false;
                }
            }         
        }

        private void HotFixProcessInfo(out int flag)
        {
            if(File.Exists(hotFixFlag))
            {
                string readProcess = File.ReadAllText(hotFixFlag);
                int hotFixProcess = 0;
                if (int.TryParse(readProcess, out hotFixProcess) && !IsHaveProcessById(hotFixProcess))
                {
                    File.Delete(hotFixFlag);
                    flag = 1;
                    return;
                }
            }
            flag = 0;
        }

        private void UpdateState()
        {
            switch (eState)
            {
                case EWaitState.Invalid:
                    UI_Box.Destroy();
                    break;
                case EWaitState.WaitOtherAppHotFix:
                    UI_Box.Create(UseUIBOxType.PC_WaitOtherAppHotFixError);
                    break;
                case EWaitState.ActionHotFix:
                    if (File.Exists(hotFixFlag))
                    {
                        eState = EWaitState.WaitOtherAppHotFix;
                        UpdateState();
                        break;
                    }
                    File.WriteAllText(hotFixFlag, AppManager.ProcessID.ToString());
                    AppManager.NextAppState = EAppState.HotFix;
                    break;
                case EWaitState.ReHotFix_RemoteVersion:
                    UI_Box.Create(UseUIBOxType.PC_RemoteVersionUpdateError);
                    break;
                case EWaitState.Game:
                    AppManager.NextAppState = EAppState.CheckVersion;
                    break;
                default:
                    break;
            }
        }

        private void ClearHotFixFlag()
        {
            string hotFixFlag = string.Format("{0}/{1}", Application.persistentDataPath, AssetPath.sHotFixFlag);
            if (File.Exists(hotFixFlag))
                File.Delete(hotFixFlag);
        }

        /// <summary>
        /// 获取当前进程数量
        /// </summary>
        public int GetGameProcessCount()
        {
            string dir = string.Format("{0}/{1}", Application.persistentDataPath, AssetPath.sProcessDir);
            if (!Directory.Exists(dir))
                return -1;
            string[] allProcessPath = Directory.GetFiles(dir);
            int count = 0;
            m_NeedDeleteProcessPath.Clear();
            for (int i = 0; i < allProcessPath.Length; i++)
            {
                if (!Path.GetExtension(allProcessPath[i]).Contains(".txt"))
                    continue;
                string fileName = Path.GetFileName(allProcessPath[i]);
                int.TryParse(fileName.Split('_')[0], out int pid);

                if(!IsHaveProcessById(pid))
                {
                    DebugUtil.LogFormat(ELogType.eNone, "IsHaveProcessById--pid:{0}", pid);
                    m_NeedDeleteProcessPath.Add(allProcessPath[i]);
                    continue;
                }
                count++;
            }
            for (int i = 0; i < m_NeedDeleteProcessPath.Count; i++)
            {
                string path = m_NeedDeleteProcessPath[i];
                if (File.Exists(path))
                    File.Delete(path);
            }
            DebugUtil.LogFormat(ELogType.eNone, "GetGameProcessCount:{0}", count);
            return count;
        }

        private bool IsHaveProcessById(int pid)
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
             return AspectRotioController.Instance.IsHaveProcessById(pid);
#else
             return true;
#endif
        }

        private static List<int> GetOtherProcessID()
        {
            string appProcessDir = string.Format("{0}/{1}", Application.persistentDataPath, AssetPath.sProcessDir);
            if (!Directory.Exists(appProcessDir))
                return null;
            string[] allProcessPath = Directory.GetFiles(appProcessDir);
            List<int> processList = new List<int>();
            for (int i = 0; i < allProcessPath.Length; i++)
            {
                if (!Path.GetExtension(allProcessPath[i]).Contains(".txt"))
                    continue;          
                string readT = File.ReadAllText(allProcessPath[i]);
                if (string.Equals(readT, EAppState.WaitOtherAppHotFix.ToString()))
                {
                    string fileName = Path.GetFileName(allProcessPath[i]);
                    int.TryParse(fileName.Split('_')[0], out int pid);
                    processList.Add(pid);
                }
            }
            return processList;
        }

#region Send,Rec Message
        public void SendMessageToOtherProcess(bool isSuccess = true)
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR

            List<int> processList = GetOtherProcessID();
            if (processList == null)
                return;
            for (int i = 0; i < processList.Count; i++)
            {
                if (!IsHaveProcessById(processList[i]))
                    continue;
                AspectRotioController.Instance.SendMessageToProcess(processList[i], isSuccess ? 1 : 0);
                if (!isSuccess)
                    break;
            }
#endif
        }

        public void RecMessageByOtherProcess(bool isHotFix = false)
        {
            if (!File.Exists(hotFixFlag) && eState == EWaitState.WaitOtherAppHotFix)
            {
                if (!isHotFix)
                    eState = EWaitState.Game;
                else
                    eState = EWaitState.ActionHotFix;
                UpdateState();
            }
        }
#endregion
    }
}
