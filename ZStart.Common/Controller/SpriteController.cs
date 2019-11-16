using ZStart.Core;
using UnityEngine;
using System.Collections.Generic;

namespace ZStart.Common.Controller
{
    [System.Serializable]
    public struct SpriteInfo
    {
        public int type;
        public SpriteOneInfo[] sprites;
    }

    [System.Serializable]
    public struct SpriteOneInfo
    {
        public int id;
        public Sprite sprite;
    }

    public class SpriteController : ZSingletonBehaviour<SpriteController>
    {
        public SpriteInfo[] infos;

        public List<Sprite> GetSprites(int type)
        {
            List<Sprite> list = new List<Sprite>(10);
            for (int i = 0; i < infos.Length;i++ )
            {
                if (infos[i].type == type)
                {
                    for (int j = 0;j < infos[i].sprites.Length;j++) {
                        list.Add(infos[i].sprites[j].sprite);
                    }
                }
            }
            return list;
        }

        public Sprite GetSprite(int type,int id)
        {
            for (int i = 0; i < infos.Length; i++)
            {
                if (infos[i].type == type)
                {
                    for (int j = 0; j < infos[i].sprites.Length; j++)
                    {
                        if (infos[i].sprites[j].id == id)
                            return infos[i].sprites[j].sprite;
                    }
                }
            }
            return null;
        }
    }
}
