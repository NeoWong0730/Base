//using Lib.Core;
//using UnityEngine;
//using UnityEngine.UI;
//using Table;
//using Framework;
//using System;

//namespace Logic
//{
//    public partial class UI_Society_Layout
//    {
//        /// <summary>
//        /// 系统消息///
//        /// </summary>
//        public class ChatSysytemItem
//        {
//            public GameObject root;

//            RectTransform rootRect;
//            public EmojiText content;
//            public RectTransform contentBg;

//            Sys_Society.ChatData roleChatData;

//            public ChatSysytemItem(GameObject gameObject)
//            {
//                root = gameObject;

//                rootRect = root.GetComponent<RectTransform>();
//                content = root.FindChildByName("Text").GetComponent<EmojiText>();
//                contentBg = root.FindChildByName("Image_Chat").GetComponent<RectTransform>();
//                content.horizontalOverflow = HorizontalWrapMode.Wrap;
//                content.verticalOverflow = VerticalWrapMode.Overflow;

//                content.onHrefClick += OnHrefClick;
//            }

//            public void Update(Sys_Society.ChatData _roleChatData)
//            {
//                roleChatData = _roleChatData;
//                content.text = EmojiTextHelper.ParseChatRichText(FontManager.GetEmoji(GlobalAssets.sEmoji_0), null, roleChatData.content, roleChatData.paramID);

//                ResetRootSize();
//            }

//            void ResetRootSize()
//            {
//                ImageFitterText imageFitterText = contentBg.GetComponent<ImageFitterText>();
//                imageFitterText.SetIsSingleLine();
//                imageFitterText.Refresh();
//                if (contentBg.sizeDelta.y + 30 < 75)
//                {
//                    rootRect.sizeDelta = new Vector2(rootRect.sizeDelta.x, 70);
//                }
//                else
//                {
//                    rootRect.sizeDelta = new Vector2(rootRect.sizeDelta.x, contentBg.sizeDelta.y + 45);
//                }
//            }

//            private void OnHrefClick(string data)
//            {
//                string[] ss = data.Split('_');
//                int type = 0;
//                ulong id = 0;
//                if (ss.Length >= 2)
//                {
//                    int.TryParse(ss[0], out type);
//                    ulong.TryParse(ss[1], out id);
//                }

//                if (type == (int)ERichType.AddFriend)
//                {
//                    Sys_Society.Instance.ReqAddFriend(id);
//                }
//            }
//        }
//    }
//}
