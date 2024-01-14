using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public class IllegalWordDetection
    {
        /// <summary>
        /// �������еĳ��ȴ���1�����дʻ�
        /// </summary>
        static HashSet<string> wordsSet = new HashSet<string>();
        /// <summary>
        /// ����ĳһ�������������д��е�λ�ã�������8���Ľض�Ϊ��8��λ�ã�
        /// </summary>
        static byte[] fastCheck = new byte[char.MaxValue];
        /// <summary>
        /// �����������дʵĳ�����Ϣ����Key��ֵΪ�������дʵĵ�һ���ʣ����дʵĳ��Ȼ�ض�Ϊ8
        /// </summary>
        static byte[] fastLength = new byte[char.MaxValue];
        /// <summary>
        /// �����������дʻ�ĵ�һ���ʵļ�¼���������ж��Ƿ�һ������һ�����߶�����дʻ�ġ���һ���ʡ����ҿ��ж���ĳһ������Ϊ��һ���ʵ�һϵ�е����дʵ����ĳ���
        /// </summary>
        static byte[] startCache = new byte[char.MaxValue];
        static char[] dectectedBuffer = null;
        static string SkipList = " \t\r\n~!@#$%^&*()_+-=������{}|;':\"���������������¦æĦŦƦǦȦɦʦ˦̦ͦΦϦЦѦҦӦԦզ֦צئ��������������������������������������������������������������������������������������ã�������������������������������������������ۣݣ�����������������������������������������������������������¢âĢ����������٢ڢۢܢݢޢߢ���ŢƢǢȢɢʢˢ̢͢΢ϢТѢҢӢԢբ֢עء֡ԡ٣��ܡݣ����ڡۡˡ��������£��ҡӡءޡġšơǡȡɡʡߡ�͡ΡϡСѡաס̡�������������뀡�������������������㣣�����ܦ�ߣ��D��⩰�����������������������©éĩũƩǩ������Щѩҩөԩթ֩ש�������穸���������������������ȩɩʩ˩̩ͩΩϩ������ة٩ک۩ܩݩީߩ��������";
        static BitArray SkipBitArray = new BitArray(char.MaxValue);
        /// <summary>
        /// �����������дʻ�����һ���ʵļ�¼���������ж��Ƿ�һ������һ�����߶�����дʻ�ġ����һ���ʡ�
        /// </summary>
        static BitArray endCache = new BitArray(char.MaxValue);

        unsafe public static void Init(string[] badwords)
        {
            if (badwords == null || badwords.Length == 0)
                return;

            int wordLength = 0;
            int maxWordLength = int.MinValue;
            for (int stringIndex = 0, len = badwords.Length; stringIndex < len; ++stringIndex)
            {
                ///��õ��������дʻ�ĳ���
                wordLength = badwords[stringIndex].Length;
                if (wordLength <= 0)
                    continue;

                maxWordLength = Math.Max(wordLength, maxWordLength);
                for (int i = 0; i < wordLength; i++)
                {
                    ///׼ȷ��¼8λ���ڵ����дʻ��ĳ�����ڴʻ��еġ�λ�á�
                    if (i < 7)
                        fastCheck[badwords[stringIndex][i]] |= (byte)(1 << i);
                    else///8λ��������дʻ�Ĵ�ֱ���޶��ڵ�8λ
                        fastCheck[badwords[stringIndex][i]] |= 0x80;///0x80���ڴ��м�Ϊ1000 0000����Ϊһ��byte�����ʾ8λ���ʳ���8λ�Ķ�λ����0x80���ضϳɵ�8λ
                }

                ///�������дʻ�ĳ���
                int cacheWordslength = Math.Min(8, wordLength);
                ///��¼���дʻ�ġ����³��ȣ�����8���ֵ����дʻ�ᱻ��ȡ��8�ĳ��ȣ�������key��ֵΪ���дʻ�ĵ�һ����
                fastLength[badwords[stringIndex][0]] |= (byte)(1 << (cacheWordslength - 1));
                ///�������ǰ��badwords[stringIndex][0]�ʿ�ͷ��һϵ�е����дʻ����ĳ���
                if (startCache[badwords[stringIndex][0]] < cacheWordslength)
                    startCache[badwords[stringIndex][0]] = (byte)(cacheWordslength);

                ///������дʻ�����һ���ʻ�ġ����������
                endCache[badwords[stringIndex][wordLength - 1]] = true;
                ///�����ȴ���1�����дʻ㶼ѹ�뵽�ֵ���
                if (!wordsSet.Contains(badwords[stringIndex]))
                    wordsSet.Add(badwords[stringIndex]);
            }

            /// ��ʼ����һ���������⵽���ַ�����buffer
            dectectedBuffer = new char[maxWordLength];
            /// ��¼Ӧ�������Ĳ�����Ĵ�
            fixed (char* start = SkipList)
            {
                char* itor = start;
                char* end = start + SkipList.Length;
                while (itor < end) SkipBitArray[*itor++] = true;
            }
        }


        private static bool _IsIllegaWorld;
        public static bool IsIllegaWorld
        {
            private set
            {
                IsIllegaWorld = value;
            }
            get
            {
                return _IsIllegaWorld;
            }
        }

        /// <summary>
        /// �����ַ���,Ĭ���������дʻ����'*'����
        /// </summary>
        /// <param name="text"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        unsafe public static string Filter(string text, string mask = "*")
        {
            var dic = DetectIllegalWords(text);
            ///���û�����дʻ㣬��ֱ�ӷ��س�ȥ
            if (dic.Count == 0)
            {
                _IsIllegaWorld = false;
                return text;
            }
            fixed (char* newText = text, cMask = mask)
            {
                var itor = newText;
                ///��ʼ�滻���дʻ�
                foreach (var item in dic)
                {
                    ///ƫ�Ƶ����дʳ��ֵ�λ��
                    itor = newText + item.Key;
                    for (int index = 0; index < item.Value; index++)
                    {
                        ///���ε����дʻ�
                        *itor++ = *cMask;
                    }
                }
            }
            _IsIllegaWorld = true;
            return text;
        }

        /// <summary>
        /// �ж�text�Ƿ������дʻ�,����з������еĴʻ��λ��,����ָ��������ӿ������ٶ�,��ʱ������һ�����������������Ǵ��븴�õ����
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        unsafe public static bool IllegalWordsExistJudgement(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            fixed (char* ptext = text, detectedStrStart = dectectedBuffer)
            {
                ///�����ַ����ĳ�ʼλ��
                char* itor = (fastCheck[*ptext] & 0x01) == 0 ? ptext + 1 : ptext;
                ///�����ַ�����ĩβλ��
                char* end = ptext + text.Length;

                while (itor < end)
                {
                    ///���text�ĵ�һ���ʲ������дʻ���ߵ�ǰ��������text��һ���ʵĺ���Ĵʣ���ѭ����⵽text�ʻ�ĵ����ڶ����ʣ�������һ�����ַ�������û�����дʻ�
                    if ((fastCheck[*itor] & 0x01) == 0)
                    {
                        while (itor < end - 1 && (fastCheck[*(++itor)] & 0x01) == 0) ;
                    }

                    ///�����ֻ��һ���ʵ����дʣ��ҵ�ǰ���ַ����ġ��ǵ�һ���ʡ�����������дʣ����ȼ����Ѽ�⵽�����д��б�
                    if (startCache[*itor] != 0 && (fastLength[*itor] & 0x01) > 0)
                    {
                        return true;
                    }

                    char* strItor = detectedStrStart;
                    *strItor++ = *itor;
                    int remainLength = (int)(end - itor - 1);
                    int skipCount = 0;
                    ///��ʱ�Ѿ���⵽һ�����дʵġ��״ʡ���,��¼�µ�һ����⵽�����дʵ�λ��
                    ///�ӵ�ǰ��λ�ü�⵽�ַ���ĩβ
                    for (int i = 1; i <= remainLength; ++i)
                    {
                        char* subItor = itor + i;
                        /// ����һЩ���˵��ַ�,����ո��������֮���
                        if (SkipBitArray[*subItor])
                        {
                            ++skipCount;
                            continue;
                        }
                        ///�����⵽��ǰ�Ĵ����������д��е�λ����Ϣ��û�д��ڵ�iλ�ģ���������������
                        if ((fastCheck[*subItor] >> Math.Min(i - skipCount, 7)) == 0)
                        {
                            break;
                        }

                        *strItor++ = *subItor;
                        ///����м�⵽���дʵ����һ���ʣ����Ҵ�ʱ�ġ���⵽�����дʻ㡱�ĳ���Ҳ����Ҫ����Ž�һ���鿴��⵽�����дʻ��Ƿ����������
                        if ((fastLength[*itor] >> Math.Min(i - 1 - skipCount, 7)) > 0 && endCache[*subItor])
                        {
                            ///��������ַ��������д��ֵ��д��ڣ����¼�������ж��Ǳ������д��м������������дʵĵ��ʣ���������㷨�޷��޳���������hash�������޳�
                            ///�����㷨�����ڼ��ٴ󲿷ֵıȽ�����
                            if (wordsSet.Contains(new string(dectectedBuffer, 0, (int)(strItor - detectedStrStart))))
                            {
                                return true;
                            }
                        }
                        else if (i - skipCount > startCache[*itor] && startCache[*itor] < 0x80)///����������Ըô�Ϊ�׵�һϵ�е����дʻ�����ĳ��ȣ��򲻼����ж�(ǰ���Ǹôʶ�Ӧ���������дʻ�û�г���8���ʵ�)
                        {
                            break;
                        }
                    }
                    ++itor;
                }
            }

            return false;
        }

        /// <summary>
        /// �ж�text�Ƿ������дʻ�,����з������еĴʻ��λ��,����ָ��������ӿ������ٶ�
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        unsafe public static Dictionary<int, int> DetectIllegalWords(string text)
        {
            var findResult = new Dictionary<int, int>();
            if (string.IsNullOrEmpty(text))
                return findResult;

            fixed (char* ptext = text, detectedStrStart = dectectedBuffer)
            {
                ///�����ַ����ĳ�ʼλ��
                char* itor = (fastCheck[*ptext] & 0x01) == 0 ? ptext + 1 : ptext;
                ///�����ַ�����ĩβλ��
                char* end = ptext + text.Length;

                while (itor < end)
                {
                    ///���text�ĵ�һ���ʲ������дʻ���ߵ�ǰ��������text��һ���ʵĺ���Ĵʣ���ѭ����⵽text�ʻ�ĵ����ڶ����ʣ�������һ�����ַ�������û�����дʻ�
                    if ((fastCheck[*itor] & 0x01) == 0)
                    {
                        while (itor < end - 1 && (fastCheck[*(++itor)] & 0x01) == 0) ;
                    }

                    ///�����ֻ��һ���ʵ����дʣ��ҵ�ǰ���ַ����ġ��ǵ�һ���ʡ�����������дʣ����ȼ����Ѽ�⵽�����д��б�
                    if (startCache[*itor] != 0 && (fastLength[*itor] & 0x01) > 0)
                    {
                        ///�������д���text�е�λ�ã��Լ����дʵĳ��ȣ������˹�����
                        findResult.Add((int)(itor - ptext), 1);
                    }

                    char* strItor = detectedStrStart;
                    *strItor++ = *itor;
                    int remainLength = (int)(end - itor - 1);
                    int skipCount = 0;
                    ///��ʱ�Ѿ���⵽һ�����дʵġ��״ʡ���,��¼�µ�һ����⵽�����дʵ�λ��
                    ///�ӵ�ǰ��λ�ü�⵽�ַ���ĩβ
                    for (int i = 1; i <= remainLength; ++i)
                    {
                        char* subItor = itor + i;
                        /// ����һЩ���˵��ַ�,����ո��������֮���
                        if (SkipBitArray[*subItor])
                        {
                            ++skipCount;
                            continue;
                        }
                        ///�����⵽��ǰ�Ĵ����������д��е�λ����Ϣ��û�д��ڵ�iλ�ģ���������������
                        if ((fastCheck[*subItor] >> Math.Min(i - skipCount, 7)) == 0)
                        {
                            break;
                        }

                        *strItor++ = *subItor;
                        ///����м�⵽���дʵ����һ���ʣ����Ҵ�ʱ�ġ���⵽�����дʻ㡱�ĳ���Ҳ����Ҫ����Ž�һ���鿴��⵽�����дʻ��Ƿ����������
                        if ((fastLength[*itor] >> Math.Min(i - 1 - skipCount, 7)) > 0 && endCache[*subItor])
                        {
                            ///��������ַ��������д��ֵ��д��ڣ����¼�������ж��Ǳ������д��м������������дʵĵ��ʣ���������㷨�޷��޳���������hash�������޳�
                            ///�����㷨�����ڼ��ٴ󲿷ֵıȽ�����
                            if (wordsSet.Contains(new string(dectectedBuffer, 0, (int)(strItor - detectedStrStart))))
                            {
                                int curDectectedStartIndex = (int)(itor - ptext);
                                findResult[curDectectedStartIndex] = i + 1;
                                itor = subItor;
                                break;
                            }
                        }
                        else if (i - skipCount > startCache[*itor] && startCache[*itor] < 0x80)///����������Ըô�Ϊ�׵�һϵ�е����дʻ�����ĳ��ȣ��򲻼����ж�(ǰ���Ǹôʶ�Ӧ���������дʻ�û�г���8���ʵ�)
                        {
                            break;
                        }
                    }
                    ++itor;
                }
            }

            return findResult;
        }
    }
}