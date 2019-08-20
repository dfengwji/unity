using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using ZStart.Common.Controller;
using ZStart.Common.Enum;
using ZStart.Common.Util;
using ZStart.Core;
using ZStart.Core.Controller;

namespace ZStart.Common.Manager
{
    public class AudioManager
    {
        public class AudioInfo
        {
            public string uid;
            public AudioClip clip;
        }

        public class AudioBuffer
        {
            public string uid;
            public byte[] data;
            public GameObject target;
            public bool loop;
        }

        public class AudioLoad
        {
            public string path;
            public GameObject target;
        }

        private static bool musicOn = true;
        private static bool soundOn = true;
        private static AudioManager _instance = null;
        private List<AudioSource> soundList;
        private List<AudioSource> musicList;
        private List<AudioInfo> clips;
        private bool isLoading = false;
        private Coroutine loadCoroutine = null;
        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AudioManager();
                return _instance;
            }
        }

        private AudioManager()
        {
            soundList = new List<AudioSource>();
            musicList = new List<AudioSource>();
            clips = new List<AudioInfo>();
        }

        private AudioInfo GetClipInfo(string uid)
        {
            for (int i = 0; i < clips.Count; i++)
            {
                if (clips[i].uid == uid)
                    return clips[i];
            }
            return null;
        }

        private AudioClip GetClip(string uid)
        {
            for (int i = 0; i < clips.Count; i++)
            {
                if (clips[i].uid == uid)
                    return clips[i].clip;
            }
            return null;
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        public void PlaySound(long audio, GameObject target, AudioType format = AudioType.OGGVORBIS)
        {
            if (audio < 1 || target.activeInHierarchy == false)
                return;
            AudioClip clip = GetClip(audio + "");
            if (clip == null)
            {
                clip = ZAssetController.Instance.GetAudioClip(audio);
                if (clip != null)
                {
                    AudioInfo info = new AudioInfo
                    {
                        uid = audio + "",
                        clip = clip
                    };
                    clips.Add(info);
                }
            }
            PlaySound(clip, target);
        }

        public void PlaySound(string path, GameObject target, AudioType format = AudioType.OGGVORBIS)
        {
            if (string.IsNullOrEmpty(path) || target.activeInHierarchy == false)
                return;
            string uid = AlgorithmUtil.ToMD5(path);
            AudioClip clip = GetClip(uid);
            if (clip == null)
            {
                if (File.Exists(path))
                {
                    if (loadCoroutine != null)
                        CallbackController.Instance.StopCoroutine(loadCoroutine);
                    loadCoroutine = CallbackController.Instance.StartCoroutine(PlaySoundDelay(uid, path, target, format));
                 }
            }
            PlaySound(clip, target);
        }
        
        private IEnumerator PlaySoundDelay(string uid, string path, GameObject target, AudioType format)
        {
            isLoading = true;
            ZLog.Log("audio manager try load sound......path = " + path);
            using (var www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, format))
            {
                yield return www.SendWebRequest();
                isLoading = false;
                if (www.isNetworkError)
                {
                    ZLog.Warning("load the aidio error!!!that msg = " + www.error);
                    NotifyManager.SendNotify(NotifyType.OnAudioLoadError, path);
                    yield break;
                }
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                if (clip.length < 1)
                {
                    ZLog.Warning("load the music is empty!!!that path = " + path);
                    NotifyManager.SendNotify(NotifyType.OnAudioLoadError, path);
                    yield break;
                }
                AudioInfo audioInfo = new AudioInfo
                {
                    uid = uid,
                    clip = clip
                };
                clips.Add(audioInfo);
                PlaySound(clip, target);
                NotifyManager.SendNotify(NotifyType.OnAudioPlay, path);
            }

            loadCoroutine = null;
        }

        public void PlaySound(AudioClip clip, GameObject target)
        {
            if (clip == null || target.activeInHierarchy == false)
                return;
            AudioSource source = target.GetComponent<AudioSource>();
            if (source == null) source = target.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.pitch = 1;
            source.clip = clip;
            source.loop = false;
            source.PlayOneShot(clip, 1);
            if (soundList.Contains(source) == false)
                soundList.Add(source);
        }

        public bool PlayMusics(string uid, AudioLoad[] audios, bool loop, AudioType format = AudioType.OGGVORBIS)
        {
            if (isLoading || audios == null || audios.Length < 1)
                return false;
            ZLog.Log("audio manager play many music......uid = " + uid);
            if (loadCoroutine != null)
                CallbackController.Instance.StopCoroutine(loadCoroutine);
            loadCoroutine = CallbackController.Instance.StartCoroutine(LoadMultyInspector(uid, audios, loop, format));
            return true;
        }

        IEnumerator LoadMultyInspector(string identify, AudioLoad[] audios, bool loop, AudioType format)
        {
            int length = audios.Length;
            isLoading = true;
            AudioClip[] array = new AudioClip[length];
            for (int i = 0; i < length; i++)
            {
                string uid = AlgorithmUtil.ToMD5(audios[i].path);
                AudioClip clip = GetClip(uid);
                if (clip == null)
                {
                    using (var www = UnityWebRequestMultimedia.GetAudioClip("file://" + audios[i].path, format))
                    {
                        yield return www.SendWebRequest();
                        if (www.isNetworkError)
                        {
                            ZLog.Warning("load the music is empty!!!that path = " + www.error);
                            NotifyManager.SendNotify(NotifyType.OnAudioLoadError, "");
                            yield break;
                        }
                        clip = DownloadHandlerAudioClip.GetContent(www);
                        if (clip.length < 1)
                        {
                            ZLog.Warning("load the music is empty!!!that path = " + audios[i].path);
                            NotifyManager.SendNotify(NotifyType.OnAudioLoadError, "");
                            yield break;
                        }
                        AudioInfo audioInfo = new AudioInfo
                        {
                            uid = uid,
                            clip = clip
                        };
                        clips.Add(audioInfo);
                        array[i] = clip;
                    }
                }
                else
                {
                    array[i] = clip;
                    yield return null;
                }
            }
            for (int i = 0; i < length; i++)
            {
                ActiveMusic(array[i], audios[i].target, loop);
            }
            yield return null;
            isLoading = false;
            NotifyManager.SendNotify(NotifyType.OnAudioPlay, identify);
        }

        public void PlayMusic(string path, GameObject target, bool loop, AudioType format = AudioType.OGGVORBIS)
        {
            if (string.IsNullOrEmpty(path) || target.activeInHierarchy == false || isLoading)
                return;
            ZLog.Log("audio manager play one......path = " + path);
            string uid = AlgorithmUtil.ToMD5(path);
            AudioClip clip = GetClip(uid);
            if (clip == null)
            {
                if (File.Exists(path))
                {
                    //Debug.LogError("time read file start:" + Time.realtimeSinceStartup);
                    //new Thread(() => ReadFile(uid, path, target, loop)).Start();
                    if (loadCoroutine != null)
                        CallbackController.Instance.StopCoroutine(loadCoroutine);
                    loadCoroutine = CallbackController.Instance.StartCoroutine(PlayMusicDelay(uid, path, target, loop, format));
                }
            }
            else
            {
                ActiveMusic(clip, target, loop);
                NotifyManager.SendNotify(NotifyType.OnAudioPlay, path);
            }
        }

        private IEnumerator PlayMusicDelay(string uid, string path, GameObject target, bool loop, AudioType format)
        {
            isLoading = true;
            ZLog.Log("audio manager try load music......path = " + path);
            using (var www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, format))
            {
                yield return www.SendWebRequest();
                isLoading = false;
                if (www.isNetworkError)
                {
                    ZLog.Warning("load the aidio error!!!that msg = " + www.error);
                    NotifyManager.SendNotify(NotifyType.OnAudioLoadError, path);
                    yield break;
                }
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                if (clip.length < 1)
                {
                    ZLog.Warning("load the music is empty!!!that path = " + path);
                    NotifyManager.SendNotify(NotifyType.OnAudioLoadError, path);
                    yield break;
                }
                AudioInfo audioInfo = new AudioInfo
                {
                    uid = uid,
                    clip = clip
                };
                clips.Add(audioInfo);
                ActiveMusic(clip, target, loop);
                NotifyManager.SendNotify(NotifyType.OnAudioPlay, path);
            }

            loadCoroutine = null;
        }

        private void ReadFile(string uid, string path, GameObject target, bool loop)
        {
            byte[] data = File.ReadAllBytes(path);
            AudioBuffer buffer = new AudioBuffer
            {
                uid = uid,
                data = data,
                target = target,
                loop = loop
            };
            NotifyManager.SendNotify(NotifyType.OnAudioRead, buffer);
        }

        private bool ActiveMusic(AudioClip clip, GameObject target, bool loop)
        {
            if (clip == null || target.activeInHierarchy == false)
                return false;
            AudioSource source = target.GetComponent<AudioSource>();
            if (source == null) source = target.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.pitch = 1;
            source.clip = clip;
            source.loop = loop;
            source.Play();
            if (musicList.Contains(source) == false)
                musicList.Add(source);
            return true;
        }

        public void ChangeMusic(bool open)
        {
            for (int i = 0; i < musicList.Count; i++)
            {
                if (open)
                    musicList[i].Play();
                else
                    musicList[i].Stop();
            }
        }

        public void ClearSound()
        {
            for (int i = 0; i < soundList.Count; i++)
            {
                soundList[i].Stop();
                soundList[i].clip = null;
            }
            soundList.Clear();
        }

        public void Pause(GameObject target)
        {
            AudioSource source = target.GetComponent<AudioSource>();
            if (source == null) return;
            source.Pause();
        }

        public void Stop(GameObject target)
        {
            AudioSource source = target.GetComponent<AudioSource>();
            if (source == null) return;
            source.Stop();
        }

        public void Resume(GameObject target)
        {
            AudioSource source = target.GetComponent<AudioSource>();
            if (source == null) return;
            source.Play();
        }

        public bool IsPlaying(GameObject target)
        {
            AudioSource source = target.GetComponent<AudioSource>();
            if (source == null) return false;
            return source.isPlaying;
        }

        public void TurnMusic(bool ison)
        {
            if (musicOn == ison) return;
            musicOn = ison;
            ChangeMusic(ison);
        }

        public void TurnSound(bool ison)
        {
            if (soundOn == ison)
                return;
            soundOn = ison;
            
        }
    }
}
