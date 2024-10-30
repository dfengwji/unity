using System.Collections.Generic;
using UnityEngine;
using ZStart.Core.Common;
using ZStart.Core.Controller;
using ZStart.Core.Enum;

namespace ZStart.Core.Util
{
    public class GameObjectUtil
    {
        static public Transform[] FindAllTransformByLayer(LayerMaskType layer)
        {
            List<Transform> list = new List<Transform>();
            Transform[] transList = GameObject.FindObjectsOfType(typeof(Transform)) as Transform[];
            int layerr = LayerMask.NameToLayer(layer.ToString());
            foreach (Transform trans in transList)
            {
                if (trans.gameObject.layer == layerr)
                {
                    list.Add(trans);
                }
            }
            return list.ToArray();
        }

        static public Transform FindChildByLayer(Transform parent, LayerMaskType layer)
        {
            if (parent == null)
                return null;

            int layerr = LayerMask.NameToLayer(layer.ToString());
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform trans = parent.GetChild(i);
                if (trans.gameObject.layer == layerr)
                {
                    return trans;
                }
            }
            return null;
        }

        static public Transform[] FindChildrenByLayer(Transform parent, LayerMaskType layer)
        {
            List<Transform> list = new List<Transform>();
            if (parent != null)
            {
                int layerr = LayerMask.NameToLayer(layer.ToString());

                for (int i = 0; i < parent.childCount; i++)
                {
                    Transform trans = parent.GetChild(i);
                    if (trans.gameObject.layer == layerr)
                    {
                        list.Add(trans);
                    }
                }
            }
            return list.ToArray();
        }

        static public int SortByName(Transform a, Transform b) { return string.Compare(a.name, b.name); }

        static public Transform[] GetChildren(Transform parent)
        {
            List<Transform> list = new List<Transform>();
            if (parent != null)
            {
                for (int i = 0; i < parent.childCount; i++)
                {
                    Transform trans = parent.GetChild(i);
                    list.Add(trans);
                }
            }
            list.Sort(SortByName);
            return list.ToArray();
        }

        static public void ChangeLayer(GameObject go, LayerMaskType layer)
        {
            foreach (Transform trans in go.GetComponentsInChildren<Transform>())
            {
                trans.gameObject.layer = LayerMask.NameToLayer(layer.ToString());
            }
        }

        static public void ChangeLayer(GameObject go, int layer)
        {
            foreach (Transform trans in go.GetComponentsInChildren<Transform>())
            {
                trans.gameObject.layer = layer;
            }
        }

        static public void RenderObject(GameObject go, bool render)
        {
            if (go == null)
                return;

            foreach (Renderer rend in go.GetComponentsInChildren<Renderer>())
            {
                rend.enabled = render;
            }
        }

        static public T InstantiateObject<T>(T prefab, Transform parent, LayerMaskType layer) where T :Transform
        {
            if (prefab == null) return null;

            T clone = InstantiateObject<T>(prefab, parent);
            ChangeLayer(clone.gameObject, layer);
            return clone;
        }

        static public T InstantiateObject<T>(T prefab, Transform parent, int layer)where T:Transform
        {
            if (prefab == null) return null;

            T clone = InstantiateObject<T>(prefab, parent);
            ChangeLayer(clone.gameObject, layer);
            return clone;
        }

        static public T InstantiateObject<T>(T prefab, Transform parent, Vector3 scale) where T:Transform
        {
            if (prefab == null)
                return null;
            T clone = GameObject.Instantiate<T>(prefab);
            if (clone != null && parent != null)
            {
                clone.SetParent(parent);
                clone.localPosition = Vector3.zero;
                clone.localRotation = Quaternion.identity;
                clone.localScale = scale;
            }
            return clone;
        }

        /// <summary>
        /// 实例化一个新的模型
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        static public T InstantiateObject<T>(T prefab, Transform parent)where T:Transform
        {
            if (prefab == null)
                return null;
            Vector3 tmpLocalScale = prefab.localScale;
            return InstantiateObject(prefab, parent, tmpLocalScale);
        }

        static public void DestroyTransform(Transform trans)
        {
            if (trans != null)
            {
                GameObject.Destroy(trans.gameObject);
            }
        }

        static public void DestroyChildren(Transform root)
        {
            if (root == null)
                return;

            if (root.childCount > 0)
            {
                for (int i = 0; i < root.childCount; i++)
                {
                    if (root.GetChild(i).GetComponent<DontDestroy>() == null)
                        GameObject.Destroy(root.GetChild(i).gameObject);
                }
            }
        }

        static public void DestroyAll()
        {
            GameObject[] goArray = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
            for (int i = 0; i < goArray.Length; i++)
            {
                if (goArray[i].GetComponent<DontDestroy>() == null)
                    GameObject.Destroy(goArray[i].gameObject);
            }
        }

        static public void EnableChildrenScripts(Transform root, bool able)
        {
            if (root == null)
                return;

            MonoBehaviour[] monos = root.GetComponentsInChildren<MonoBehaviour>();
            foreach (MonoBehaviour mono in monos)
            {
                mono.enabled = able;
            }
        }

        static public void EnableCollider(Transform trans, bool enable)
        {
            if (trans == null)
                return;
            Collider[] colliders = trans.GetComponentsInChildren<Collider>(true);
            foreach (Collider col in colliders)
            {
                col.enabled = enable;
            }
        }

        static public T FindInParents<T>(Transform target) where T : Component
        {
            if (target == null) return null;
            Transform trans = target;
            object comp = trans.GetComponent<T>();
            if (comp == null)
            {
                while (comp == null)
                {
                    trans = trans.parent;
                    if (trans != null)
                    {
                        comp = trans.GetComponent<T>();
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return (T)comp;
        }

        static public T FindInParents<T>(GameObject go) where T : Component
        {
            if (go == null) return null;

            Transform t = go.transform.parent;
            object comp = t.GetComponent<T>();
            if (comp == null)
            {
                while (t != null && comp == null)
                {
                    comp = t.GetComponent<T>();
                    t = t.parent;
                }
            }
            return (T)comp;
        }

        static public void ActiveChildren(GameObject parent, bool act)
        {
            //Debug.Log(parent.name);
            parent.SetActive(act);

            //Transform trans = parent.transform;
            //for (int i = 0; i < trans.childCount;i++ )
            //{
            //    trans.GetChild(i).gameObject.SetActive(act);
            //}
        }

        /// <summary>
        /// 播放特效
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="life"></param>
        /// <param name="parent"></param>
        /// <param name="locPosition"></param>
        /// <param name="locRotation"></param>
        /// <returns></returns>
        static public ZEffectBase PlayEffect(GameObject prefab, float life, Transform parent, bool worldStay)
        {
            if (prefab == null)
                return null;
            ZEffectBase effect = ZAssetController.Instance.ActivateEffect(prefab, parent, worldStay);
            if (effect != null)
            {
                effect.Play(life);
            }
            return effect;
        }

        /// <summary>
        /// 播放特效
        /// </summary>
        /// <param name="effectID"></param>
        /// <param name="parent"></param>
        /// <param name="locPosition"></param>
        /// <param name="locRotation"></param>
        /// <returns></returns>
        static public ZEffectBase PlayEffect(long effectID, Transform parent, bool worldStay)
        {
            if (effectID < 1)
                return null;
            ZEffectBase effect = ZAssetController.Instance.ActivateEffect(effectID, parent, worldStay);
            if (effect != null)
            {
                effect.Play();
            }
            return effect;
        }
    }
}
