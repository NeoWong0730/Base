using System;
using System.Collections.Generic;

public enum EBTStatus
{
    Failure,
    Success,
    Running,
    Abort
}

public abstract class BTNode
{
    public virtual int UpdateRate { get { return 1; } }

    public abstract EBTStatus Tick();
}

// 复合节点
public abstract class BTBranch : BTNode
{
    // 复合节点的精华： 执行完毕一次之后，会记录当前执行的是第几个子节点，以及当前的状态。
    public int childIndex { get; protected set; }
    public List<BTNode> children { get; protected set; } = new List<BTNode>();

    public virtual BTBranch Init(params BTNode[] children)
    {
        foreach (var child in children)
        {
            if (child != null)
            {
                this.children.Add(child);
            }
        }
        return this;
    }

    public virtual void ResetChildren()
    {
        childIndex = 0;
        foreach (var child in children)
        {
            (child as BTBranch)?.ResetChildren();
        }
    }
    public void Clear()
    {
        childIndex = 0;
        children.Clear();
    }
}

// 所有子节点全部执行完毕，才success
public abstract class BTBlock : BTBranch
{
    public override EBTStatus Tick()
    {
        switch (children[childIndex].Tick())
        {
            case EBTStatus.Running:
                return EBTStatus.Running;
            default:
                childIndex++;
                if (childIndex == children.Count)
                {
                    childIndex = 0;
                    return EBTStatus.Success;
                }
                return EBTStatus.Running;
        }
    }
}

public class BTSequence : BTBranch
{
    public override EBTStatus Tick()
    {
        EBTStatus childState = children[childIndex].Tick();
        switch (childState)
        {
            case EBTStatus.Success:
                // 之后成功的时候才会转移到下一个子节点执行
                ++childIndex;
                if (childIndex == children.Count)
                {
                    childIndex = 0;
                    return EBTStatus.Success;
                }
                else
                {
                    return EBTStatus.Running;
                }
            case EBTStatus.Failure:
                childIndex = 0;
                return EBTStatus.Failure;
            case EBTStatus.Running:
                return EBTStatus.Running;
            case EBTStatus.Abort:
                childIndex = 0;
                return EBTStatus.Abort;
        }

        throw new Exception("This should never happen, but clearly it has.");
    }
}

public class BTSelector : BTBranch
{
    public override EBTStatus Tick()
    {
        EBTStatus childState = children[childIndex].Tick();

        switch (childState)
        {
            case EBTStatus.Success:
                childIndex = 0;
                return EBTStatus.Success;
            case EBTStatus.Failure:
                childIndex++;
                if (childIndex == children.Count)
                {
                    childIndex = 0;
                    return EBTStatus.Failure;
                }
                else
                {
                    return EBTStatus.Running;
                }
            case EBTStatus.Running:
                return EBTStatus.Running;
            case EBTStatus.Abort:
                childIndex = 0;
                return EBTStatus.Abort;
        }

        throw new System.Exception("This should never happen, but clearly it has.");
    }
}

// root功能虽然和sequence类似，但是root控制了整个行为树的stop,resume等操作，而sequence不能提供这些功能，所以必须存在root
public class BTRoot : BTBlock
{
    // 开关
    private bool onOff = true;
    // 结束之后是否循环
    public bool repeated = false;

    public BTRoot(bool repeated) { this.repeated = repeated; }

    public override EBTStatus Tick()
    {
        if (!onOff) return EBTStatus.Abort;
        while (true)
        {
            switch (children[childIndex].Tick()) 
            {
                case EBTStatus.Running:
                    return EBTStatus.Running;
                case EBTStatus.Abort:
                    Stop();
                    return EBTStatus.Abort;

                // 成功 or 失败
                default:
                    childIndex++;
                    if (childIndex == children.Count)
                    {
                        childIndex = 0;
                        if (!repeated)
                        {
                            Stop();
                        }
                        return EBTStatus.Success;
                    }
                    continue;
            }
        }
    }

    public void Resume() { onOff = true; }
    public void Stop() 
    {
         onOff = false; 
         Clear();
    }
}

public abstract class BTAction : BTNode
{
    public abstract bool IsCompleted();
}

public class BTSimpleAction : BTNode
{
    public Action action;

    public BTSimpleAction(Action action) { this.action = action; }

    public override EBTStatus Tick()
    {
        action?.Invoke();
        return EBTStatus.Success;
    }
}

public abstract class BTSimpleCondition : BTNode
{
    public Func<bool> ifFunc;

    public BTSimpleCondition(Func<bool> ifFunc) { this.ifFunc = ifFunc; }

    public override EBTStatus Tick()
    {
        bool result = true;
        if (ifFunc != null)
        {
            result = ifFunc.Invoke();
        }

        EBTStatus retStatus  = result ? EBTStatus.Success : EBTStatus.Failure;
        return retStatus;
    }
}

public class BTConditions : BTBlock
{
    public Func<bool> ifFunc;
    private bool hasExeced = false;

    public BTConditions(Func<bool> ifFunc)
    {
        this.ifFunc = ifFunc;
    }

    public override EBTStatus Tick()
    {
        if (!hasExeced)
        {
            hasExeced = ifFunc();
        }
        if (hasExeced)
        {
            EBTStatus result = base.Tick();
            if (result == EBTStatus.Running)
            {
                return EBTStatus.Running;
            }
            else
            {
                hasExeced = false;
                return result;
            }
        }
        else
        {
            return EBTStatus.Failure;
        }
    }
}

