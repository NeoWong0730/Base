using UnityEngine;

namespace Logic.Core
{
    public abstract class UIBase : Framework.Core.UI.FUIBase
    {
        public TComponent AddComponent<TComponent>(Transform go) where TComponent : UIComponent, new()
        {
            TComponent component = new TComponent();
            component.Init(go);
            return component;
        }

        protected void OnButtonClick(int a, int b, int c, int d)
        {
            switch (a)
            {
                case 0:
                    {
                        uint ruleID = (uint)b;
                        if (ruleID == 0)
                        {
                            ruleID = (uint)nID;
                        }
                        Table.CSVUIRule.Data csvRule = Table.CSVUIRule.Instance.GetConfData(ruleID);
                        UIManager.OpenUI(EUIID.UI_HelpRule, false, csvRule.ruleIds);
                    }
                    break;
                case 1:
                    UIManager.CloseUI((EUIID)nID, false, true);
                    break;
                default:
                    break;
            }
        }

        protected sealed override void _DoLoaded()
        {
            base._DoLoaded();
            //new UIHelpRule(this.nID, this.transform).TryLoad();
            if (this.transform.TryGetComponent<ButtonList>(out ButtonList buttonList))
            {
                buttonList.onClick += OnButtonClick;
            }
        }
        
        protected sealed override void _DoBeginEnter()
        {
            base._DoBeginEnter();
            UIManager.HitPointShow((EUIID)nID);
        }

        protected sealed override void _DoEndExit()
        {
            base._DoEndExit();
            UIManager.HitPointHide((EUIID)nID, nShowTimePoint);
        }        

#if UNITY_EDITOR
        //仅仅是再编辑器模式下 防止写代码的时候被继承
        protected sealed override void _DoEndEnter()
        {
            base._DoEndEnter();
        }

        protected sealed override void _DoBeginExit()
        {
            base._DoBeginExit();
        }

        protected sealed override void _CloseOrDestroy()
        {
            base._CloseOrDestroy();
        }        
#endif

        protected void CloseSelf(bool immediate = false)
        {
            mOwnerStack?.HideUI(nID, Framework.Core.UI.EUIState.Destroy, immediate);
        }

        //以下生命周期重载函数 是为了防止子类中调用 base.xxx()导致再次对框架层代码的调用
        //并且当子类并没有重写以下函数的时候 会在机制上避免对以下函数的调用 节省空的ILRuntime函数的调用， 所以以下不要写任何内容
#if ILRUNTIME_MODE
        public new void OnForeQuit()
        {
            UIManager.HitPointHide((EUIID)nID, nShowTimePoint, "", false);
        }
        /*
        public new virtual void OnSetData(object arg) { }
        protected new virtual void OnInit() { }
        protected new virtual void OnOpen(object arg) { }
        protected new virtual void OnLoaded() { }
        protected new virtual void OnUpdate() { }
        protected new virtual void OnLateUpdate(float dt, float usdt) { }
        protected new virtual void OnOpened() { }
        protected new virtual void OnShow() { }
        protected new virtual void OnShowEnd() { }
        protected new virtual void OnHideStart() { }
        protected new virtual void OnHide() { }
        protected new virtual void OnClose() { }
        protected new virtual void OnDestroy() { }
        protected new virtual void ProcessEvents(bool toRegister) { }
        protected new virtual void ProcessEventsForEnable(bool toRegister) { }
        */
#else
        public sealed override void OnForeQuit()
        {
            UIManager.HitPointHide((EUIID)nID, nShowTimePoint, "", false);
        }
#endif
    }
}
