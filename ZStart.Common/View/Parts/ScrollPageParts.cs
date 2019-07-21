using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZStart.Common.View.Parts
{
    public class ScrollPageParts:AppParts
    {
        public Transform contentBox;
        public Image prefab;
        public Color overColor = Color.white;
        public Color outColor = Color.gray;
        public Sprite overSp;
        public Sprite outSp;
        public List<Image> allImages = new List<Image>();
        private List<Image> showImages = new List<Image>();
        public void InitImages(int max)
        {
            for (int i = 0; i < showImages.Count; i++)
            {
                showImages[i].gameObject.SetActive(false);
            }
            showImages.Clear();
            if (max < 2)
                return;
            for (int i = 0; i < max; i += 1)
            {
                Image tmp = ActiveNumImage();
                showImages.Add(tmp);
            }
        }

        /// <summary>
        /// 高亮页面
        /// </summary>
        /// <param name="page">大于0，从1开始算</param>
        public void HighlightPage(int page, bool color)
        {
            if (color)
            {
                for (int i = 0; i < showImages.Count; i += 1)
                {
                    Image tmp = showImages[i];
                    if (i + 1 == page)
                    {
                        tmp.color = overColor;
                    }
                    else
                    {
                        tmp.color = outColor;
                    }
                }
            }
            else
            {
                if (page < 1 || page > showImages.Count)
                    return;
                for (int i = 0; i < showImages.Count; i++)
                {
                    if (i == (page - 1))
                    {
                        showImages[i].sprite = outSp;
                    }
                    else
                    {
                        showImages[i].sprite = overSp;
                    }
                }
            }
        }

        public override void Clear()
        {
            for (int i = 0; i < allImages.Count; i += 1)
            {
                allImages[i].gameObject.SetActive(false);
            }
        }

        private Image ActiveNumImage()
        {
            for (int i = 0; i < allImages.Count; i += 1)
            {
                if (!allImages[i].isActiveAndEnabled)
                {
                    allImages[i].gameObject.SetActive(true);
                    return allImages[i];
                }
            }
            Image tmp = GameObject.Instantiate(prefab);
            allImages.Add(tmp);
            tmp.transform.SetParent(contentBox);
            tmp.transform.localPosition = Vector3.zero;
            tmp.transform.localScale = Vector3.one;
            tmp.gameObject.SetActive(true);
            return tmp;
        }

        public void UpdateSprites(Sprite normal,Sprite selected)
        {
            overSp = normal;
            outSp = selected;
        }
    }
}
