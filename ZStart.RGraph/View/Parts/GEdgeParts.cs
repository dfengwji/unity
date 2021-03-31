using UnityEngine;
using UnityEngine.UI;
using ZStart.Core.Controller;
using ZStart.RGraph.Enum;
using ZStart.RGraph.Model;

namespace ZStart.RGraph.View.Parts
{
    public class GEdgeParts: AppUIParts
    {
        private Image lineImage;
        public RectTransform line;
        public Image background;
        public Image arrowLeft;
        public Image arrowRight;

        public Vector2 textSize = new Vector2(70f, 50f);
        public float lettorWidth = 20f;
        public RectTransform labelBox;
        public Text label;
        public bool IsHighlight
        {
            set
            {
                if (background != null)
                {
                    background.color = new Color(1f, 1f, 1f, value ? 1f : 0.8f);
                }
                if (lineImage != null)
                {
                    var color = lineImage.color;
                    if (value)
                    {
                        lineImage.color = new Color(color.r, color.g, color.b, 1f);
                    }
                    else
                    {
                        lineImage.color = new Color(color.r, color.g, color.b, 0.5f);
                    }
                }
            }
        }

        public DirectionType Direction
        {
            set
            {
                if (value == DirectionType.ToFrom)
                {
                    arrowLeft.enabled = false;
                    arrowRight.enabled = true;
                }
                else if (value == DirectionType.FromTo)
                {
                    arrowLeft.enabled = true;
                    arrowRight.enabled = false;
                }
                else
                {
                    arrowLeft.enabled = true;
                    arrowRight.enabled = true;
                }
            }
        }

        protected override void Start()
        {
            if (line == null)
            {
                line = mTransform.Find("line").gameObject.GetComponent<RectTransform>();
                lineImage = line.GetComponent<Image>();
                background = mTransform.Find("labelBox/bgBox/bg").GetComponent<Image>();
                arrowLeft = mTransform.Find("labelBox/bgBox/left").GetComponent<Image>();
                arrowRight = mTransform.Find("labelBox/bgBox/right").GetComponent<Image>();
                label = mTransform.Find("labelBox/text").GetComponent<Text>();
                labelBox = mTransform.Find("labelBox").GetComponent<RectTransform>();
                gameObject.SetActive(false);
            }
            IsHighlight = false;
        }

        public bool UpdateInfo(LinkInfo data)
        {
            if (string.IsNullOrEmpty(data.name))
            {
                labelBox.gameObject.SetActive(false);
            }
            else
            {
                label.text = data.name;
                var num = data.name.Length - 2;
                if (num < 0)
                    num = 0;
                labelBox.sizeDelta = new Vector2(textSize.x + num * lettorWidth, textSize.y);
                //arrowLeft.transform.localPosition = new Vector3(-lettorWidth - num * 0.1f, 0f, 0f);
                //arrowRight.transform.localPosition = new Vector3(lettorWidth + num * 0.1f, 0f, 0f);
                labelBox.gameObject.SetActive(true);
            }
            return true;
        }

        public override void Show()
        {
            gameObject.SetActive(true);
        }

        public override void UnShow()
        {
            ZAssetController.Instance.DeActivateAsset(mTransform);
        }
    }
}
