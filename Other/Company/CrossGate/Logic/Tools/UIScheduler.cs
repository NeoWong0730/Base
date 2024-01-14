using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Lib.Core;

namespace Logic.Core
{
    public enum EUIPopType
    {
        WhenAnyWhere,    // 任何地方弹出
        WhenMaininterfaceOpenning,     // 主界面处于打开的时候才能弹出
        WhenMaininterfaceRealOpenning,     // 主界面处于打开的时候才能弹出
        WhenMaininterfaceIsTop,     // 主界面在最上层
        WhenLastPopedUIClosed,     // 当上一个从sequence中弹出的被关闭之后，才能弹出
    }

    public class UISequenceElement
    {
        public EUIID ui { get; protected set; }
        public System.Object arg { get; protected set; } = null;
        public Action closeAction { get; protected set; } = null;
        public bool immediate { get; protected set; } = false;

        // 什么时候可以进行弹出的回调判断
        public Func<bool> onPredicated { get; private set; } = delegate { return true; };

        public UISequenceElement(EUIID ui, Func<bool> onPredicated = null) { Reset(ui, onPredicated, null, null, false); }
        public UISequenceElement(EUIID ui, Func<bool> onPredicated, System.Object arg = null, Action closeAction = null, bool immediate = false) { Reset(ui, onPredicated, arg, closeAction, immediate); }
        public UISequenceElement Reset(EUIID ui, Func<bool> onPredicated, System.Object arg = null, Action closeAction = null, bool immediate = false)
        {
            this.ui = ui;
            this.onPredicated = onPredicated ?? this.onPredicated;
            this.arg = arg;
            this.closeAction = closeAction;
            this.immediate = immediate;
            return this;
        }
        public UISequenceElement Clear() { return Reset((EUIID)0, null); }

        public bool CanPop() { return onPredicated.Invoke(); }
    }

    // 其实就是一个命令队列的继承实现
    public static class UIScheduler
    {
        // 用于简化onPredicated的书写
        public static readonly Dictionary<EUIPopType, Func<bool>> popTypes = new Dictionary<EUIPopType, Func<bool>>(new FastEnumIntEqualityComparer<EUIPopType>())
        {
            { EUIPopType.WhenAnyWhere, () => { return true; } },
            { EUIPopType.WhenMaininterfaceOpenning, () => {
                return UIManager.IsOpen(EUIID.UI_Menu);
            } },
            { EUIPopType.WhenMaininterfaceRealOpenning, () => {
                return UIManager.IsVisibleAndOpen(EUIID.UI_Menu);
            } },
            //{ EUIPopType.WhenMaininterfaceIsTop, () => {
            //    return UIManager.IsTop((int)EUIID.UI_Menu);
            //} },
            { EUIPopType.WhenLastPopedUIClosed, () => {
                return !UIManager.IsVisibleAndOpen(lastPopedUI);
            } },
        };

        public const float TimeGap = 0.05f;
        private static Queue<UISequenceElement> queue = new Queue<UISequenceElement>();

        private static float lastTime;
        private static EUIID lastPopedUI = (EUIID)99999999;
        
        public static void OnLogin()
        {
            queue.Clear();
        }

        public static void Push(EUIID ui, System.Object arg = null, Action closeAction = null, bool immediate = false, Func<bool> onPredicated = null)
        {
            queue.Enqueue(new UISequenceElement(ui, onPredicated, arg, closeAction, immediate));
        }
        public static void Push(int ui, System.Object arg = null, Action closeAction = null, bool immediate = false,
            Func<bool> onPredicated = null)
        {
            Push((EUIID)ui, arg, closeAction, immediate, onPredicated);
        }
        public static UISequenceElement Pop()
        {
            return queue.Dequeue();
        }
        public static UISequenceElement Top()
        {
            return queue.Peek();
        }

        public static void Update()
        {
            if (queue.Count > 0)
            {
                float now = UnityEngine.Time.time;
                if (now - lastTime <= TimeGap) { return; }
                lastTime = now;

                UISequenceElement top = Top();
                if (top != null && top.CanPop())
                {
                    UIManager.OpenUI(top.ui, top.immediate, top.arg);
                    UIManager.SendMsg(top.ui, top.arg);
                        
                    lastPopedUI = (EUIID)top.ui;
                    Pop();
                }
            }
            else
            {
                lastPopedUI = (EUIID)99999999;
            }
        }
    }
}
