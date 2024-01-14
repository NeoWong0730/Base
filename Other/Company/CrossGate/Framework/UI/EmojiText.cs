using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.EventSystems;
using Framework;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EmojiText : Text, IPointerClickHandler
{
    public class UITextRichInfo
    {
        /// <summary>
        /// 文本起始索引
        /// </summary>
        public int index;
        /// <summary>
        /// 文本长度
        /// </summary>
        public int count;
        /// <summary>
        /// 数据
        /// </summary>
        public string data;
        /// <summary>
        /// 包围盒
        /// </summary>
        public List<Rect> rects;
        public Color color;
        /// <summary>
        /// 1 = 表情
        /// 2 = 超链接
        /// </summary>
        public int richType;
    }
   
    //(^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$)
    //@"<[a-zA-Z]+.*?>([\s\S]*?)</[a-zA-Z]*?>"
    //@"<(?<tag>[^\s>]+)[^>]*>(.|\n)*?</\k<tag>>"
    //public static readonly Regex mEmojiRegex = new Regex(@"<quad emoji=([^\s]*)(.*?)/>", RegexOptions.Singleline);
    //@"<color=(.*?) a=(.*?)>(.*?)</color>|<quad emoji=([^\s]*)(.*?)/>"
    //@"<color=#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{8}) a=(.*?)>(.*?)</color>|<quad (emoji)=([^\s]*)(.*?)/>"
    public static readonly Regex mEmojiRegex = new Regex(@"<quad emoji=(.*?)/>", RegexOptions.Singleline);
    public static readonly Regex mHrefRegex = new Regex(@"<color=#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{8}) a=(.*?)>(.*?)</color>", RegexOptions.Singleline);

    public UnityEngine.Events.UnityAction<System.String> onHrefClick;

    [SerializeField]
    private EmojiAsset _emojiAsset;
    public EmojiAsset emojiAsset
    {
        get
        {
            return _emojiAsset;
        }
        set
        {
            if (_emojiAsset != value)
            {
                _emojiAsset = value;
                material = emojiAsset?.material;
            }
        }
    }
    
    private List<UITextRichInfo> _RichInfos = null;

    protected override void Awake()
    {
        material = _emojiAsset?.material;
        base.Awake();
    }

    public override void SetVerticesDirty()
    {
        base.SetVerticesDirty();
        //ParseEmoji();
        ParseRich();
    }
    /*
    private void ParseEmoji()
    {
        MatchCollection matchs = mEmojiRegex.Matches(m_Text);
        int matchsCount = matchs.Count;

        if (_emojiInfos == null)
        {
            _emojiInfos = new List<UITextRichInfo>(matchsCount);
        }
        else
        {
            _emojiInfos.Clear();
            if (_emojiInfos.Capacity < matchsCount)
            {
                _emojiInfos.Capacity = matchsCount;
            }
        }

        for (int i = 0; i < matchsCount; ++i)
        {
            Match match = matchs[i];
            UITextRichInfo richInfo = new UITextRichInfo();
            richInfo.index = match.Index;
            richInfo.count = match.Length;            
            richInfo.data = match.Groups[1].Value;
            _emojiInfos.Add(richInfo);
        }
    }
    */

    private void ParseRich()
    {
        MatchCollection emojiMatchs = mEmojiRegex.Matches(m_Text);
        MatchCollection hrefMatchs = mHrefRegex.Matches(m_Text);

        if (_RichInfos == null)
        {
            _RichInfos = new List<UITextRichInfo>(emojiMatchs.Count + hrefMatchs.Count);
        }
        else
        {
            _RichInfos.Clear();
            if (_RichInfos.Capacity < emojiMatchs.Count + hrefMatchs.Count)
            {
                _RichInfos.Capacity = emojiMatchs.Count + hrefMatchs.Count;
            }
        }

        int matchsCount = emojiMatchs.Count;
        for (int i = 0; i < matchsCount; ++i)
        {
            Match match = emojiMatchs[i];
            UITextRichInfo richInfo = new UITextRichInfo();
            richInfo.index = match.Index;
            richInfo.count = match.Length;
            richInfo.richType = 1;
            string value = match.Groups[1].Value;
            int index = value.IndexOf(' ');
            if (index < 0)
            {
                richInfo.data = value;
            }
            else
            {
                richInfo.data = value.Remove(index);
            }

            //Debug.Log(match.Value);
            //for (int j = 0; j < match.Groups.Count; ++j)
            //{
            //    //1 2
            //    //4
            //    Debug.Log(j + "  " + match.Groups[j].Value);
            //}

            //richInfo.data = emojiID;
            _RichInfos.Add(richInfo);
        }

        matchsCount = hrefMatchs.Count;
        for (int i = 0; i < matchsCount; ++i)
        {
            Match match = hrefMatchs[i];
            UITextRichInfo richInfo = new UITextRichInfo();
            richInfo.index = match.Index;
            richInfo.count = match.Length;
            richInfo.richType = 2;
            richInfo.data = match.Groups[2].Value;
            ColorUtility.TryParseHtmlString("#" + match.Groups[1].Value, out Color color);
            richInfo.color = color;

            //Debug.Log(match.Value);
            //for (int j = 0; j < match.Groups.Count; ++j)
            //{
            //    //1 2
            //    //4
            //    Debug.Log(j + "  " + match.Groups[j].Value);
            //}

            //richInfo.data = emojiID;
            int index = 0;
            while (index < _RichInfos.Count)
            {
                if (_RichInfos[index].index > richInfo.index)
                {
                    break;
                }
                ++index;
            }
            _RichInfos.Insert(index, richInfo);
        }
    }

    readonly UIVertex[] m_TempVerts = new UIVertex[4];
    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        if (!supportRichText)
        {
            base.OnPopulateMesh(toFill);
            return;
        }

        if (font == null)
            return;

        // We don't care if we the font Texture changes while we are doing our Update.
        // The end result of cachedTextGenerator will be valid for this instance.
        // Otherwise we can get issues like Case 619238.
        m_DisableFontTextureRebuiltCallback = true;

        Vector2 extents = rectTransform.rect.size;

        var settings = GetGenerationSettings(extents);
        cachedTextGenerator.PopulateWithErrors(text, settings, gameObject);//重置网格

        // Apply the offset to the vertices
        IList<UIVertex> verts = cachedTextGenerator.verts;
        float unitsPerPixel = 1 / pixelsPerUnit;
        int vertCount = verts.Count;

        // We have no verts to process just return (case 1037923)
        if (vertCount <= 0)
        {
            toFill.Clear();
            return;
        }

        Vector2 roundingOffset = new Vector2(verts[0].position.x, verts[0].position.y) * unitsPerPixel;
        roundingOffset = PixelAdjustPoint(roundingOffset) - roundingOffset;
        toFill.Clear();

        if (roundingOffset != Vector2.zero)
        {
            for (int i = 0; i < vertCount; ++i)
            {
                int tempVertsIndex = i & 3;
                m_TempVerts[tempVertsIndex] = verts[i];
                m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
                if (tempVertsIndex == 3)
                    toFill.AddUIVertexQuad(m_TempVerts);
            }
        }
        else
        {
            for (int i = 0; i < vertCount; ++i)
            {
                int tempVertsIndex = i & 3;
                m_TempVerts[tempVertsIndex] = verts[i];
                m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                if (tempVertsIndex == 3)
                    toFill.AddUIVertexQuad(m_TempVerts);
            }
        }

        if (_RichInfos.Count > 0)
        {
            UIVertex vertex = UIVertex.simpleVert;

            IList<UICharInfo> uICharInfos = cachedTextGenerator.characters;
            IList<UILineInfo> uILineInfos = cachedTextGenerator.lines;

            int meshIndex = -1;

            int emojiInfoIndex = 0;
            int lineIndex = 0;

            int currentBoxLine = lineIndex;

            UITextRichInfo uiEmojiInfo = _RichInfos[emojiInfoIndex];
            //矩形区域需要重新计算
            if (uiEmojiInfo.rects != null)
            {
                uiEmojiInfo.rects.Clear();
            }

            UILineInfo uILineInfo = uILineInfos[lineIndex];
            //int lineHeight = uILineInfo.height;
            float middleY = (uILineInfos[lineIndex].topY - uILineInfos[lineIndex].height * 0.5f) * unitsPerPixel;

            int uICharInfosCount = uICharInfos.Count;
            for (int i = 0; i < uICharInfosCount; ++i)
            {
                while (i >= uiEmojiInfo.index + uiEmojiInfo.count)
                {
                    ++emojiInfoIndex;
                    if (emojiInfoIndex < _RichInfos.Count)
                    {
                        uiEmojiInfo = _RichInfos[emojiInfoIndex];
                        //矩形区域需要重新计算
                        if (uiEmojiInfo.rects != null)
                        {
                            uiEmojiInfo.rects.Clear();
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                //if (emojiInfoIndex >= _RichInfos.Count)
                //{
                //    break;
                //}

                if (uICharInfos[i].charWidth == 0 || m_Text[i] == ' ')
                {
                    continue;
                }

                //获取当前所在行
                while (lineIndex + 1 < uILineInfos.Count && i >= uILineInfos[lineIndex + 1].startCharIdx)
                {
                    ++lineIndex;
                    middleY = (uILineInfos[lineIndex].topY - uILineInfos[lineIndex].height * 0.5f) * unitsPerPixel;
                }
                
                //lineHeight = uILineInfos[lineIndex].height;

                //当前的MeshIndex
                ++meshIndex;
                int vertStart = meshIndex * 4;
                toFill.PopulateUIVertex(ref vertex, vertStart);
                m_TempVerts[3] = vertex;//0
                toFill.PopulateUIVertex(ref vertex, vertStart + 1);
                m_TempVerts[2] = vertex;//1
                toFill.PopulateUIVertex(ref vertex, vertStart + 2);
                m_TempVerts[1] = vertex;//2
                toFill.PopulateUIVertex(ref vertex, vertStart + 3);
                m_TempVerts[0] = vertex;//3

                if (i >= uiEmojiInfo.index && i < uiEmojiInfo.index + uiEmojiInfo.count)
                {
                    if (uiEmojiInfo.richType == 1)
                    {
                        int emojiID = 0;
                        int.TryParse(uiEmojiInfo.data, out emojiID);

                        if (emojiAsset.TryGetValue((uint)emojiID, out EmojiInfo emojiInfo))
                        {
                            //0-1 4-5
                            //|\| |\|
                            //3-2 7-6

                            //EmojiInfo 
                            float pixelOffset = emojiInfo.size;
                            m_TempVerts[0].uv1 = new Vector2(emojiInfo.x, emojiInfo.y);//3
                            m_TempVerts[1].uv1 = new Vector2(emojiInfo.x + pixelOffset, emojiInfo.y);//2
                            m_TempVerts[2].uv1 = new Vector2(emojiInfo.x + pixelOffset, emojiInfo.y + pixelOffset);//1
                            m_TempVerts[3].uv1 = new Vector2(emojiInfo.x, emojiInfo.y + pixelOffset);//0
                        }
                        else
                        {
                            //表情错误的表情为空白
                            m_TempVerts[0].uv1 = Vector2.zero;
                            m_TempVerts[1].uv1 = Vector2.zero;
                            m_TempVerts[2].uv1 = Vector2.zero;
                            m_TempVerts[3].uv1 = Vector2.zero;

                            m_TempVerts[0].color = Color.clear;
                            m_TempVerts[1].color = Color.clear;
                            m_TempVerts[2].color = Color.clear;
                            m_TempVerts[3].color = Color.clear;
                        }
                    }
                    else if (uiEmojiInfo.richType == 2)
                    {
                        //不是表情的文本需要往上调整位置
                        //float dy = m_TempVerts[3].position.y - m_TempVerts[0].position.y;
                        //float offsety = (lineHeight * unitsPerPixel - dy) / 2;
                        //m_TempVerts[0].position.y += offsety;
                        //m_TempVerts[1].position.y += offsety;
                        //m_TempVerts[2].position.y += offsety;
                        //m_TempVerts[3].position.y += offsety;

                        m_TempVerts[0].color = uiEmojiInfo.color;
                        m_TempVerts[1].color = uiEmojiInfo.color;
                        m_TempVerts[2].color = uiEmojiInfo.color;
                        m_TempVerts[3].color = uiEmojiInfo.color;

                        if (raycastTarget)
                        {
                            if (uiEmojiInfo.rects == null)
                            {
                                uiEmojiInfo.rects = new List<Rect>(1);
                            }

                            Vector3 endPos = m_TempVerts[1].position;

                            if (uiEmojiInfo.rects.Count == 0 || currentBoxLine != lineIndex)
                            {
                                //当没有矩形区域或者换行的时候 添加新的矩形
                                Vector3 startPos = m_TempVerts[3].position;
                                Rect rect = new Rect(startPos.x, endPos.y, endPos.x - startPos.x, startPos.y - endPos.y);
                                uiEmojiInfo.rects.Add(rect);
                            }
                            else
                            {
                                //否则拓展原有矩形
                                Rect rect = uiEmojiInfo.rects[uiEmojiInfo.rects.Count - 1];
                                rect.Set(rect.x, rect.y, endPos.x - rect.x, rect.height);
                                uiEmojiInfo.rects[uiEmojiInfo.rects.Count - 1] = rect;
                            }
                        }

                        currentBoxLine = lineIndex;
                    }
                }
                //else
                //{
                //    //不是表情的文本需要往上调整位置
                //    float dy = m_TempVerts[3].position.y - m_TempVerts[0].position.y;
                //    float offsety = (lineHeight * unitsPerPixel - dy) / 2;
                //    m_TempVerts[0].position.y += offsety;
                //    m_TempVerts[1].position.y += offsety;
                //    m_TempVerts[2].position.y += offsety;
                //    m_TempVerts[3].position.y += offsety;
                //}

                //文本需要居中对齐
                float currentMiddleY = (m_TempVerts[3].position.y + m_TempVerts[0].position.y) * 0.5f;
                float offsety = middleY - currentMiddleY;
                m_TempVerts[0].position.y += offsety;
                m_TempVerts[1].position.y += offsety;
                m_TempVerts[2].position.y += offsety;
                m_TempVerts[3].position.y += offsety;

                toFill.SetUIVertex(m_TempVerts[0], vertStart);
                toFill.SetUIVertex(m_TempVerts[1], vertStart + 1);
                toFill.SetUIVertex(m_TempVerts[2], vertStart + 2);
                toFill.SetUIVertex(m_TempVerts[3], vertStart + 3);
            }
        }

        m_DisableFontTextureRebuiltCallback = false;
    }

    /// <summary>
    /// 点击事件检测是否点击到超链接文本
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_RichInfos != null)
        {
            Vector2 lp;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, eventData.pressEventCamera, out lp);


            for (int i = 0; i < _RichInfos.Count; ++i)
            {
                List<Rect> rects = _RichInfos[i].rects;
                if (rects != null)
                {
                    for (int j = 0; j < rects.Count; ++j)
                    {
                        if (rects[j].Contains(lp))
                        {
                            Lib.Core.DebugUtil.LogFormat(Lib.Core.ELogType.eChat, "点击了: {0}", _RichInfos[i].data);
                            if (onHrefClick != null)
                            {
                                onHrefClick(_RichInfos[i].data);
                            }
                            return;
                        }
                    }
                }
            }
        }        
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(EmojiText))]
public class EmojiTextChatInsprctor : UnityEditor.UI.TextEditor
{
    EmojiText _emojiText = null;
    EmojiText emojiText
    {
        get
        {
            if (_emojiText == null)
                _emojiText = target as EmojiText;
            return _emojiText;
        }
    }

    public override void OnInspectorGUI()
    {
        emojiText.emojiAsset = (EmojiAsset)EditorGUILayout.ObjectField(emojiText.emojiAsset, typeof(EmojiAsset), false);
        base.OnInspectorGUI();
    }
}
#endif

