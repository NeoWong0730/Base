using UnityEngine;
using UnityEngine.UI;
using System;
using NaughtyAttributes;
using UnityEngine.EventSystems;

namespace Framework
{
    public class ContentSizeFitterHelper : UIBehaviour
    {
        RectTransform rect;

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            rect = gameObject.GetComponent<RectTransform>();
        }
    }

    [ExecuteInEditMode]
    [Serializable]
    public class ImageFitterText : MonoBehaviour
    {
        public Image image;
        RectTransform imageRectTrans;

        public EmojiText targetText;
        Vector2 textScale;

        protected Vector2 lastTextSize;

        public Vector2 sizeOffset = new Vector2(10, 10);

        public bool useSuggestTextSetting = true;
        public bool useTextScale = true;

        public float width = 400;
        public bool isSingleLine = false;
        public float singleLineOffset = 0f;

        private void Awake()
        {
            if (image == null)
            {
                image = transform.GetComponent<Image>();
            }

            if (image != null)
            {
                imageRectTrans = image.GetComponent<RectTransform>();
            }

            if (targetText == null)
            {
                targetText = transform.GetComponentInChildren<EmojiText>();
                targetText.gameObject.AddComponent<ContentSizeFitterHelper>();
            }

            if (targetText != null)
            {
                textScale = targetText.GetComponent<RectTransform>().localScale.ToVector2XY();
                lastTextSize = new Vector2(targetText.preferredWidth, targetText.preferredHeight);

                if (useSuggestTextSetting)
                {
                    targetText.alignment = TextAnchor.UpperCenter;
                    targetText.horizontalOverflow = HorizontalWrapMode.Wrap;
                    targetText.verticalOverflow = VerticalWrapMode.Overflow;
                }
            }

            if (image == null || targetText == null)
            {
                Debug.Log("Text or Image is null");
            }
        }

        public void SetIsSingleLine()
        {
            if (targetText.preferredWidth < width)
            {
                isSingleLine = true;
            }
        }

        public void Refresh()
        {         
            UpdateImageSize(GetTextPreferredSize(), sizeOffset);
            lastTextSize = GetTextPreferredSize();
        }

        void UpdateImageSize(Vector2 size, Vector2 offset)
        {
            if (imageRectTrans != null)
            {
                imageRectTrans.sizeDelta = size + offset;
            }
        }

        Vector2 GetTextPreferredSize()
        {
            if (targetText == null)
                return Vector2.zero;

            Vector2 size;
            if (isSingleLine)
            {
                size = new Vector2(targetText.preferredWidth + singleLineOffset, targetText.preferredHeight);
            }
            else
            {
                size = new Vector2(width, targetText.preferredHeight);
            }
            if (useTextScale)
            {
                size = new Vector2(size.x * textScale.x, size.y * textScale.y);
            }
            return size;
        }

        [Button]

        public void Reset()
        {
            Refresh();
        }
    }
}
