using UnityEngine;
using ZStart.RGraph.Enum;

namespace ZStart.RGraph.Model
{
    public struct ThemeInfo
    {
        public string uid;
        public string name;
        public string cover;
        public string remark;
        public string node;
    }

    [System.Serializable]
    public struct MenuSprite
    {
        public MenuType menu;
        public Sprite normal;
        public Sprite highlight;
        public Sprite disable;
    }
}
