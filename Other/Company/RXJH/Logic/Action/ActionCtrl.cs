//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Logic
//{  
//    public class ActionCtrl : Singleton<ActionCtrl>
//    {
//        public enum EActionCtrlStatus
//        {
//            PlayerCtrl, 
//            Auto      
//        }

//        public enum EActionStatus
//        {
//            Idle,
//            Ing,
//        }

//        private EActionCtrlStatus _actionCtrlStatus = EActionCtrlStatus.PlayerCtrl;
//        public EActionCtrlStatus actionCtrlStatus
//        {
//            get => _actionCtrlStatus;
//            set => _actionCtrlStatus = value;
//        }

//        private EActionStatus _actionStatus = EActionStatus.Idle;
//        public EActionStatus actionStatus
//        {
//            get => _actionStatus;
//            set => _actionStatus = value;
//        }

//        Queue<ActionBase> cacheAutoActions = new Queue<ActionBase>();
//        Queue<ActionBase> executeAutoActions = new Queue<ActionBase>();

//        private ActionBase _currentPlayerCtrlAction;
//        public ActionBase currentPlayerCtrlAction
//        {
//            get => _currentPlayerCtrlAction;
//            set => _currentPlayerCtrlAction = value;
//        }

//        private ActionBase _currentAutoAction;
//        public ActionBase currentAutoAction
//        {
//            get => _currentAutoAction;
//            set => _currentAutoAction = value;
//        }

//        ActionBase lastAutoAction;

//        private static bool _actionExecuteLockFlag = false;
//        public static bool ActionExecuteLockFlag
//        {
//            get => _actionExecuteLockFlag;
//            set => _actionExecuteLockFlag = value;
//        }

//        public ActionBase CreateAction(Type type)
//        {
//            ActionBase actionBase = PoolManager.Fetch(type) as ActionBase;
//            return actionBase;
//        }

//        bool ActionCtrlCondition()
//        {
//            return true;
//        }

//        public void ExecutePlayerCtrlAction(ActionBase actionBase)
//        {
//            if (actionBase == null)
//                return;

//            if (ActionExecuteLockFlag)
//                return;

//            if (!ActionCtrlCondition())
//                return;

//            Reset();

//            currentPlayerCtrlAction = actionBase;
//            actionCtrlStatus = EActionCtrlStatus.PlayerCtrl;
//            actionStatus = EActionStatus.Ing;
//            currentPlayerCtrlAction.Execute();
//        }

//        public void Reset()
//        {

//        }
//    }
//}