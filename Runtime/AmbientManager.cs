using System;
using UnityEngine;

namespace DreemurrStudio.AudioSystem
{
    /// <summary>
    /// 环境音播放管理器
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AmbientManager : Singleton<AmbientManager>
    {
        [SerializeField][Tooltip("环境音注册信息")]
        private AmbientRegistrySO ambientInfoSO;
        [SerializeField][Tooltip("默认在游戏开始时播放的环境音，空文本时不播放")]
        private string ambientOnAwake;

        /// <summary>
        /// 当前正在播放的环境音信息
        /// </summary>
        private AmbientInfo currentAmbient;
        /// <summary>
        /// 上一个播放的环境音信息
        /// </summary>
        private AmbientInfo prevAmbient;
        
        /// <summary>
        /// 环境音播放音频源组件
        /// </summary>
        private AudioSource audioSource;

        protected override void Awake()
        {
            base.Awake();
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            if(!string.IsNullOrEmpty(ambientOnAwake))
                Play(ambientOnAwake);
        }

        public void Play(string ambientName)
        {
            var info = ambientInfoSO.GetAmbientInfo(ambientName);
            audioSource.clip = info.clip;
            audioSource.volume = info.Volume;
            audioSource.Play();
            currentAmbient = info;
        }

        public void Pause() => audioSource.Pause();
        public void Resume() => audioSource.UnPause();
        public void Stop() => audioSource.Stop();
    }
}
