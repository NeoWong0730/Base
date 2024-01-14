using System;
using System.Collections.Generic;
using System.Diagnostics;
public class CmdHandler
{
    private static string CmdPath = "cmd.exe";
    //C:\Windows\System32\cmd.exe
    /// <summary>
    /// ִ��cmd���� ����cmd������ʾ����Ϣ
    /// ��������ʹ���������������ӷ���
    /// <![CDATA[
    /// &:ͬʱִ����������
    /// |:����һ����������,��Ϊ��һ�����������
    /// &&����&&ǰ������ɹ�ʱ,��ִ��&&�������
    /// ||����||ǰ������ʧ��ʱ,��ִ��||�������]]>
    /// </summary>
    /// <param name="cmd">ִ�е�����</param>
    public static string RunCmd(string cmd, Action<string> act = null)
    {
        cmd = cmd.Trim().TrimEnd('&') + "&exit";//˵�������������Ƿ�ɹ���ִ��exit������򵱵���ReadToEnd()����ʱ���ᴦ�ڼ���״̬
        using (Process p = new Process())
        {
            p.StartInfo.FileName = CmdPath;
            p.StartInfo.UseShellExecute = false;        //�Ƿ�ʹ�ò���ϵͳshell����
            p.StartInfo.RedirectStandardInput = true;   //�������Ե��ó����������Ϣ
            p.StartInfo.RedirectStandardOutput = true;  //�ɵ��ó����ȡ�����Ϣ
            p.StartInfo.RedirectStandardError = true;   //�ض����׼�������
            p.StartInfo.CreateNoWindow = true;          //����ʾ���򴰿�
            p.Start();//��������

            //��cmd����д������
            p.StandardInput.WriteLine(cmd);
            p.StandardInput.AutoFlush = true;

            //��ȡcmd���ڵ������Ϣ
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();//�ȴ�����ִ�����˳�����
            p.Close();
            if (act != null)
            {
                act(output);
            }
            return output;
        }
    }

    /// <summary>
    /// ִ�ж��cmd����
    /// </summary>
    /// <param name="cmdList"></param>
    /// <param name="act"></param>
    public static void RunCmd(List<string> cmd, Action<string> act = null)
    {
        //cmd = cmd.Trim().TrimEnd('&') + "&exit";//˵�������������Ƿ�ɹ���ִ��exit������򵱵���ReadToEnd()����ʱ���ᴦ�ڼ���״̬
        using (Process p = new Process())
        {
            p.StartInfo.FileName = CmdPath;
            p.StartInfo.UseShellExecute = false;        //�Ƿ�ʹ�ò���ϵͳshell����
            p.StartInfo.RedirectStandardInput = true;   //�������Ե��ó����������Ϣ
            p.StartInfo.RedirectStandardOutput = true;  //�ɵ��ó����ȡ�����Ϣ
            p.StartInfo.RedirectStandardError = true;   //�ض����׼�������
            p.StartInfo.CreateNoWindow = true;          //����ʾ���򴰿�
            p.Start();//��������

            //��cmd����д������
            foreach (var cm in cmd)
            {
                p.StandardInput.WriteLine(cm);
                p.StandardInput.WriteLine("exit");
                p.StandardInput.AutoFlush = true;
                //��ȡcmd���ڵ������Ϣ
                string output = p.StandardOutput.ReadToEnd();
                if (act != null)
                {
                    act(output);
                }
                p.Start();
            }

            p.WaitForExit();//�ȴ�����ִ�����˳�����
            p.Close();
        }
    }
}