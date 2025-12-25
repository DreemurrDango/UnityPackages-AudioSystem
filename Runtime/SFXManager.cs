using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace DreemurrStudio.AudioSystem
{
    /// <summary>
    /// 音效播放控制器，负责管理和播放全局音效和世界音效
    /// </summary>
    public class SFXManager : Singleton<SFXManager>
    {
        [SerializeField][Tooltip("音效注册表")]
        private SoundEffectRegistrySO seInfoSO;

        [SerializeField] [Tooltip("全局音效实例预制体")] 
        private AudioSource seOverlayPrefab;
        [SerializeField][Tooltip("最大同时播放的全局音源数量")]
        private int maxOverlaySFXNum = 10;
        [SerializeField] [Tooltip("世界音效实例预制体")] 
        private AudioSource seWorldPrefab;
        [SerializeField][Tooltip("最大同时播放的世界音源数量")]
        private int maxWorldSFXNum = 30;
        
    
        /// <summary>
        /// 用于播放全局音效的音源组件对象池
        /// </summary>
        private ObjectPool<AudioSource> overLayAudioSourcePool;
        /// <summary>
        /// 用于播放世界音效的音源组件对象池
        /// </summary>
        private ObjectPool<AudioSource> worldAudioSourcePool;

        //TODO:待优化的唯一播放音效的处理方式，尝试使用唯一集来管理
        /// <summary>
        /// 储存全局音效中需要确保唯一播放的 音效名-音源组件 字典
        /// </summary>
        private Dictionary<string, AudioSource> overlayUniqueSFXDictionary;
        /// <summary>
        /// 储存世界音效中需要确保唯一播放的 游戏对象名+音效名-音源组件 字典
        /// </summary>
        private Dictionary<string, AudioSource> worldUniqueSFXDictionary;
    
        protected override void Awake()
        {
            base.Awake();
            InitPool();
            overlayUniqueSFXDictionary = new Dictionary<string, AudioSource>();
            worldUniqueSFXDictionary = new Dictionary<string, AudioSource>();
        }

        /// <summary>
        /// 初始化音源对象池
        /// </summary>
        private void InitPool()
        {
            //全局音效对象池
            overLayAudioSourcePool = new ObjectPool<AudioSource>(
                createFunc:CreateOverlaySEAudioSouce,
                actionOnGet:OnGetAudioSource,
                actionOnRelease:OnReleaseAudioSource,
                actionOnDestroy:(source) => Destroy(source.gameObject),
                defaultCapacity:10,
                maxSize: maxOverlaySFXNum,
                collectionCheck: false
            );
            //世界音效对象池
            worldAudioSourcePool = new ObjectPool<AudioSource>(
                createFunc:CreateWorldSEAudioSouce,
                actionOnGet:OnGetAudioSource,
                actionOnRelease:OnReleaseAudioSource,
                actionOnDestroy:(source) => Destroy(source.gameObject),
                defaultCapacity:10,
                maxSize: maxWorldSFXNum,
                collectionCheck: false
            );

            AudioSource CreateOverlaySEAudioSouce()
                => Instantiate(seOverlayPrefab.gameObject, transform).GetComponent<AudioSource>();
            AudioSource CreateWorldSEAudioSouce()
                => Instantiate(seWorldPrefab.gameObject, transform).GetComponent<AudioSource>();
            void OnGetAudioSource(AudioSource source) => source.gameObject.SetActive(true);
            void OnReleaseAudioSource(AudioSource source) => source.gameObject.SetActive(false);
        }

        /// <summary>
        /// 执行全局音效的播放
        /// </summary>
        /// <param name="info">要播放的方案信息</param>
        /// <param name="source">进行播放的音频源组件</param>
        private void DoPlayOverlaySE(SESingleInfo info,AudioSource source)
        {
            source.clip = info.clip;
            source.volume = 1f + info.volumeAdjust;
            source.pitch = 1f + Random.Range(info.pitchRandomAdjust.x, info.pitchRandomAdjust.y);
            source.Play();
        }
    
        /// <summary>
        /// 执行世界音效的播放
        /// </summary>
        /// <param name="info">要播放的音效方案</param>
        /// <param name="source">用于播放的音频源组件</param>
        /// <param name="pos"></param>
        private void DoPlayWorldSE(SESingleInfo info, AudioSource source, Vector3 pos)
        {
            source.transform.position = pos;
            source.clip = info.clip;
            source.volume = 1f + info.volumeAdjust;
            source.pitch = 1f + Random.Range(info.pitchRandomAdjust.x, info.pitchRandomAdjust.y);
            source.Play();
        }

        /// <summary>
        /// 全局音效播放完成后回调
        /// </summary>
        /// <param name="seName">播放的音效名</param>
        /// <param name="audioSource">进行播放的音源实例</param>
        /// <returns></returns>
        private IEnumerator AfterOverlaySFXPlayCompleted(string seName, AudioSource audioSource)
        {
            float length = audioSource.clip.length;
            while (audioSource.isPlaying)
                yield return new WaitForSecondsRealtime(length + 0.1f);
            overlayUniqueSFXDictionary.Remove(seName);
            overLayAudioSourcePool.Release(audioSource);
        }
        /// <summary>
        /// 世界音效播放完成后回调
        /// </summary>
        /// <param name="seName">播放的音效名</param>
        /// <param name="audioSource">进行播放的音源实例</param>
        /// <returns></returns>
        private IEnumerator AfterWorldSFXPlayCompleted(string seName, AudioSource audioSource)
        {
            float length = audioSource.clip.length;
            while (audioSource.isPlaying)
                yield return new WaitForSeconds(length + 0.1f);
            worldUniqueSFXDictionary.Remove(seName);
            worldAudioSourcePool.Release(audioSource);
        }
    
        /// <summary>
        /// 播放全局音效
        /// </summary>
        /// <param name="seName">音效在注册表中的方案名</param>
        public void PlayOverlaySFX(string seName)
        {
            //从对象池中获取可用的音源组件，若无说明已达到上限，拒绝播放
            var sfxSource = overLayAudioSourcePool.Get();
            if (sfxSource == null) return;
            //获取音效播放信息，检查是否存在重复播放
            var info = seInfoSO.GetSEInfo(seName);
            if (info.repeatScheme != SERepeatScheme.playAll)
            {
                //有重复播放解决方案的音效，已存在实例时
                if (overlayUniqueSFXDictionary.ContainsKey(seName))
                {
                    switch (info.repeatScheme)
                    {
                        case SERepeatScheme.playOld:
                            return;
                        case SERepeatScheme.playNew:
                            overlayUniqueSFXDictionary[seName].Play();
                            return;
                    }
                }
                overlayUniqueSFXDictionary.Add(seName, sfxSource);
            }
            DoPlayOverlaySE(info.GetSingleInfo(), sfxSource);
            StartCoroutine(AfterOverlaySFXPlayCompleted(seName, sfxSource));
        }
    
        /// <summary>
        /// 播放世界音效
        /// </summary>
        /// <param name="seName">要播放的注册音效名</param>
        /// <param name="go">挂载的游戏对象</param>
        public void PlayWorldSFX(string seName,GameObject go)
        {
            //从对象池中获取可用的音源组件，若无说明已达到上限，拒绝播放
            var sfxSource = worldAudioSourcePool.Get();
            if (sfxSource == null) return;
            //获取音效播放信息，检查是否存在重复播放
            var info = seInfoSO.GetSEInfo(seName);
            if (info.repeatScheme != SERepeatScheme.playAll)
            {
                //有重复播放解决方案的音效，已存在实例时
                if (worldUniqueSFXDictionary.ContainsKey(go.name + seName))
                {
                    switch (info.repeatScheme)
                    {
                        case SERepeatScheme.playOld:
                            return;
                        case SERepeatScheme.playNew:
                            worldUniqueSFXDictionary[go.name + seName].Play();
                            return;
                    }
                }
                worldUniqueSFXDictionary.Add(go.name + seName, sfxSource);
            }
            DoPlayWorldSE(info.GetSingleInfo(), sfxSource,go.transform.position);
            StartCoroutine(AfterWorldSFXPlayCompleted(seName, sfxSource));
        }
    }
}
