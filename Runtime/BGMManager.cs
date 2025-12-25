using System;
using System.Collections;
using UnityEngine;

namespace DreemurrStudio.AudioSystem
{
    /// <summary>
    /// 背景音乐播放管理器:需要安置在音频源上
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class BGMManager : Singleton<BGMManager>
    {
        /// <summary>
        /// 背景音乐播放模式
        /// </summary>
        public enum BGMPlayMode
        {
            /// <summary>
            /// 事件控制：通过脚本控制播放
            /// </summary>
            Event,
            /// <summary>
            /// 音乐台模式：根据清单播放
            /// </summary>
            BandStand
        }

        [SerializeField]
        [Tooltip("背景音乐播放模式，一般一旦设置全局固定（目前仅事件控制状态有效）")]
        private BGMPlayMode bgmPlayMode;
        [SerializeField]
        [Tooltip("背景音乐播放信息")]
        private BGMInfoRegistrySO bgmInfoSO;
        [SerializeField]
        [Min(0f)]
        [Tooltip("切换背景音乐后再切回时，间隔时间小于该值时继续原进度\n设为0时总是不续播，设为较大值以总是续播")]
        private float continuePlayMaxTime = 0f;
        [SerializeField]
        [Tooltip("默认在游戏开始时播放的背景音乐，空文本时不播放")]
        private string playBGMOnAwake;
        [SerializeField]
        [Tooltip("是否循环播放背景音乐")]
        private bool doLoop = true;
        [SerializeField, Min(0f)]
        [Tooltip("背景音乐播放结束后的播放下一曲的间隔时间")]
        private float interval = 0f;

        /// <summary>
        /// 背景音乐播放音频源组件
        /// </summary>
        private AudioSource bgmAudioSource;
        /// <summary>
        /// 当前正在播放的音乐信息
        /// </summary>
        private BGMInfo? currentBGM;
        /// <summary>
        /// 上一次播放的音乐信息
        /// </summary>
        private BGMInfo? prevBGM;
        /// <summary>
        /// 背景音乐上次播放截止位置，用于续播背景音乐
        /// </summary>
        private float bgmPrevMemoryTime = 0f;
        /// <summary>
        /// 上首背景音乐停止播放的时间戳
        /// </summary>
        private float t_bgmPrevStop;

        protected override void Awake()
        {
            base.Awake();
            bgmAudioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            if (playBGMOnAwake != "") Play(playBGMOnAwake);
        }


        /// <summary>
        /// 立即播放项目名对应名的背景音乐
        /// </summary>
        /// <param name="bgmName">音乐项目名</param>
        public void Play(string bgmName)
        {
            StartCoroutine(PlayBGMCoroutine(bgmInfoSO.GetBGMInfo(bgmName)));

            IEnumerator PlayBGMCoroutine(BGMInfo info)
            {
                if (bgmAudioSource.isPlaying) Stop();
                //续播功能判断
                float continueTime = 0f;
                if (continuePlayMaxTime > 0 && prevBGM.HasValue && prevBGM.Value.name == info.name &&
                    Time.unscaledTime < continuePlayMaxTime + t_bgmPrevStop)
                    continueTime = bgmPrevMemoryTime;
                //进行播放
                bgmAudioSource.clip = info.clip;
                bgmAudioSource.volume = info.Volume;
                if (continueTime > 0f) bgmAudioSource.time = bgmPrevMemoryTime;
                bgmAudioSource.Play();
                currentBGM = info;
                var length = info.Length;
                yield return new WaitUntil(() => bgmAudioSource.time > length - 0.2f);
                if (doLoop)
                {
                    yield return new WaitForSeconds(0.2f);
                    yield return new WaitForSeconds(interval);
                    Play(info.name);
                }
                else currentBGM = null;
            }

        }

        // TEMP:未测试的功能，淡入播放背景音乐
        public void Play(string bgmName, float fadeTime)
        {
            StartCoroutine(FadeInBGMCoroutine(bgmName, fadeTime));
            IEnumerator FadeInBGMCoroutine(string bgmName, float fadeTime)
            {
                var info = bgmInfoSO.GetBGMInfo(bgmName);
                //续播功能判断
                float continueTime = 0f;
                if (continuePlayMaxTime > 0 && prevBGM.HasValue && prevBGM.HasValue && prevBGM.Value.name == info.name &&
                    Time.unscaledTime < continuePlayMaxTime + t_bgmPrevStop)
                    continueTime = bgmPrevMemoryTime;
                //记录旧背景音乐播放信息
                prevBGM = currentBGM;
                t_bgmPrevStop = Time.unscaledTime;
                bgmPrevMemoryTime = bgmAudioSource.time;

                //进行播放
                bgmAudioSource.clip = info.clip;
                bgmAudioSource.volume = 0f; // 初始音量为0
                if (continueTime > 0f) bgmAudioSource.time = bgmPrevMemoryTime;
                bgmAudioSource.Play();

                float startVolume = info.Volume;
                float elapsed = 0f;
                while (elapsed < fadeTime)
                {
                    elapsed += Time.unscaledDeltaTime;
                    bgmAudioSource.volume = Mathf.Lerp(0f, startVolume, elapsed / fadeTime);
                    yield return null;
                }
                currentBGM = info; // 更新当前背景音乐信息
            }
        }

        public void Pause() => bgmAudioSource.Pause();
        public void Resume() => bgmAudioSource.UnPause();

        /// <summary>
        /// 经过指定淡出时间后停止背景音乐
        /// </summary>
        /// <param name="fadeTime">停止播放前的淡出时间</param>
        public void Stop(float fadeTime = 0f)
        {
            prevBGM = currentBGM;
            t_bgmPrevStop = Time.unscaledTime;
            bgmPrevMemoryTime = bgmAudioSource.time;
            if (fadeTime > 0f) StartCoroutine(FadeOutBGMCoroutine(fadeTime));
            else bgmAudioSource.Stop();

            IEnumerator FadeOutBGMCoroutine(float fadeTime)
            {
                float startVolume = bgmAudioSource.volume;
                float elapsed = 0f;
                while (elapsed < fadeTime)
                {
                    elapsed += Time.unscaledDeltaTime;
                    bgmAudioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeTime);
                    yield return null;
                }
                bgmAudioSource.Stop();
                bgmAudioSource.volume = startVolume; // 恢复音量
            }
        }
    }
}
