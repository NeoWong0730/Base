using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Lib.Core;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Knowledge_Gleanings_Left
    {

        private Transform transform;

        public Widget_List_Left03 left03;

        private IListener _listener;

        public void Init(Transform trans)
        {
            transform = trans;

            var itemList = Sys_Knowledge.Instance.GetGleaningsTypeData();

            left03 = new Widget_List_Left03(transform, OnClickItem, itemList);
            left03.UpdateRedPointState();

            ProcessEvents(true);
        }

        public void OnShow()
        {
            transform.gameObject.SetActive(true);
        }

        public void OnHide()
        {
            transform.gameObject.SetActive(false);
        }

        public void OnDestroy()
        {
            ProcessEvents(false);
        }

        private void ProcessEvents(bool register)
        {
            Sys_Knowledge.Instance.eventEmitter.Handle(Sys_Knowledge.EEvents.OnDelNewKnowledgeNtf, OnDelNewKnowledge, register);
        }

        private void OnClickItem(uint arg1, uint arg2)
        {
            //Debug.LogErrorFormat("type = {0}", arg1);
            //Debug.LogErrorFormat("subType = {0}", arg2);
            _listener?.OnSelect(arg1, arg2);
        }

        private void OnDelNewKnowledge()
        {
            left03.UpdateRedPointState();
        }

        public void OnSelect(uint typeId)
        {
            left03.OnSelect(typeId);
        }

        public void Register(IListener listener)
        {
            _listener = listener;
        }

        public interface IListener
        {
            void OnSelect(uint typeId, uint subTypeId);
        }
    }
}


