using Unity.Mathematics;
using UnityEngine;

public class TerrainUtil
{
    public static float2 GetIndexPostion(Terrain terrain, int x, int y)
    {
        TerrainData tData = terrain.terrainData;
        float width = tData.size.x;
        float length = tData.size.z;

        //width = length = math.max(width, length);

        float posX = (float)x / tData.detailWidth * width + terrain.GetPosition().x;
        float posY = (float)y / tData.detailHeight * length + terrain.GetPosition().z;

        return new float2(posX, posY);
    }

    public static int2 DetailMapToHeightMapIndex(Terrain terrain, int x, int y)
    {
        TerrainData tData = terrain.terrainData;
        int newX = (int)(x * (float)tData.heightmapResolution / (float)tData.detailResolution);
        int newY = (int)(y * (float)tData.heightmapResolution / (float)tData.detailResolution);
        return new int2(newX, newY);
    }

    /** 
     * Terrain的HeightMap坐标原点在左下角
     *   y
     *   ↑
     *   0 → x
     */

    /// <summary>
    /// 返回Terrain上某一点的HeightMap索引。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="point">Terrain上的某点</param>
    /// <returns>该点在HeightMap中的位置索引</returns>
    public static int[] GetHeightmapIndex(Terrain terrain, Vector3 point)
    {
        TerrainData tData = terrain.terrainData;
        float width = tData.size.x;
        float length = tData.size.z;

        // 根据相对位置计算索引
        int x = (int)((point.x - terrain.GetPosition().x) / width * tData.heightmapResolution);
        int y = (int)((point.z - terrain.GetPosition().z) / length * tData.heightmapResolution);

        return new int[2] { x, y };
    }

    public static int2 GetDetailMapIndex(Terrain terrain, Vector3 point)
    {
        TerrainData tData = terrain.terrainData;
        float width = tData.size.x;
        float length = tData.size.z;

        int2 pos = int2.zero;
        // 根据相对位置计算索引
        pos.x = (int)((point.x - terrain.GetPosition().x) / width * tData.detailWidth);
        pos.y = (int)((point.z - terrain.GetPosition().z) / length * tData.detailHeight);

        return pos;
    }

    /// <summary>
    /// 返回GameObject在Terrain上的相对（于Terrain的）位置。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="go">GameObject</param>
    /// <returns>相对位置</returns>
    public static Vector3 GetRelativePosition(Terrain terrain, GameObject go)
    {
        return go.transform.position - terrain.GetPosition();
    }

    /// <summary>
    /// 返回Terrain上指定点在世界坐标系下的高度。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="point">Terrain上的某点</param>
    /// <param name="vertex">true: 获取最近顶点高度  false: 获取实际高度</param>
    /// <returns>点在世界坐标系下的高度</returns>
    public static float GetPointHeight(Terrain terrain, Vector3 point, bool vertex = false)
    {
        // 对于水平面上的点来说，vertex参数没有影响
        if (vertex)
        {
            // GetHeight得到的是离点最近的顶点的高度
            int[] index = GetHeightmapIndex(terrain, point);
            return terrain.terrainData.GetHeight(index[0], index[1]) + terrain.GetPosition().y;
        }
        else
        {
            // SampleHeight得到的是点在斜面上的实际高度
            return terrain.SampleHeight(point) + terrain.GetPosition().y;
        }
    }

    /// <summary>
    /// 返回Terrain的HeightMap，这是一个 height*width 大小的二维数组，并且值介于 [0.0f,1.0f] 之间。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="xBase">检索HeightMap时的X索引起点</param>
    /// <param name="yBase">检索HeightMap时的Y索引起点</param>
    /// <param name="width">在X轴上的检索长度</param>
    /// <param name="height">在Y轴上的检索长度</param>
    /// <returns></returns>
    public static float[,] GetHeightMap(Terrain terrain, int xBase = 0, int yBase = 0, int width = 0, int height = 0)
    {
        if (xBase + yBase + width + height == 0)
        {
            width = terrain.terrainData.heightmapResolution;
            height = terrain.terrainData.heightmapResolution;
        }

        return terrain.terrainData.GetHeights(xBase, yBase, width, height);
    }

    public static int[,] GetDetailMap(Terrain terrain, int xBase = 0, int yBase = 0, int width = 0, int height = 0, int layer = 0)
    {
        if (xBase + yBase + width + height == 0)
        {
            width = terrain.terrainData.detailWidth;
            height = terrain.terrainData.detailHeight;
        }

        return terrain.terrainData.GetDetailLayer(xBase, yBase, width, height, layer);
    }

    public static float[,,] GetAlphaMap(Terrain terrain, int xBase = 0, int yBase = 0, int width = 0, int height = 0)
    {
        if (xBase + yBase + width + height == 0)
        {
            width = terrain.terrainData.alphamapWidth;
            height = terrain.terrainData.alphamapHeight;
        }

        return terrain.terrainData.GetAlphamaps(xBase, yBase, width, height);
    }

    /// <summary>
    /// 升高Terrain上某点的高度。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="point">Terrain上的点</param>
    /// <param name="opacity">升高的高度</param>
    /// <param name="size">笔刷大小</param>
    /// <param name="amass">当笔刷范围内其他点的高度已经高于笔刷中心点时是否同时提高其他点的高度</param>
    public static void Rise(Terrain terrain, Vector3 point, float opacity, int size, bool amass = true)
    {
        int[] index = GetHeightmapIndex(terrain, point);
        Rise(terrain, index, opacity, size, amass);
    }

    /// <summary>
    /// 升高Terrain上的某点。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="index">HeightMap索引</param>
    /// <param name="opacity">升高的高度</param>
    /// <param name="size">笔刷大小</param>
    /// <param name="amass">当笔刷范围内其他点的高度已经高于笔刷中心点时是否同时提高其他点的高度</param>
    public static void Rise(Terrain terrain, int[] index, float opacity, int size, bool amass = true)
    {
        TerrainData tData = terrain.terrainData;

        int bound = size / 2;
        int xBase = index[0] - bound >= 0 ? index[0] - bound : 0;
        int yBase = index[1] - bound >= 0 ? index[1] - bound : 0;
        int width = xBase + size <= tData.heightmapResolution ? size : tData.heightmapResolution - xBase;
        int height = yBase + size <= tData.heightmapResolution ? size : tData.heightmapResolution - yBase;

        float[,] heights = tData.GetHeights(xBase, yBase, width, height);
        float initHeight = tData.GetHeight(index[0], index[1]) / tData.size.y;
        float deltaHeight = opacity / tData.size.y;

        // 得到的heights数组维度是[height,width]，索引为[y,x]
        ExpandBrush(heights, deltaHeight, initHeight, height, width, amass);
        tData.SetHeights(xBase, yBase, heights);
    }

    /// <summary>
    /// 降低Terrain上某点的高度。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="point">Terrain上的点</param>
    /// <param name="opacity">降低的高度</param>
    /// <param name="size">笔刷大小</param>
    /// <param name="amass">当笔刷范围内其他点的高度已经低于笔刷中心点时是否同时降低其他点的高度</param>
    public static void Sink(Terrain terrain, Vector3 point, float opacity, int size, bool amass = true)
    {
        int[] index = GetHeightmapIndex(terrain, point);
        Sink(terrain, index, opacity, size, amass);
    }

    /// <summary>
    /// 降低Terrain上某点的高度。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="index">HeightMap索引</param>
    /// <param name="opacity">降低的高度</param>
    /// <param name="size">笔刷大小</param>
    /// <param name="amass">当笔刷范围内其他点的高度已经低于笔刷中心点时是否同时降低其他点的高度</param>
    public static void Sink(Terrain terrain, int[] index, float opacity, int size, bool amass = true)
    {
        TerrainData tData = terrain.terrainData;

        int bound = size / 2;
        int xBase = index[0] - bound >= 0 ? index[0] - bound : 0;
        int yBase = index[1] - bound >= 0 ? index[1] - bound : 0;
        int width = xBase + size <= tData.heightmapResolution ? size : tData.heightmapResolution - xBase;
        int height = yBase + size <= tData.heightmapResolution ? size : tData.heightmapResolution - yBase;

        float[,] heights = tData.GetHeights(xBase, yBase, width, height);
        float initHeight = tData.GetHeight(index[0], index[1]) / tData.size.y;
        float deltaHeight = -opacity / tData.size.y;  // 注意负号

        // 得到的heights数组维度是[height,width]，索引为[y,x]
        ExpandBrush(heights, deltaHeight, initHeight, height, width, amass);
        tData.SetHeights(xBase, yBase, heights);
    }

    /// <summary>
    /// 根据笔刷四角的高度来平滑Terrain，该方法不会改变笔刷边界处的Terrain高度。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="point">Terrain上的点</param>
    /// <param name="opacity">平滑灵敏度，值介于 [0.05,1] 之间</param>
    /// <param name="size">笔刷大小</param>
    public static void Smooth(Terrain terrain, Vector3 point, float opacity, int size)
    {
        int[] index = GetHeightmapIndex(terrain, point);
        Smooth(terrain, index, opacity, size);
    }

    /// <summary>
    /// 根据笔刷四角的高度来平滑Terrain，该方法不会改变笔刷边界处的Terrain高度。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="index">HeightMap索引</param>
    /// <param name="opacity">平滑灵敏度，值介于 [0.05,1] 之间</param>
    /// <param name="size">笔刷大小</param>
    public static void Smooth(Terrain terrain, int[] index, float opacity, int size)
    {
        TerrainData tData = terrain.terrainData;
        if (opacity > 1 || opacity <= 0)
        {
            opacity = Mathf.Clamp(opacity, 0.05f, 1);
            Debug.LogError("Smooth方法中的opacity参数的值应该介于 [0.05,1] 之间，强制将其设为：" + opacity);
        }

        // 取出笔刷范围内的HeightMap数据数组
        int bound = size / 2;
        int xBase = index[0] - bound >= 0 ? index[0] - bound : 0;
        int yBase = index[1] - bound >= 0 ? index[1] - bound : 0;
        int width = xBase + size <= tData.heightmapResolution ? size : tData.heightmapResolution - xBase;
        int height = yBase + size <= tData.heightmapResolution ? size : tData.heightmapResolution - yBase;
        float[,] heights = tData.GetHeights(xBase, yBase, width, height);

        // 利用笔刷4角的高度来计算平均高度
        float avgHeight = (heights[0, 0] + heights[0, width - 1] + heights[height - 1, 0] + heights[height - 1, width - 1]) / 4;
        Vector2 center = new Vector2((float)(height - 1) / 2, (float)(width - 1) / 2);
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                // 点到矩阵中心点的距离
                float toCenter = Vector2.Distance(center, new Vector2(i, j));
                float diff = avgHeight - heights[i, j];

                // 判断点在4个三角形区块上的位置
                // 利用相似三角形求出点到矩阵中心点与该点连线的延长线与边界交点的距离
                float d = 0;
                if (i == height / 2 && j == width / 2)  // 中心点
                {
                    d = 1;
                    toCenter = 0;
                }
                else if (i >= j && i <= size - j)  // 左三角区
                {
                    // j/((float)width / 2) = d/(d+toCenter)，求出距离d，其他同理
                    d = toCenter * j / ((float)width / 2 - j);
                }
                else if (i <= j && i <= size - j)  // 上三角区
                {
                    d = toCenter * i / ((float)height / 2 - i);
                }
                else if (i <= j && i >= size - j)  // 右三角区
                {
                    d = toCenter * (size - j) / ((float)width / 2 - (size - j));
                }
                else if (i >= j && i >= size - j)  // 下三角区
                {
                    d = toCenter * (size - i) / ((float)height / 2 - (size - i));
                }

                // 进行平滑时对点进行升降的比例
                float ratio = d / (d + toCenter);
                heights[i, j] += diff * ratio * opacity;
            }
        }

        tData.SetHeights(xBase, yBase, heights);
    }

    /// <summary>
    /// 压平Terrain并提升到指定高度。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="height">高度</param>
    public static void Flatten(Terrain terrain, float height)
    {
        TerrainData tData = terrain.terrainData;
        float scaledHeight = height / tData.size.y;

        float[,] heights = new float[tData.heightmapResolution, tData.heightmapResolution];
        for (int i = 0; i < tData.heightmapResolution; i++)
        {
            for (int j = 0; j < tData.heightmapResolution; j++)
            {
                heights[i, j] = scaledHeight;
            }
        }

        tData.SetHeights(0, 0, heights);
    }

    /// <summary>
    /// 设置Terrain的HeightMap。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="heights">HeightMap</param>
    /// <param name="xBase">X起点</param>
    /// <param name="yBase">Y起点</param>
    public static void SetHeights(Terrain terrain, float[,] heights, int xBase = 0, int yBase = 0)
    {
        terrain.terrainData.SetHeights(xBase, yBase, heights);
    }

    // TODO 
    // public static void SaveHeightmapData(Terrain terrain, string path) {}

    /// <summary>
    /// 扩大笔刷作用范围。
    /// </summary>
    /// <param name="heights">HeightMap</param>
    /// <param name="deltaHeight">高度变化量[-1,1]</param>
    /// <param name="initHeight">笔刷中心点的初始高度</param>
    /// <param name="row">HeightMap行数</param>
    /// <param name="column">HeightMap列数</param>
    /// <param name="amass">当笔刷范围内其他点的高度已经高于笔刷中心点时是否同时提高其他点的高度</param>
    private static void ExpandBrush(float[,] heights, float deltaHeight, float initHeight, int row, int column, bool amass)
    {
        // 高度限制
        float limit = initHeight + deltaHeight;

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (amass) { heights[i, j] += deltaHeight; }
                else  // 不累加高度时
                {
                    if (deltaHeight > 0)  // 升高地形
                    {
                        heights[i, j] = heights[i, j] >= limit ? heights[i, j] : heights[i, j] + deltaHeight;
                    }
                    else  // 降低地形
                    {
                        heights[i, j] = heights[i, j] <= limit ? heights[i, j] : heights[i, j] + deltaHeight;
                    }
                }
            }
        }
    }



    #region 弃用的旧方法
    /*public static*/
    [System.Obsolete]
    void Rise_Old(Terrain terrain, int[] index, float opacity, int size, bool amass = true)
    {
        if (index.Length != 2)
        {
            Debug.LogError("参数错误！");
            return;
        }

        TerrainData tData = terrain.terrainData;

        // heights中存储的是顶点高度，不是斜面的准确高度
        float[,] heights = tData.GetHeights(0, 0, tData.heightmapResolution, tData.heightmapResolution);
        float deltaHeight = opacity / tData.size.y;

        ExpandBrush_Old(heights, index, deltaHeight, size, amass, tData.heightmapResolution, tData.heightmapResolution);
        tData.SetHeights(0, 0, heights);
    }

    /*private static*/
    [System.Obsolete]
    void ExpandBrush_Old(float[,] heights, int[] index, float deltaHeight, int size, bool amass, int xMax, int yMax)
    {
        float limit = heights[index[0], index[1]] + deltaHeight;

        int bound = size / 2;
        for (int offsetX = -bound; offsetX <= bound; offsetX++)
        {
            int x = index[0] + offsetX;
            if (x < 0 || x > xMax) continue;

            for (int offsetY = -bound; offsetY <= bound; offsetY++)
            {
                int y = index[1] + offsetY;
                if (y < 0 || y > yMax) continue;

                if (amass)
                {
                    heights[x, y] += deltaHeight;
                }
                else
                {
                    if (deltaHeight > 0)
                    {
                        // 升高地形
                        heights[x, y] = heights[x, y] >= limit ? heights[x, y] : heights[x, y] + deltaHeight;
                    }
                    else
                    {
                        // 降低地形
                        heights[x, y] = heights[x, y] <= limit ? heights[x, y] : heights[x, y] + deltaHeight;
                    }
                }
            }
        }

        // 平滑方程：y = (cos(x) + 1) / 2;
        //float rad = 180.0f * (smooth / 9) * Mathf.Deg2Rad;
        //float height = (Mathf.Cos(rad) + 1) / 2;
    }
    #endregion
}