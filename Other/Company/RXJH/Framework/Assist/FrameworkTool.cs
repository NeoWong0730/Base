///框架层工具类///
using Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Lib.Core
{
    public static class FrameworkTool
    {
        /// <summary>
        /// 获得文件MD5
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFileMD5(string filePath)
        {
            string fileMD5 = string.Empty;
            try
            {
                using (FileStream fs = File.OpenRead(filePath))
                {
                    MD5 md5 = MD5.Create();
                    byte[] fileMD5Bytes = md5.ComputeHash(fs);
                    fileMD5 = FormatMD5(fileMD5Bytes);
                }
            }
            catch (FileNotFoundException e)
            {
                Debug.LogException(e);
            }

            return fileMD5;
        }

        public static string GetFileMD5(FileStream fs)
        {
            string fileMD5 = string.Empty;
            try
            {
                MD5 md5 = MD5.Create();
                byte[] fileMD5Bytes = md5.ComputeHash(fs);
                fileMD5 = FormatMD5(fileMD5Bytes);
            }
            catch (FileNotFoundException e)
            {
                Debug.LogException(e);
            }

            return fileMD5;
        }

        /// <summary>
        /// 将byte[]装换成字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string FormatMD5(Byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", "").ToLower();
        }

        public static Google.Protobuf.ByteString ConvertToGoogleByteString(string str)
        {
            return Google.Protobuf.ByteString.CopyFrom(str, System.Text.Encoding.UTF8);
        }

        public static void CopyFile(string srcPath, string destPath, bool overwrite)
        {
            try
            {
                if (!File.Exists(srcPath))
                {
                    Debug.LogErrorFormat("not find file {0}", srcPath);
                    return;
                }

                string dir = Path.GetDirectoryName(destPath);

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);

                }

                File.Copy(srcPath, destPath, overwrite);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public static void CopyDirectory(string srcPath, string destPath, string ignoreExt)
        {
            try
            {
                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath);

                }

                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    string pathName = i.Name.Replace('\\', '/');

                    if (i is DirectoryInfo)     //判断是否文件夹
                    {
                        if (!Directory.Exists(destPath + "/" + pathName))
                        {
                            Directory.CreateDirectory(destPath + "/" + pathName);   //目标目录下不存在此文件夹即创建子文件夹
                        }
                        CopyDirectory(i.FullName, destPath + "/" + pathName, ignoreExt);    //递归调用复制子文件夹
                    }
                    else if (!i.Extension.Equals(ignoreExt))
                    {
                        File.Copy(i.FullName, destPath + "/" + pathName, true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public static bool IsAllUTF8(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return false;

            byte[] chars = System.Text.Encoding.UTF8.GetBytes(s);

            if (chars.Length == s.Length)
                return true;

            return false;
        }

        public static GameObject CreateGameObject(string rName, params Type[] rComps)
        {
            GameObject rGo = new GameObject(rName, rComps);

            rGo.transform.localPosition = Vector3.zero;
            rGo.transform.localRotation = Quaternion.identity;
            rGo.transform.localScale = Vector3.one;

            return rGo;
        }

        public static GameObject CreateGameObject(Transform rParentGo, string rName, params Type[] rComps)
        {
            GameObject rGo = new GameObject(rName, rComps);
            rGo.transform.parent = rParentGo;

            rGo.transform.localPosition = Vector3.zero;
            rGo.transform.localRotation = Quaternion.identity;
            rGo.transform.localScale = Vector3.one;

            return rGo;
        }

        public static GameObject CreateGameObject(GameObject rTemplateGo)
        {
            GameObject rGo = GameObject.Instantiate(rTemplateGo);

            rGo.name = rTemplateGo.name;
            rGo.transform.localPosition = Vector3.zero;
            rGo.transform.localRotation = Quaternion.identity;
            rGo.transform.localScale = Vector3.one;

            return rGo;
        }

        public static GameObject CreateGameObject(GameObject rTemplateGo, GameObject rParentGo)
        {
            GameObject rGo = GameObject.Instantiate(rTemplateGo);
            rGo.transform.SetParent(rParentGo.transform);
            //rGo.transform.parent = rParentGo.transform;

            rGo.name = rTemplateGo.name;
            rGo.transform.localPosition = Vector3.zero;
            rGo.transform.localRotation = Quaternion.identity;
            rGo.transform.localScale = rTemplateGo.transform.localScale;

            return rGo;
        }

        public static void CreateChildList(Transform rParentGo, int needchildCount)
        {
            int curchildCount = rParentGo.childCount;
            if (curchildCount < 1)
            {
                DebugUtil.LogErrorFormat($"{rParentGo.name} 没有模板子物体，无法创建子物体列表");
                return;
            }
            GameObject rTemplateGo = rParentGo.GetChild(0).gameObject;
            int needInstantiateCount = needchildCount - curchildCount;
            if (needInstantiateCount <= 0)
            {
                for (int i = 0; i < curchildCount; i++)
                {
                    if (i < needchildCount)
                    {
                        rParentGo.GetChild(i).gameObject.SetActive(true);
                    }
                    else
                    {
                        rParentGo.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                for (int i = 0; i < curchildCount; i++)
                {
                    rParentGo.GetChild(i).gameObject.SetActive(true);
                }
                for (int i = 0; i < needInstantiateCount; i++)
                {
                    CreateGameObject(rTemplateGo, rParentGo.gameObject);
                }
            }
        }

        public static void CreateChildListAndFixedLast(Transform rParentGo, Transform rLastChild, int needchildCount)
        {
            int curchildCount = rParentGo.childCount - 1;
            if (curchildCount < 1)
            {
                DebugUtil.LogErrorFormat($"{rParentGo.name} 没有模板子物体，无法创建子物体列表");
                return;
            }
            GameObject rTemplateGo = rParentGo.GetChild(0).gameObject;
            int needInstantiateCount = needchildCount - curchildCount;
            if (needInstantiateCount <= 0)
            {
                for (int i = 0; i < curchildCount; i++)
                {
                    if (i < needchildCount)
                    {
                        rParentGo.GetChild(i).gameObject.SetActive(true);
                    }
                    else
                    {
                        rParentGo.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                for (int i = 0; i < curchildCount; i++)
                {
                    rParentGo.GetChild(i).gameObject.SetActive(true);
                }
                for (int i = 0; i < needInstantiateCount; i++)
                {
                    CreateGameObject(rTemplateGo, rParentGo.gameObject);
                }
            }

            rLastChild.SetAsLastSibling();
            FrameworkTool.ForceRebuildLayout(rParentGo.gameObject);
        }

        public static void CreateChildList(Transform rParentGo, int needchildCount, int startIndex)
        {
            int curchildCount = rParentGo.childCount - startIndex;
            if (curchildCount < 1)
            {
                DebugUtil.LogErrorFormat($"{rParentGo.name} 没有模板子物体，无法创建子物体列表");
                return;
            }
            GameObject rTemplateGo = rParentGo.GetChild(startIndex).gameObject;
            int needInstantiateCount = needchildCount - curchildCount;
            if (needInstantiateCount <= 0)
            {
                for (int i = startIndex; i < rParentGo.childCount; i++)
                {
                    if (i < needchildCount + startIndex)
                    {
                        rParentGo.GetChild(i).gameObject.SetActive(true);
                    }
                    else
                    {
                        rParentGo.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                for (int i = startIndex; i < curchildCount; i++)
                {
                    rParentGo.GetChild(i).gameObject.SetActive(true);
                }
                for (int i = 0; i < needInstantiateCount; i++)
                {
                    CreateGameObject(rTemplateGo, rParentGo.gameObject);
                }
            }
        }


        public static void DestroyChildren(GameObject parent, params string[] list)
        {
            List<string> dontList = new List<string>();
            if (list != null)
            {
                foreach (string var in list)
                {
                    dontList.Add(var);
                }
            }
            parent.DestoryAllChildren(dontList, true);
        }

        public static void ForceRebuildLayout(GameObject go)
        {
            UnityEngine.UI.ContentSizeFitter[] fitter = go.GetComponentsInChildren<UnityEngine.UI.ContentSizeFitter>(true);
            for (int i = fitter.Length - 1; i >= 0; --i)
            {
                RectTransform trans = fitter[i].gameObject.GetComponent<RectTransform>();
                if (trans != null)
                    UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(trans);
            }
        }


        public static Color GetPixel(Texture2D texture, int x, int y)
        {
            return texture.GetPixel(x, y);
        }

        public static void SetPixel(Texture2D texture, int x, int y, Color col)
        {
            texture.SetPixel(x, y, col);
        }

        public static void CheckPoint(Texture2D MyTex, GameObject rawImage, Vector3 worldPos, int width, int height, int brushSize, Action action)
        {
            Vector3 localPos = rawImage.transform.InverseTransformPoint(worldPos);

            if (localPos.x > -width / 2 && localPos.x < width / 2 && localPos.y > -height / 2 && localPos.y < height / 2)
            {
                for (int i = (int)localPos.x - brushSize; i < (int)localPos.x + brushSize; i++)
                {
                    for (int j = (int)localPos.y - brushSize; j < (int)localPos.y + brushSize; j++)
                    {
                        if (Mathf.Pow(i - localPos.x, 2) + Mathf.Pow(j - localPos.y, 2) > Mathf.Pow(brushSize, 2))
                            continue;
                        if (i < 0) { if (i < -width / 2) { continue; } }
                        if (i > 0) { if (i > width / 2) { continue; } }
                        if (j < 0) { if (j < -width / 2) { continue; } }
                        if (j > 0) { if (j > width / 2) { continue; } }

                        Color col = GetPixel(MyTex, i + width / 2, j + height / 2);
                        if (col.a != 0f)
                        {
                            col.a = 0.0f;
                            action?.Invoke();
                            SetPixel(MyTex, i + width / 2, j + height / 2, col);
                        }
                    }
                }
                MyTex.Apply();
            }
        }

        public static Vector3 KeepFract(ref Vector3 pos, uint exp = 3)
        {
            if (exp != 0)
            {
                int pow = (int)Math.Pow(10, exp);
                pos.x = Mathf.FloorToInt(pos.x * pow) * 1f / pow;
                pos.y = Mathf.FloorToInt(pos.y * pow) * 1f / pow;
                pos.z = Mathf.FloorToInt(pos.z * pow) * 1f / pow;
            }
            return pos;
        }

        public static string ToLongTimeString(long time)
        {
            var dt = new DateTime(0).AddSeconds(time);
            return dt.ToLongTimeString();
        }


        public static IList<T> FilterList<T>(IList<T> less, IList<T> more)
        {
            List<T> res = new List<T>();
            for (int i = 0; i < more.Count; i++)
            {
                if (!less.Contains(more[i]))
                {
                    res.Add(more[i]);
                }
            }
            return res;
        }

        public static string ListEncode<T>(IList<T> ls)
        {
            if (ls != null && ls.Count > 0)
            {
                StringBuilder tempStringBuilder = StringBuilderPool.GetTemporary();
                int i = 0;
                for (int length = ls.Count - 1; i < length; ++i)
                {
                    tempStringBuilder.Append(ls[i].ToString());
                    tempStringBuilder.Append(",");
                }

                tempStringBuilder.Append(ls[i].ToString());
                string rlt = tempStringBuilder.ToString();
                StringBuilderPool.ReleaseTemporary(tempStringBuilder);
                return rlt;
            }
            else
            {
                return null;
            }
        }
        public static List<uint> ListDecode(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return new List<uint>(0);
            }
            else
            {
                string[] segments = content.Split(',');
                int length = segments.Length;
                var ls = new List<uint>(length);
                for (int i = 0; i < length; ++i)
                {
                    uint.TryParse(segments[i], out uint value);
                    if (value != 0)
                    {
                        ls.Add(value);
                    }
                }
                return ls;
            }
        }

        public static void CreateChild(in Transform t, in Dictionary<int, Transform> template, in List<int> ls)
        {
            DestroyChildren(t.gameObject, null);
            if (template != null)
            {
                for (int i = 0, length = ls.Count; i < length; ++i)
                {
                    if (template.TryGetValue(ls[i], out Transform orig))
                    {
                        Transform tmp = GameObject.Instantiate<Transform>(orig, t);
                        tmp.name = ls[i].ToString();
                    }
                }
            }
        }

        public static int GetCharNum(string text)
        {
            /*
            int totalNum = 0;
            char[] charText = text.ToCharArray();
            foreach (var item in charText)
            {
                int leng = System.Text.Encoding.UTF8.GetByteCount(item.ToString());
                if (leng >= 2) leng = 2;
                totalNum += leng;
            }
            return totalNum;
            */

            if (string.IsNullOrEmpty(text))
                return 0;

            int totalNum = 0;
            int length = text.Length;
            for (int i = 0; i < length; ++i)
            {
                int count;
                char c = text[i];
                unsafe
                {
                    char* cPtr = &c;
                    count = System.Text.Encoding.UTF8.GetByteCount(cPtr, 1);
                }
                totalNum += Mathf.Min(2, count);
            }
            return totalNum;
        }

        public static bool IsIntergetOrLetterOrChinese(string text)
        {
            System.Text.RegularExpressions.Regex reg1
                = new System.Text.RegularExpressions.Regex(@"^[\u4e00-\u9fa5a-zA-Z0-9]+$");

            if (reg1.IsMatch(text))
                return true;
            else
                return false;
        }

        public static void AppendIgnore(this StringBuilder stringBuilder, string value, int startIndex, int count, char c)
        {
            for (int i = 0; i < value.Length; ++i)
            {
                char v = value[i];
                if (v != c)
                {
                    stringBuilder.Append(v);
                }
            }
        }

        public static void AppendIgnore(this StringBuilder stringBuilder, string value, int startIndex, int count, HashSet<char> cs)
        {
            for (int i = 0; i < value.Length; ++i)
            {
                char v = value[i];
                if (!cs.Contains(v))
                {
                    stringBuilder.Append(v);
                }
            }
        }
    }
}
