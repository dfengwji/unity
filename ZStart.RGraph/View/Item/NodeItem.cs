using DG.Tweening;
using TMPro;
using UnityEngine;
using ZStart.Core.Controller;
using ZStart.RGraph.Enum;
using ZStart.RGraph.Model;

namespace ZStart.RGraph.View.Item
{
    public class NodeItem:Core.ZBehaviourBase
    {
        public Collider colliderBox;
        public Renderer maskRender;
        //public Material maskPrefab;
        public Renderer normalRender;
        //public Material ringPrefab;
        public GameObject pinRender;
        public Color color;
        public TextMeshPro label;
        public Transform highlight;

        public MenuItem menuItem;
        public NodeInfo Data {
            private set;
            get;
        }

        public bool Pinned
        {
            set
            {
                Data.Pinned = value;
                if(pinRender)
                    pinRender.SetActive(value);
                if (highlight)
                    highlight.GetComponent<MeshRenderer>().enabled = !value;
                if (normalRender)
                    normalRender.enabled = !value;
                if (menuItem)
                {
                    menuItem.UpdateState(MenuType.Pin, value ? ExpendStatus.Closed : ExpendStatus.Opened);
                }
            }
            get
            {
                return Data.Pinned;
            }
        }

        public ExpendStatus Expend
        {
            set
            {
                Data.Expended = value;
                if (menuItem)
                    menuItem.UpdateState(MenuType.Expend, value);
            }
            get
            {
                return Data.Expended;
            }
        }

        private bool isSelected = false;
        public bool Selected
        {
            set
            {
                isSelected = value;
                if (value)
                {
                    Data.ClearForce();
                }
                highlight.gameObject.SetActive(value);
                normalRender.gameObject.SetActive(!value);
            }
            get
            {
                return isSelected;
            }
        }

        public bool ShowMenu
        {
            set
            {
                if (value)
                {
                    menuItem = ZAssetController.Instance.ActivateAsset<MenuItem>(mTransform);
                    menuItem.Show();
                    menuItem.UpdateState(MenuType.Pin, Data.Pinned ? ExpendStatus.Closed : ExpendStatus.Opened);
                    menuItem.UpdateState(MenuType.Expend, Data.Expended);
                }
                else
                {
                    if (menuItem)
                        menuItem.Hide();
                    menuItem = null;
                }
            }
        }

        public bool IsHighlight
        {
            set
            {
                if (maskRender.material.HasProperty("_Color"))
                {
                    if (value)
                    {
                        maskRender.material.SetColor("_Color", new Color(color.r, color.g, color.b, 0f));
                    }
                    else
                    {
                        maskRender.material.SetColor("_Color", new Color(color.r, color.g, color.b, 0.5f));
                    }
                }

                if (normalRender.material.HasProperty("_TintColor"))
                {
                    if (value)
                    {
                        normalRender.material.SetColor("_TintColor", new Color(color.r, color.g, color.b, 1.0f));
                    }
                    else
                    {
                        normalRender.material.SetColor("_TintColor", new Color(color.r, color.g, color.b, 0.2f));
                    }
                }
            }
        }

        private void Start()
        {
            //color = prefab.color;
            //CreateMats();
            Pinned = false;
        }

        public virtual void UpdateInfo(NodeInfo data)
        {
            Data = data;
            Data.Acceleration = Vector3.zero;
            Data.Velocity = Vector3.zero;
            label.text = data.name;
            Selected = false;
            Pinned = false;
            ShowMenu = false;
            gameObject.SetActive(true);
            mTransform.localScale = Vector3.one * 0.01f;
        }

        public virtual void UpdateTexture(Texture2D texture)
        {

        }

        public void Show()
        {
            gameObject.SetActive(true);
            mTransform.DOScale(Vector3.one, 0.2f);
        }

        public void Hide()
        {
            //Data = null;
            if (menuItem)
                menuItem.Hide();
            mTransform.DOScale(Vector3.one * 0.01f, 0.2f).OnComplete(()=> {
                gameObject.SetActive(false);
                ZAssetController.Instance.DeActivateAsset(mTransform);
            });
            menuItem = null;
        }

        protected virtual void CreateMats()
        {
            //maskRender.material = new Material(maskPrefab)
            //{
            //    name = "NodeMaskMat" + Time.realtimeSinceStartup
            //};
            //normalRender.material = new Material(ringPrefab)
            //{
            //    name = "NodeRingMat" + Time.realtimeSinceStartup
            //};
            IsHighlight = false;
        }

        public void ApplyForce(Vector3 force)
        {
            Data.Acceleration += force / Data.mass;
        }

        public void Move()
        {
            var pos = Data.Position;
            //Debug.LogWarning(Data.name + "---" + pos);
            if(!Data.Pinned)
                mTransform.localPosition = pos;
        }

        public void Clear()
        {
            Data.Dispose();
            Data = null;
        }
    }
}
