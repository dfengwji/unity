using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ZStart.Core.Enum;
using ZStart.Core.Manager;
using ZStart.Core.Model;

namespace ZStart.Core.Controller
{
    [Serializable]
    public struct PoolObjectInfo
    {
        public string name;
        public string path;
        public int bundle;
        public GameObject gameObject;
        public Transform transform;
        public MonoBehaviour component;
        public void Clear()
        {
            name = "";
            path = "";
            bundle = 0;
            gameObject = null;
            component = null;
        }
    }

    public struct PoolImageInfo<T> 
    {
        public int bundle;
        public string path;
        public T image;
    }

    public class ZAssetController : ZSingletonBehaviour<ZAssetController>
    {
        public Transform effectParent;
        public int poolSize = 100;
        private Dictionary<string, AudioClip> soundDic;
        private Dictionary<string, GameObject> prefabDic;
       
        public List<ZEffectBase> effectList;
        public List<PoolObjectInfo> assetList;
        public List<ZEffectBase> activedEffects;
        public List<PoolImageInfo<Sprite>> spriteList;
        public List<PoolImageInfo<Texture2D>> textureList;

        public AssetVarietyInfo[] varityAssets;
        public List<EffectInfo> effectDataList;
        public List<AudioInfo> audioDataList;
        public List<AssetInfo> assetDataList;
        public bool isInstant = false;

        public ZBehaviourBase[] extraPrefabs;

        protected override void Awake()
        {
            base.Awake();
            effectList = new List<ZEffectBase>(poolSize);
            activedEffects = new List<ZEffectBase>(poolSize);
            assetList = new List<PoolObjectInfo>(poolSize);
            assetDataList = new List<AssetInfo>();
            soundDic = new Dictionary<string, AudioClip>();
            prefabDic = new Dictionary<string, GameObject>();
           
            spriteList = new List<PoolImageInfo<Sprite>>();
            textureList = new List<PoolImageInfo<Texture2D>>();
        }

        void Start()
        {
            isStartEnd = true;
        }

        public void InitData(List<EffectInfo> effects,List<AudioInfo> audios,List<AssetInfo> assets, AssetVarietyInfo[] varieties)
        {
            effectDataList = effects;
            audioDataList = audios;
            assetDataList = assets;
            varityAssets = varieties;
        }

        public void PreInstant(Transform parent,UnityAction<float> progress, UnityAction complete)
        {
            effectParent = parent;
            if (isStartEnd) 
            {
                StartCoroutine(InstantInspector(progress, complete));
            }
        }

        IEnumerator InstantInspector(UnityAction<float> progress, UnityAction complete)
        {
            yield return null;
            int index = 0;
            int sum = 1;
            List<AssetInfo> assets = new List<AssetInfo>();
            for (int i = 0; i < assetDataList.Count; i++)
            {
                AssetInfo asset = assetDataList[i];
                if (assetDataList[i].preInstant > 0)
                {
                    for (int j = 0; j < asset.preInstant; j++)
                    {
                        assets.Add(asset);
                    }
                }
            }
           
            float length = effectDataList.Count + assets.Count + audioDataList.Count + 1f;
            progress(sum / length);
            yield return null;
           
            while (index < effectDataList.Count)
            {
                CreateEffect(effectDataList[index].id, effectParent, false);
                index++;
                sum++;
                progress(sum / length);
                yield return null;
            }

            index = 0;
            while (index < audioDataList.Count)
            {
                GetAudioClip(audioDataList[index].bundle,audioDataList[index].assetPath);
                index++;
                sum++;
                progress(sum / length);
                yield return null;
            }

            index = 0;
            while (index < assets.Count)
            {
                AssetInfo info = assets[index];
                CreateAsset(info.name, info.asset, effectParent);
                index++;
                sum++;
                progress(sum / length);
                yield return null;
            }
            ZBundleManager.Instance.RemoveBundles(BundleType.Prefab_Effect);
            ZBundleManager.Instance.RemoveBundles(BundleType.AudioClip);
            yield return null;
            ZUIPanelController.Init();
            yield return null;
            progress(1f);
            yield return null;
            complete?.Invoke();
        }

        public AudioClip GetAudioClip(int bundle, string path)
        {
            try
            {
                soundDic.TryGetValue(path, out AudioClip clip);
                if (clip == null)
                {
                    if(bundle > 0)
                        clip = LoadResource<AudioClip>(bundle, path,"ogg");
                    else
                        clip = LoadResource<AudioClip>(path,"ogg",BundleType.AudioClip);
                    if (clip == null)
                    {
                        ZLog.Warning("clip is null!!! that bundle = "+bundle+" ;name = " + path);
                        return null;
                    }
                    if (soundDic.ContainsKey(path))
                    {
                        soundDic[path] = clip;
                    }
                    else
                    {
                        soundDic.Add(path, clip);
                    }
                }
                return clip;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public AudioClip GetAudioClip(long id)
        {
            AudioInfo model = GetAudioModel(id);
            if (model.id < 1)
                return null;
            return GetAudioClip(model.bundle, model.assetPath);
        }

        public AudioClip[] GetAudioClips(int[] list)
        {
            List<AudioClip> array = new List<AudioClip>();
            try
            {
                foreach (long id in list)
                {
                    AudioInfo model = GetAudioModel(id);
                    if (model.id > 0)
                    {
                        AudioClip clip = GetAudioClip(model.bundle, model.assetPath);
                        if (clip != null)
                            array.Add(clip);
                    }
                }
                return array.ToArray();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public Texture2D GetTexture2D(int assetid)
        {
            AssetVarietyInfo info = GetVarietyInfo(assetid);
            if (info.id < 1)
            {
                ZLog.Warning("Can not find variety asset info that id is " + assetid + " !!!!!");
                return null;
            }
            return GetTexture2D(info.spriteBundle, info.spriteName, info.spriteType.ToString().ToLower());
        }

        public Texture2D GetTexture2D(int bundle,string path,string suffix)
        {
            if (string.IsNullOrEmpty(path)) return null;
            try
            {
                Texture2D texture = null;
                for (int i = 0; i < spriteList.Count; i++)
                {
                    if (spriteList[i].path == path && spriteList[i].bundle == bundle)
                    {
                        texture = textureList[i].image;
                        break;
                    }
                }

                if (texture == null)
                {
                    if (bundle > 0)
                        texture = LoadResource<Texture2D>(bundle, path, suffix);
                    else
                        texture = LoadResource<Texture2D>(path, suffix, BundleType.Texture2D);
                    if (texture == null)
                    {
                        ZLog.Log("Texture is null!!! path = " + path);
                        return null;
                    }
                    PoolImageInfo<Texture2D> info = new PoolImageInfo<Texture2D>();
                    info.path = path;
                    info.image = texture;
                    info.bundle = bundle;
                    textureList.Add(info);
                }
                return texture;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public Sprite GetSprite(int assetid)
        {
            AssetVarietyInfo info = GetVarietyInfo(assetid);
            if(info.id < 1)
            {
                ZLog.Warning("Can not find variety asset info that id is " + assetid + " !!!!!");
                return null;
            }
            return GetSprite(info.spriteBundle, info.spriteName,info.spriteType.ToString().ToLower());
        }

        public Sprite GetSprite(string path,SpriteFormatType type)
        {
            return GetSprite(0,path,type.ToString().ToLower());
        }

        public Sprite GetSprite(int bundle, string path,string suffix)
        {
            if (string.IsNullOrEmpty(path)) return null;
            try
            {
                Sprite sprite = null;
                for (int i = 0;i < spriteList.Count;i++)
                {
                    if (spriteList[i].path == path && spriteList[i].bundle == bundle)
                    {
                        sprite = spriteList[i].image;
                        break;
                    }
                }
               
                if (sprite == null)
                {
                    if(bundle > 0)
                        sprite = LoadResource<Sprite>(bundle,path, suffix);
                    else
                        sprite = LoadResource<Sprite>(path, suffix, BundleType.Sprite);
                    if (sprite == null)
                    {
                        ZLog.Log("sprite is null!!! path = " + path);
                        return null;
                    }
                    PoolImageInfo<Sprite> info = new PoolImageInfo<Sprite>();
                    info.path = path;
                    info.image = sprite;
                    info.bundle = bundle;
                    spriteList.Add(info);
                }
                return sprite;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public GameObject GetEffectPrefab(long id)
        {
            EffectInfo model = GetEffectModel(id);
            if (model.id < 1)
                return null;
            Transform target = GetPrefab<Transform>(model.bundle,model.assetPath,BundleType.Prefab_Effect);
            if (target != null)
            {
                ZEffectBase tool = target.GetComponent<ZEffectBase>();
                if (tool == null)
                    tool = target.gameObject.AddComponent<ZEffectBase>();
                tool.lifeTime = model.lifeTime;
                tool.id = id;
                tool.isLoop = model.isLoop;
                tool.name = model.name;
                tool.type = model.type;
                return target.gameObject;
            }
            return null;
        }

        public GameObject[] GetEffectPrefabs(long[] list)
        {
            List<GameObject> array = new List<GameObject>();
            try
            {
                foreach (long id in list)
                {
                    GameObject prefab = GetEffectPrefab(id);
                    if (prefab != null)
                        array.Add(prefab);
                }
                return array.ToArray();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public T GetExtraPrefab<T>() where T : MonoBehaviour
        {
            for (int i = 0; i < extraPrefabs.Length; i++)
            {
                T tmp = extraPrefabs[i].GetComponent<T>();
                if (tmp != null)
                    return tmp;
            }
            return null;
        }

        private T GetPrefab<T>(int bundle, string path, BundleType type) where T:Component
        {
            if (string.IsNullOrEmpty(path)) return null;
            try
            {
                GameObject obj;
                prefabDic.TryGetValue(path, out obj);
                if (obj == null)
                {
                    if (bundle > 0)
                        obj = LoadResource<GameObject>(bundle,path,"prefab");
                    else
                        obj = LoadResource<GameObject>(path);
                    if (obj == null)
                    {
                        ZLog.Log("Can not find prefab !!!! that name = " + path);
                        return null;
                    }
                    if (prefabDic.ContainsKey(path))
                    {
                        prefabDic[path] = obj;
                    }
                    else
                    {
                        prefabDic.Add(path, obj);
                    }
                }
                if (obj != null)
                    return obj.GetComponent<T>();
                return null;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public T GetPrefab<T>(string path) where T:Component
        {
            if (string.IsNullOrEmpty(path)) return null;
            try
            {
                GameObject obj;
                prefabDic.TryGetValue(path, out obj);
                if (obj == null)
                {
                    obj = LoadResource<GameObject>(path);
                    if (obj == null)
                    {
                        ZLog.Log("Can not find prefab !!!! that name = " + path);
                        return null;
                    }
                    if (prefabDic.ContainsKey(path))
                    {
                        prefabDic[path] = obj;
                    }
                    else
                    {
                        prefabDic.Add(path, obj);
                    }
                }
                if (obj != null)
                    return obj.GetComponent<T>();
                return null;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        private T GetPrefab<T>() where T:MonoBehaviour
        {
            foreach(KeyValuePair<string,GameObject> pair in prefabDic)
            {
                T prefab = pair.Value.GetComponent<T>();
                if (prefab != null)
                    return prefab;
            }
            return null;
        }

        private EffectInfo GetEffectModel(long id)
        {
            if (effectDataList == null || effectDataList.Count < 1)
            {
                Debug.LogError("Can not find all the effect data!!!");
                return new EffectInfo();
            }
            for (int i = 0; i < effectDataList.Count;i++ )
            {
                if(effectDataList[i].id == id)
                    return effectDataList[i];
            }
            ZLog.Warning("Can not find the effect data!!!that id = " + id);
            return new EffectInfo();
        }

        private AudioInfo GetAudioModel(long id)
        {
            if (audioDataList == null || audioDataList.Count < 1)
            {
                Debug.LogError("Can not find all the audio data!!!");
                return new AudioInfo();
            }
            for (int i = 0; i < audioDataList.Count; i++)
            {
                if (audioDataList[i].id == id)
                    return audioDataList[i];
            }
            ZLog.Warning("Can not find the audio data!!!that id = " + id);
            return new AudioInfo();
        }

        public List<AssetInfo> GetAssets(AssetType type)
        {
            if (assetDataList == null || assetDataList.Count < 1)
            {
                Debug.LogError("Can not find all the ui asset data!!!");
                return null;
            }
            List<AssetInfo> infos = new List<AssetInfo>();
            for (int i = 0; i < assetDataList.Count; i++)
            {
                if (assetDataList[i].type == type)
                    infos.Add(assetDataList[i]);
            }
            return infos;
        }

        private ZEffectBase CreateEffect(long effectid, Transform parent, bool worldStay)
        {
            GameObject prefab = GetEffectPrefab(effectid);
            if (prefab == null) return null;
            ZEffectBase effect = CreateEffect(prefab, parent, worldStay);
            effect.id = effectid;
            return effect;
        }

        private ZEffectBase CreateEffect(GameObject prefab, Transform parent, bool worldStay)
        {
            if (prefab == null) return null;
            GameObject temp = Instantiate(prefab);
            temp.transform.SetParent(parent, worldStay);
            ZEffectBase tool = temp.GetComponent<ZEffectBase>();
            if (tool == null)
            {
                tool = temp.AddComponent<ZEffectBase>();
            }
            tool.label = prefab.name;
            temp.SetActive(false);
            effectList.Add(tool);
            return tool;
        }

        public ZEffectBase ActivateEffect(GameObject prefab, Transform parent, bool worldStay)
        {
            if (prefab == null)
                return null;
            ZEffectBase effect = GetSleepEffect(prefab.name);
            if (effect == null)
                effect = CreateEffect(prefab, parent, worldStay);
            else
                effect.mTransform.SetParent(parent, worldStay);
            if (effect == null) return null;
            if (worldStay == false)
            {
                effect.mTransform.localPosition = Vector3.zero;
                effect.mTransform.localRotation = Quaternion.identity;
            }
            activedEffects.Add(effect);
            if (effect.gameObject.activeInHierarchy == false)
                effect.gameObject.SetActive(true);
            return effect;
        }

        public ZEffectBase ActivateEffect(long effectid, Transform parent)
        {
            return ActivateEffect(effectid, parent, true);
        }

        public ZEffectBase ActivateEffect(long effectid, Transform parent, bool worldStay)
        {
            ZEffectBase effect = GetSleepEffect(effectid);
            if (effect == null)
                effect = CreateEffect(effectid, parent, worldStay);
            else
                effect.mTransform.SetParent(parent,worldStay);
            if (effect == null) return null;
            if(worldStay == false)
            {
                effect.mTransform.localPosition = Vector3.zero;
                effect.mTransform.localRotation = Quaternion.identity;
            }
            
            effect.mTransform.localScale = Vector3.one;
            activedEffects.Add(effect);
            if (effect.gameObject.activeInHierarchy == false)
                effect.gameObject.SetActive(true);
            return effect;
        }

        public void DeActivateEffect(Transform target)
        {
            if (target == null)
                return;
            target.SetParent(effectParent);
            ZEffectBase effect = target.GetComponent<ZEffectBase>();
            for (int i = 0; i < activedEffects.Count;i++ )
            {
                if(activedEffects[i] == effect){
                    activedEffects.RemoveAt(i);
                    break;
                }
            }
            if (effect != null)
                effect.DeActive();
            else
                target.gameObject.SetActive(false);
        }

        private T GetSleepAsset<T>(string path) where T : MonoBehaviour
        {
            for (int i = 0; i < assetList.Count; i++)
            {
                PoolObjectInfo info = assetList[i];
                GameObject go = info.gameObject;

                if (info.component != null && !go.activeSelf && path == info.path && info.transform.parent == mTransform)
                {
                    if (typeof(T) == info.component.GetType() || info.component.GetType().IsSubclassOf(typeof(T)))
                    {
                        return info.component as T;
                    }
                }
            }
            return null;
        }

        private T GetSleepAssetByName<T>(string uname) where T : MonoBehaviour
        {
            for (int i = 0; i < assetList.Count; i++)
            {
                PoolObjectInfo info = assetList[i];
                
                if (info.component != null && !info.gameObject.activeSelf && uname == info.name && info.transform.parent == mTransform)
                {
                    if (typeof(T) == info.component.GetType() || info.component.GetType().IsSubclassOf(typeof(T)))
                    {
                        return info.component as T;
                    }
                }
            }
            return null;
        }

        private T GetSleepAsset<T>()where T:MonoBehaviour
        {
             for (int i = 0; i < assetList.Count; i++)
            {
                PoolObjectInfo info = assetList[i];
               
                if (info.component != null && !info.gameObject.activeInHierarchy && info.name == typeof(T).Name && info.transform.parent == mTransform)
                {
                    if (typeof(T) == info.component.GetType() || info.component.GetType().IsSubclassOf(typeof(T)))
                    {
                         return info.component as T;
                    }
                }
            }
            return null;
        }

        public T ActivateAsset<T>(T prefab,Transform parent)where T:MonoBehaviour
        {
            if (prefab == null) return null;
            T prop = GetSleepAssetByName<T>(prefab.name);
            if (prop == null)
            {
                prop = CreateAsset<T>(prefab,"");
                if (prop == null)
                {
                    ZLog.Warning("Can not create asset that name = " + typeof(T) + "!!!");
                    return null;
                }
            }

            ResetAsset(prop.transform, parent);
            return prop;
        }

        private AssetVarietyInfo GetVarietyInfo(int id)
        {
            if (varityAssets == null)
                return new AssetVarietyInfo();
            for (int i = 0;i < varityAssets.Length;i++)
            {
                if (varityAssets[i].id == id)
                    return varityAssets[i];
            }
            return new AssetVarietyInfo();
        }

        public T ActivateAsset<T>(int assetid, Transform parent) where T : MonoBehaviour
        {
            AssetVarietyInfo info = GetVarietyInfo(assetid);
            if(info.id < 1)
            {
                ZLog.Warning("can not find variety asset info that id is " + assetid + " !!!!!");
                return null;
            }
            T prop = GetSleepAsset<T>(info.prefabName);
            if (prop == null)
            {
                T prefab = GetPrefab<T>(info.prefabBundle, info.prefabName,BundleType.Prefab_Mix);
                if (prefab == null)
                    return null;
                prop = CreateAsset<T>(prefab, info.prefabName);
                if (prop == null)
                {
                    ZLog.Warning("can not create asset that name = " + typeof(T) + "!!!");
                    return null;
                }
            }

            ResetAsset(prop.transform, parent);
            return prop;
        }

        private T ActivateAsset<T>(string path, Transform parent,BundleType type)where T:MonoBehaviour
        {
            T prop = GetSleepAsset<T>(path);
            if (prop == null)
            {
                T prefab = GetPrefab<T>(0,path, type);
                if (prefab == null)
                    return null;
                prop = CreateAsset<T>(prefab, path);
                if (prop == null)
                {
                    ZLog.Warning("Can not create asset that name = " + typeof(T) + "!!!");
                    return null;
                }
            }

            ResetAsset(prop.transform, parent);
            return prop;
        }

        private T ActivateAsset<T>(string path,int bundle, Transform parent) where T : MonoBehaviour
        {
            T prop = GetSleepAsset<T>(path);
            if (prop == null)
            {
                T prefab = LoadResource<T>(bundle,path,"prefab");
                if (prefab == null)
                    return null;
                prop = CreateAsset<T>(prefab, path);
                if (prop == null)
                {
                    ZLog.Warning("can not create asset that name = " + typeof(T) + "!!!");
                    return null;
                }
            }
            ResetAsset(prop.transform, parent);
            return prop;
        }

        public T ActivateAsset<T>(Transform parent) where T : MonoBehaviour
        {
            T prop = GetSleepAsset<T>();
            if (prop == null)
            {
                T tmp = GetExtraPrefab<T>();
                if (tmp == null)
                    tmp = GetPrefab<T>();

                prop = CreateAsset<T>(tmp, ""); 
                if (prop == null)
                {
                    ZLog.Warning("Can not create asset that name = " + typeof(T) + "!!!");
                    return null;
                }
            }
            ResetAsset(prop.transform,parent);
            return prop;
        }

        public T ActivateAsset<T>(string path,string assetName, Transform parent) where T : MonoBehaviour
        {
            T prop = GetSleepAsset<T>(path);
            if (prop == null)
            {
                prop = CreateAsset<T>(path, assetName);
                if (prop == null)
                {
                    ZLog.Warning("can not create asset that name = " + typeof(T) + "!!!");
                    return null;
                }
            }
            ResetAsset(prop.transform, parent);
            return prop;
        }

        private void ResetAsset(Transform trans,Transform parent)
        {
            trans.SetParent(parent, false);
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = Vector3.one;
            if (trans.gameObject.activeInHierarchy == false)
                trans.gameObject.SetActive(true);
        }

        public void DeActivateAsset(Transform target)
        {
            if (target == null)
                return;
            target.gameObject.SetActive(false);
            target.SetParent(effectParent,false);
        }

        public void DestroyAsset(Transform target)
        {
            if (target == null)
                return;

            for (int i = 0;i < assetList.Count;i++)
            {
                if(assetList[i].transform == target)
                {
                    Destroy(assetList[i].gameObject);
                    assetList[i].Clear();
                    assetList.RemoveAt(i);
                    break;
                }
            }
        }

        private void CreateAsset(string uname,string path, Transform parent)
        {
            Transform prefab = GetPrefab<Transform>(path);
            if (prefab == null)
                return;
            Transform target = Instantiate(prefab);
            PoolObjectInfo info = new PoolObjectInfo
            {
                path = path,
                name = uname,
                gameObject = target.gameObject,
                transform = target,
                component = target.GetComponent<ZUIBehaviour>()
            };
            if (info.component == null)
                info.component = target.GetComponent<ZBehaviourBase>();

            assetList.Add(info);
            target.SetParent(parent, false);
            target.gameObject.SetActive(false);
        }

        private T CreateAsset<T>(string path, string assetName) where T : MonoBehaviour
        {
            GameObject obj = ZBundleManager.Instance.GetAsset<GameObject>(path, assetName);
            if (obj == null)
            {
                ZLog.Warning("can not get gameobject that path = " + path + "; asset name = "+assetName);
                return null;
            }
            T prefab = obj.GetComponent<T>();
            if (prefab == null)
            {
                ZLog.Warning("miss script that path = " + path + "; asset name = " + assetName);
                return null;
            }
            return CreateAsset(prefab, assetName, path);
        }

        private T CreateAsset<T>(T prefab, string path) where T : MonoBehaviour
        {
            if (prefab == null)
            {
                ZLog.Warning("can not create asset that path = " + path + "!!!");
                return null;
            }
            return CreateAsset(prefab, prefab.name,path);
        }

        private T CreateAsset<T>(T prefab,string uname, string path) where T : MonoBehaviour
        {
            if (prefab == null)
            {
                //ZLog.Warning("can not create asset that path = " + path + "!!!");
                return null;
            }
            T clone = Instantiate<T>(prefab);
            PoolObjectInfo info = new PoolObjectInfo
            {
                path = path,
                name = uname,
                gameObject = clone.gameObject,
                transform = clone.transform,
                component = clone
            };
            assetList.Add(info);
            return clone;
        }

        public void PauseEffects()
        {
            for (int i = 0; i < activedEffects.Count;i++ )
            {
                if(activedEffects[i].type == EffectType.Scene)
                    activedEffects[i].Pause();
            }
        }

        public void PlayEffects()
        {
            for (int i = 0; i < activedEffects.Count; i++)
            {
                activedEffects[i].Play();
            }
        }

        public void RecoverEffect()
        {
            int leng = effectList.Count;
            for (int i = 0; i < leng; i++)
            {
                ZEffectBase tool = effectList[i];
                if (tool != null && tool.gameObject.activeInHierarchy)
                {
                    tool.transform.parent = effectParent;
                    tool.gameObject.SetActive(false);
                }
            }
            activedEffects.Clear();
        }

        public void Recover()
        {
            int leng = assetList.Count;
            for (int i = 0; i < leng; i++)
            {
                PoolObjectInfo info = assetList[i];
                if (info.gameObject != null && info.gameObject.activeInHierarchy)
                {
                    info.transform.parent = effectParent;
                    info.gameObject.SetActive(false);
                }
                else if (info.gameObject == null)
                {
                    
                }
            }
            
            RecoverEffect();
        }

        private ZEffectBase GetSleepEffect(long id)
        {
            for (int i = 0; i < effectList.Count; i++)
            {
                ZEffectBase tool = effectList[i];
                if (tool != null && !tool.gameObject.activeInHierarchy && tool.id == id)
                {
                    return tool;
                }
            }
            return null;
        }

        private ZEffectBase GetSleepEffect(string uname)
        {
            for (int i = 0; i < effectList.Count; i++)
            {
                ZEffectBase tool = effectList[i];
                if (tool != null && !tool.gameObject.activeInHierarchy && tool.label == uname)
                {
                    return tool;
                }
            }
            return null;
        }

        private T LoadResource<T>(string path) where T : UnityEngine.Object
        {
            T prefab = Resources.Load<T>(path);
            if (prefab == null)
            {
                ZLog.Warning("can not find the prefab asset in local that path = " + path);
                return null;
            }
            return prefab;
        }

        private T LoadResource<T>(int bundle,string path,string suffix) where T : UnityEngine.Object
        {
            T prefab = null;
            prefab = GetObjectFromBundle<T>(bundle, path, suffix, false);
            if (prefab == null)
                prefab = Resources.Load<T>(path);
            if (prefab == null)
            {
                ZLog.Warning("can not found asset that bundle id = " + bundle+" and asset name = " + path);
                return null;
            }
            return prefab;
        }

        private T LoadResource<T>(string path,string suffix, BundleType type) where T : UnityEngine.Object
        {
            T prefab = null;
            prefab = GetObjectFromBundle<T>(type, path, suffix, false);
            if (prefab == null)
                prefab = Resources.Load<T>(path);
            if (prefab == null)
            {
                ZLog.Warning("Can not found asset that bundle type = " + type + " and asset name = " + path);
                return null;
            }
            return prefab;
        }

        private T GetObjectFromBundle<T>(int bundleID, string assetName,string suffix, bool isUnload) where T : UnityEngine.Object
        {
            T bundleObj = null;
            AssetBundle bundle = ZBundleManager.Instance.GetBundle(bundleID);
            if (bundle != null)
            {
                string fullPath = "assets/resources/" + assetName.ToLower() + "." + suffix; 
                if (bundle.Contains(fullPath))
                {
                    bundleObj = bundle.LoadAsset<T>(fullPath);
                    if (isUnload)
                        bundle.Unload(false);
                }
            }
            return bundleObj;
        }

        private T GetObjectFromBundle<T>(BundleType type,string assetName,string suffix, bool isUnload)where T:UnityEngine.Object
        {
            AssetBundle[] bundles = ZBundleManager.Instance.GetBundles(type);
            T bundleObj = null;
            if (bundles != null && bundles.Length > 0)
            {
                for (int i = 0; i < bundles.Length; i++)
                {
                    string fullPath = "assets/resources/" + assetName.ToLower() + "." + suffix;
                    AssetBundle bundle = bundles[i];
                    if (bundle.Contains(fullPath))
                    {
                        bundleObj = bundle.LoadAsset<T>(fullPath);
                        if (isUnload)
                            bundle.Unload(false);
                        break;
                    }
                }
            }
            return bundleObj;
        }

        private string GetFullAssetName(string uname,string suffix)
        {
            return "assets/resources/" + uname.ToLower() + "."+ suffix;
        }
    }
}
