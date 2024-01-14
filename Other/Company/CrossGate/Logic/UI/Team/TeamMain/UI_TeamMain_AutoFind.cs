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
    public class UI_TeamMain_AutoFind : UIComponent
    {
        UI_TeamMain_AutoFind_Layout m_Layout = new UI_TeamMain_AutoFind_Layout();        

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

            m_Layout.Show();

            var data = CSVTeam.Instance.GetConfData(Sys_Team.Instance.MatchingTarget);

            string title = data == null ? string.Empty: LanguageHelper.GetTextContent(data.subclass_name);

            m_Layout.SetTitle(title);

        }
        public override void Hide()
        {
            m_Layout.Hide();

        }

        public void RegisterEvents(UI_TeamMain_Layout.IListener listener, bool state)
        {
            m_Layout.RegisterEvents(listener, state);

        }

    }
}
