using Lib.AssetLoader;
using Lib.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using Logic.Core;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using Framework;
using Packet;

namespace Logic
{
    public class UI_TeamMain_WithoutTeam : UIComponent
    {
        UI_TeamMain_WithoutTeam_Layout m_Layout = new UI_TeamMain_WithoutTeam_Layout();        

        protected override void Loaded()
        {
            m_Layout.Load(gameObject.transform);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
        public override void Show()
        {

            //base.Show();
            m_Layout.Show();

        }
        public override void Hide()
        {
            //base.Hide();
            m_Layout.Hide();
        }

        public void RegisterEvents(UI_TeamMain_Layout.IListener listener, bool state)
        {
            m_Layout.RegisterEvents(listener, state);
        }

    }
}
