using System.Collections;
using System.IO;
using UnityEngine;
using ZStart.RGraph.Model;

namespace ZStart.RGraph.View.Item
{
    public class NodePersonItem:NodeItem
    {
        public Renderer avatarRender;
        public Shader avatarShader;

        public override void UpdateInfo(NodeInfo data)
        {
            base.UpdateInfo(data);
            //if (avatarRender.material == null)
            //{
            //    CreateMats();
            //}
            //StartCoroutine(LoadImageDelay(data.avatar));
        }

        public override void UpdateTexture(Texture2D texture)
        {
            avatarRender.material.mainTexture = texture;
        }

        IEnumerator LoadImageDelay(string avatar)
        {
            yield return null;
            byte[] bytes = File.ReadAllBytes(avatar);
            Texture2D texture = new Texture2D(256, 256);
            texture.LoadImage(bytes);
            //Debug.LogWarning(avatarRender.material + "---" + avatarRender.material.mainTexture);

            yield return null;
            //render.material.SetTexture("_MainTex", texture);
            avatarRender.material.mainTexture = texture;
        }

        protected override void CreateMats()
        {
            base.CreateMats();
            //avatarRender.material = new Material(avatarShader)
            //{
            //    name = "NodeAvataMat" + Time.realtimeSinceStartup
            //};
        }
    }
}
