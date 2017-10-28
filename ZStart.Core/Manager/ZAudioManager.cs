using System.Collections.Generic;
using UnityEngine;
using ZStart.Core.Controller;

namespace ZStart.Core.Manager
{
    public class ZAudioManager
    {
        private static bool musicOn = true;
        private static bool soundOn = true;
        private static ZAudioManager _instance = null;
        private List<AudioSource> soundList;
        private List<AudioSource> musicList;

        public static ZAudioManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ZAudioManager();
                return _instance;
            }
        }

        private ZAudioManager()
        {
            soundList = new List<AudioSource>();
            musicList = new List<AudioSource>();
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        public void ActiveSound(long audio, GameObject target)
        {
            if (audio < 1 || target.activeInHierarchy == false)
                return;
            AudioClip clip = ZAssetController.Instance.GetAudioClip(audio);
            AudioSource source = target.GetComponent<AudioSource>();
            if (source == null) source = target.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.pitch = 1;
            source.clip = clip;
            source.PlayOneShot(clip, 1);
            if(soundList.Contains(source) == false)
                soundList.Add(source);
        }

        public void ActiveSound(AudioClip clip, GameObject target)
        {
            if (clip == null || target.activeInHierarchy == false)
                return;
            AudioSource source = target.GetComponent<AudioSource>();
            if (source == null) source = target.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.pitch = 1;
            source.clip = clip;
            source.loop = false;
            source.PlayOneShot(clip,1);
            if (soundList.Contains(source) == false)
                soundList.Add(source);
        }

        public void ActiveMusic(AudioClip clip, GameObject target, bool loop)
        {
            if (clip == null || target.activeInHierarchy == false)
                return;
            AudioSource source = target.GetComponent<AudioSource>();
            if (source == null) source = target.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.pitch = 1;
            source.clip = clip;
            source.loop = loop;
            source.Play();
            if(musicList.Contains(source) == false)
                musicList.Add(source);
        }

        public void ChangeMusic(bool open)
        {
            for (int i = 0;i < musicList.Count;i++)
            {
                if (open)
                    musicList[i].Play();
                else
                    musicList[i].Stop();
            }
        }

        public void ClearSound()
        {
            for (int i = 0;i < soundList.Count;i++)
            {
                soundList[i].Stop();
                soundList[i].clip = null;
            }
        }

        public static void PlayMusic(AudioClip clip, GameObject target, bool loop)
        {
            if (musicOn == false) return;
            Instance.ActiveMusic(clip,target,loop);
        }

        public static void Stop(GameObject target)
        {
            AudioSource source = target.GetComponent<AudioSource>();
            if (source == null) return;
            source.Stop();
        }

        public static void TurnMusic(bool ison)
        {
            if (musicOn == ison) return;
            musicOn = ison;
            Instance.ChangeMusic(ison);
        }

        public static void PlaySound(long audio, GameObject target)
        {
            if (soundOn == false) return;
            Instance.ActiveSound(audio,target);
        }

        public static void PlaySound(AudioClip clip, GameObject target)
        {
            if (soundOn == false) return;
            Instance.ActiveSound(clip, target);
        }

        public static void TurnSound(bool ison)
        {
            if (soundOn == ison) return;
            soundOn = ison;
        }
    }
}
