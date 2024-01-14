using System.Collections;
using System.Collections.Generic;
using Logic;
using UnityEngine;

public class CrossCell
{
    public bool use;
    
    // from 0
    public int index;
    // from 0
    public int row { get; private set; }
    // from 0
    public int col { get; private set; }

    private List<int> _neighbours;
    public List<int> neighbours
    {
        get { return _neighbours; }
    }

    public CrossCell(int index, int row, int col)
    {
        this.index = index;
        this.row = row;
        this.col = col;
        this.use = false;
        
        this._neighbours = new List<int>();
    }

    public CrossCell SetUse(bool toUse)
    {
        use = toUse;
        return this;
    }

    public CrossCell CalaulateNeighbours(CrossGrid grid, int totalCount, int countPerLine)
    {
        int index0 = index - 1;
        int index1 = index - countPerLine;
        int index2 = index + 1;
        int index3 = index + countPerLine;
            
        _neighbours.Clear();
        int line = index / countPerLine;
        CrossCell outer;
        if (CodeHelper.IsIndexValid(index0, totalCount) && index0 / countPerLine == line)
        {
            if (grid.TryGetCell(index0, out outer) && outer.use)
            {
                _neighbours.Add(index0);
            }
        }
        if (CodeHelper.IsIndexValid(index1, totalCount) && index1 / countPerLine == line - 1)
        {
            if (grid.TryGetCell(index1, out outer) && outer.use)
            {
                _neighbours.Add(index1);
            }
        }
        if (CodeHelper.IsIndexValid(index2, totalCount) && index2 / countPerLine == line)
        {
            if (grid.TryGetCell(index2, out outer) && outer.use)
            {
                _neighbours.Add(index2);
            }
        }
        if (CodeHelper.IsIndexValid(index3, totalCount) && index3 / countPerLine == line + 1)
        {
            if (grid.TryGetCell(index3, out outer) && outer.use)
            {
                _neighbours.Add(index3);
            }
        }

        return this;
    }

    public bool IsNeighbour(CrossCell cell)
    {
        return IsNeighbour(cell.index);
    }
    public bool IsNeighbour(int index)
    {
        return _neighbours.Contains(index);
    }

    /// <summary>
    /// 十字架结构：上下左右的方位
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="targetCell"></param>
    /// <returns></returns>
    public EDir GetRelativeDir(CrossGrid grid, CrossCell targetCell)
    {
        EDir dir = EDir.SelfOrUnknown;
        if (targetCell.row == this.row)
        {
            if (targetCell.col > this.col)
            {
                dir = EDir.Right;
            }
            else if (targetCell.col < this.col)
            {
                dir = EDir.Left;
            }
        }
        else
        {
            if (targetCell.col == this.col)
            {
                if (targetCell.row > this.row)
                {
                    dir = EDir.Down;
                }
                else if (targetCell.row < this.row)
                {
                    dir = EDir.Up;
                }
            }
        }
        return dir;
    }
}

// 十字架
public class CrossGrid
{
    public int totalCount;
    public int countPerLine;
    public int usingCount;

    private List<CrossCell> cells;

    public CrossCell this[int index]
    {
        get { return cells[index]; }
    }

    public CrossGrid(int totalCount, int countPerLine)
    {
        this.totalCount = totalCount;
        this.countPerLine = countPerLine;
        this.cells = new List<CrossCell>(totalCount);

        for (int i = 0; i < totalCount; ++i)
        {
            int row = i / countPerLine;
            int col = i % countPerLine;
            CrossCell cell = new CrossCell(i, row, col);
            cells.Add(cell);
        }
    }

    public CrossGrid SetAllUse(bool toUse)
    {
        foreach (var cell in cells)
        {
            cell.SetUse(toUse);
        }
        return this;
    }
    public CrossGrid SetUsingCount(int count)
    {
        if (0 < count && count <= totalCount)
        {
            usingCount = count;
        }
        return this;
    }

    public CrossGrid ResetUseCells(List<int> useCellIndexs)
    {
        SetAllUse(false);
        foreach (var cellIndex in useCellIndexs)
        {
            if (CodeHelper.IsIndexValid(cellIndex, totalCount))
            {
                cells[cellIndex].SetUse(true);
            }
        }
        return this;
    }

    public int GetIndex(int row, int col)
    {
        return countPerLine * row + col;
    }

    public bool TryGetCell(int index, out CrossCell cell)
    {
        if (CodeHelper.IsIndexValid(index, totalCount))
        {
            cell = cells[index];
            return true;
        }

        cell = default;
        return false;
    }
}
