using System.Collections;
using System.IO;
using UnityEngine;

namespace ZStart.RGraph.View.Parts
{
    public class GNodePersonParts: GNodeParts
    {
        public override void UpdateLabel(string tip)
        {
            base.UpdateLabel(tip);
        }

        IEnumerator LoadImageDelay(string avatar)
        {
            yield return null;
            byte[] bytes = File.ReadAllBytes(avatar);
            Texture2D texture = new Texture2D(256, 256);
            texture.LoadImage(bytes);

            yield return null;
            header.texture = texture;
        }
    }
}
