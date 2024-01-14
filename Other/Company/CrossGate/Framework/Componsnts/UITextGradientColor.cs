using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


//和文本的 Shadow 和 outline 效果配合 ， 渲染顺序不一样，效果也不一样

[AddComponentMenu("UI/Effects/Text Gradient Color")]
[RequireComponent(typeof(Text))]
public class UITextGradientColor : BaseMeshEffect
{
    public Gradient gradientColor = new Gradient();
    //是否垂直方向
    public bool isVertical = true;
    //是否叠加原有颜色
    public bool isMultiplyTextColor = false;
    protected UITextGradientColor()
    {
    }

    private void ModifyVertices(VertexHelper vh)
    {
        List<UIVertex> verts = new List<UIVertex>(vh.currentVertCount);
        vh.GetUIVertexStream(verts);
        vh.Clear();

        //每个字母 分为两个三角形,6个顶点，如下图 0和5位置相同 2和3位置相同
        /**
         *   5-0 ---- 1
         *    | \    |
         *    |  \   |
         *    |   \  |
         *    |    \ |
         *    4-----3-2
         **/

        int step = 6;
        for (int i = 0; i < verts.Count; i += step)
        {

            UIVertex start1, start2, end1, end2, current1, current2;
            if (isVertical)
            {
                start1 = verts[i + 0];
                start2 = verts[i + 1];
                end1 = verts[i + 4];
                end2 = verts[i + 3];
            }
            else
            {
                start1 = verts[i + 0];
                start2 = verts[i + 4];
                end1 = verts[i + 1];
                end2 = verts[i + 2];
            }

            for (int j = 0; j < gradientColor.colorKeys.Length; j++)
            {
                GradientColorKey colorKey = gradientColor.colorKeys[j];
                if (j == 0)
                {
                    multiplyColor(ref start1, colorKey.color);
                    multiplyColor(ref start2, colorKey.color);
                }
                else if (j == gradientColor.colorKeys.Length - 1)
                {
                    multiplyColor(ref end1, colorKey.color);
                    multiplyColor(ref end2, colorKey.color);

                    //right
                    vh.AddVert(start1);
                    vh.AddVert(start2);
                    vh.AddVert(end2);

                    //left
                    vh.AddVert(end2);
                    vh.AddVert(end1);
                    vh.AddVert(start1);

                }
                else
                {
                    // create right
                    current2 = CreateVertexByTime(start2, end2, colorKey.time);
                    vh.AddVert(start1);
                    vh.AddVert(start2);
                    vh.AddVert(current2);

                    // create left
                    current1 = CreateVertexByTime(start1, end1, colorKey.time);
                    vh.AddVert(current2);
                    vh.AddVert(current1);
                    vh.AddVert(start1);

                    start1 = current1;
                    start2 = current2;
                }
            }
        }

        //添加三角形

        //每个字母的顶点数量
        int stepVertCount = (gradientColor.colorKeys.Length - 1) * 2 * 3;
        for (int i = 0; i < vh.currentVertCount; i += stepVertCount)
        {
            for (int m = 0; m < stepVertCount; m += 3)
            {
                vh.AddTriangle(i + m + 0, i + m + 1, i + m + 2);
            }
        }
    }

    private UIVertex multiplyColor(ref UIVertex vertex, Color color)
    {
        if (isMultiplyTextColor)
            vertex.color = Multiply(vertex.color, color);
        else
            vertex.color = color;
        return vertex;
    }

    public static Color32 Multiply(Color32 a, Color32 b)
    {
        a.r = (byte)((a.r * b.r) >> 8);
        a.g = (byte)((a.g * b.g) >> 8);
        a.b = (byte)((a.b * b.b) >> 8);
        a.a = (byte)((a.a * b.a) >> 8);
        return a;
    }

    //根据比例创建顶点 （time这里是gradientColor里的比例）
    private UIVertex CreateVertexByTime(UIVertex start, UIVertex end, float time)
    {
        UIVertex center = new UIVertex();
        center.normal = Vector3.Lerp(start.normal, end.normal, time);
        center.position = Vector3.Lerp(start.position, end.position, time);
        center.tangent = Vector4.Lerp(start.tangent, end.tangent, time);
        center.uv0 = Vector2.Lerp(start.uv0, end.uv0, time);
        center.uv1 = Vector2.Lerp(start.uv1, end.uv1, time);
        center.color = gradientColor.Evaluate(time);

        if (isMultiplyTextColor)
        {
            //multiply color
            var color = Color.Lerp(start.color, end.color, time);
            center.color = Multiply(color, gradientColor.Evaluate(time));
        }
        else
        {
            center.color = gradientColor.Evaluate(time);
        }

        return center;
    }

    #region implemented abstract members of BaseMeshEffect

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!this.IsActive())
        {
            return;
        }

        ModifyVertices(vh);
    }

    #endregion
}