using System.Collections;
using System.IO;
using UnityEngine;
using ZStart.Core;
using ZStart.Core.Controller;
using ZStart.RGraph.Manager;
using ZStart.RGraph.View.Parts;

namespace ZStart.RGraph
{
    [DisallowMultipleComponent]
    public class RGSession: ZBehaviourBase
    {
        private void Awake()
        {
            ZLog.isLog = true;
        }

        IEnumerator Start()
        {
            gameObject.AddComponent<ZAssetController>();
            gameObject.AddComponent<ZImageController>();
            gameObject.AddComponent<ZBundleController>();
            gameObject.AddComponent<ZHttpController>();
            //gameObject.AddComponent<LogoutTool>();
            yield return null;
            ZAssetController.Instance.sleepParent = mTransform;
            CheckPrefabs();
            ZLog.Warning("relation graph session start end... ");
        }

        public void SetRootPath(string path)
        {
            //var path = Path.Combine(Application.persistentDataPath, dir);
            MessageManager.Instance.SetInfo("", true, path);
            ZLog.Warning("relation graph session awake...source path = " + path);
        }

        private void CheckController()
        {
            GameObject obj = GameObject.Find("ZController");
            if (obj != null)
                return;
            obj = new GameObject
            {
                name = "ZController"
            };
            obj.AddComponent<RGSession>();
        }

        private void CheckPrefabs()
        {
            var prefab = ZAssetController.Instance.GetExtraPrefab<GNodeParts>();
            if (prefab != null)
            {
                return;
            }
            var node = mTransform.Find("RGNode").gameObject.AddComponent<GNodeParts>();
            var edge = mTransform.Find("RGEdge").gameObject.AddComponent<GEdgeParts>();
            var person = mTransform.Find("RGNodePerson").gameObject.AddComponent<GNodePersonParts>();
            node.gameObject.SetActive(true);
            edge.gameObject.SetActive(true);
            person.gameObject.SetActive(true);
            ZAssetController.Instance.RegisterPrefab(node);
            ZAssetController.Instance.RegisterPrefab(edge);
            ZAssetController.Instance.RegisterPrefab(person);
        }

        private void Update()
        {
            DFNotifyManager.Instance.Update();
        }
    }
}
