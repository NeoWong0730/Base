using Framework;
using Lib.Core;
using Logic.Core;
using Packet;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_MagicBook_Tips : UIBase
    {
        private Button closeBtn;
        private Transform view;
        private uint chapterId;
        protected override void OnLoaded()
        {
            closeBtn = transform.Find("Close").GetComponent<Button>();
            closeBtn.onClick.AddListener(() => { UIManager.CloseUI(EUIID.UI_MagicBook_Tips, true); });
            view = transform.Find("ZoneAward/Scroll_View/Viewport");
        }

        protected override void OnOpen(object arg)
        {
            chapterId = Convert.ToUInt32(arg);
        }

        protected override void OnShow()
        {
            Chapter chapter = Sys_MagicBook.Instance.GetSeverChapterByChapterId(chapterId);
            bool isGet = false;
            if (null != chapter)
            {
                isGet = chapter.Awarded;
            }

            FrameworkTool.DestroyChildren(view.gameObject);
            CSVChapterInfo.Data chapterInfoData = CSVChapterInfo.Instance.GetConfData(chapterId);
            if (null != chapterInfoData)
            {
                List<ItemIdCount> temp = CSVDrop.Instance.GetDropItem(chapterInfoData.DropWatchId);
                int count = temp.Count;
                FrameworkTool.DestroyChildren(view.gameObject);
                for (int i = 0; i < count; i++)
                {
                    PropItem propItem = PropIconLoader.GetAsset(new PropIconLoader.ShowItemData(temp[i].id, temp[i].count,
                                      true, false, false, false, false, _bShowCount: true), view, EUIID.UI_MagicBook_Tips);
                    propItem.gotGo.SetActive(isGet);
                }
            }

        }
    }
}
