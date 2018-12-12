using ZStart.Core;
using UnityEngine;

namespace ZStart.Common.Controller
{
    [System.Serializable]
    public struct SpriteInfo
    {
        public int type;
        public Sprite[] sprites;
    }
    public class SpriteController : ZSingletonBehaviour<SpriteController>
    {
        public SpriteInfo[] spriteInfos;

        public Sprite[] GetSprites(int type)
        {
            for (int i = 0; i < spriteInfos.Length;i++ )
            {
                if (spriteInfos[i].type == type)
                    return spriteInfos[i].sprites;
            }
            return null;
        }

        public Sprite GetSprite(int type,int index)
        {
            for (int i = 0; i < spriteInfos.Length; i++)
            {
                if (spriteInfos[i].type == type)
                {
                    Sprite[] sprites = spriteInfos[i].sprites;
                    if (sprites != null && sprites.Length > index)
                        return sprites[index];
                }
            }
            return null;
        }
    }
}
