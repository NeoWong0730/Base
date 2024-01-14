using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public struct CheckTextLengthResult
{
    public bool IsSplit;
    public string ResultStr;
}

public class CheckTextLengthScript : MonoBehaviour
{
    public InputField input;
    public int CHARACTER_LIMIT = 10;
    public SplitType m_SplitType = SplitType.ASCII;

    public enum SplitType
    {
        ASCII = 1,
        GB = 2,
        Unicode = 3,
        UTF8 = 4,
    }

    public CheckTextLengthResult Check()
    {
        return GetSplitText((int)m_SplitType);
    }

    public CheckTextLengthResult GetSplitText(int checkType)
    {
        string temp = input.text.Substring(0, (input.text.Length < CHARACTER_LIMIT + 1) ? input.text.Length : CHARACTER_LIMIT + 1);
        if (checkType == (int)SplitType.ASCII)
        {
            return SplitTextByASCII(temp);
        }
        else if (checkType == (int)SplitType.GB)
        {
            return SplitTextByGB(temp);
        }
        else if (checkType == (int)SplitType.Unicode)
        {
            return SplitTextByUnicode(temp);
        }
        else if (checkType == (int)SplitType.UTF8)
        {
            return SplitTextByUTF8(temp);
        }

        return default(CheckTextLengthResult);
    }
    //4、UTF8编码格式（汉字3byte，英文1byte）,//UTF8编码格式,目前是最常用的 
    private CheckTextLengthResult SplitTextByUTF8(string temp)
    {
        CheckTextLengthResult result = new CheckTextLengthResult();
        int count = 0;

        for (int i = 0; i < temp.Length; i++)
        {
            string tempStr = temp.Substring(i, 1);
            byte[] encodedBytes = System.Text.ASCIIEncoding.UTF8.GetBytes(tempStr);//Unicode用两个字节对字符进行编码
            //string output = "[" + temp + "]";
            //for (int byteIndex = 0; byteIndex < encodedBytes.Length; byteIndex++)
            //{
            //    output += Convert.ToString((int)encodedBytes[byteIndex], 2) + "  ";//二进制
            //}
            //Debug.Log(output);

            int byteCount = System.Text.ASCIIEncoding.UTF8.GetByteCount(tempStr);
            //Debug.Log("字节数=" + byteCount);

            if (byteCount > 1)
            {
                count += 2;
            }
            else
            {
                count += 1;
            }
            if (count <= CHARACTER_LIMIT)
            {
                result.ResultStr += tempStr;
            }
            else
            {
                result.IsSplit = true;
                break;
            }
        }
        return result;
    }

    private CheckTextLengthResult SplitTextByUnicode(string temp)
    {
        CheckTextLengthResult result = new CheckTextLengthResult();
        int count = 0;

        for (int i = 0; i < temp.Length; i++)
        {
            string tempStr = temp.Substring(i, 1);
            byte[] encodedBytes = System.Text.ASCIIEncoding.Unicode.GetBytes(tempStr);//Unicode用两个字节对字符进行编码
            if (encodedBytes.Length == 2)
            {
                int byteValue = (int)encodedBytes[1];
                if (byteValue == 0)//这里是单个字节
                {
                    count += 1;
                }
                else
                {
                    count += 2;
                }
            }
            if (count <= CHARACTER_LIMIT)
            {
                result.ResultStr += tempStr;
            }
            else
            {
                result.IsSplit = true;
                break;
            }
        }
        return result;
    }

    private CheckTextLengthResult SplitTextByGB(string temp)
    {
        CheckTextLengthResult result = new CheckTextLengthResult();
        int count = 0;

        for (int i = 0; i < temp.Length; i++)
        {
            string tempStr = temp.Substring(i, 1);
            byte[] encodedBytes = System.Text.ASCIIEncoding.Default.GetBytes(tempStr);
            if (encodedBytes.Length == 1)
            {
                //单字节
                count += 1;
            }
            else
            {
                //双字节
                count += 2;
            }

            if (count <= CHARACTER_LIMIT)
            {
                result.ResultStr += tempStr;
            }
            else
            {
                result.IsSplit = true;
                break;
            }
        }
        return result;
    }

    private CheckTextLengthResult SplitTextByASCII(string temp)
    {
        byte[] encodedBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(temp);

        CheckTextLengthResult result = new CheckTextLengthResult();
        int count = 0;

        for (int i = 0; i < temp.Length; i++)
        {
            if ((int)encodedBytes[i] == 63)//双字节
                count += 2;
            else
                count += 1;

            if (count <= CHARACTER_LIMIT)
                result.ResultStr += temp.Substring(i, 1);
            else if (count > CHARACTER_LIMIT)
                break;
        }

        if (count <= CHARACTER_LIMIT)
        {
            result.ResultStr = temp;
        }
        else
        {
            result.IsSplit = true;
        }

        return result;
    }

}
