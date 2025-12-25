using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DreemurrStudio.AudioSystem
{
    /// <summary>
    /// 音效播放信息
    /// </summary>
    [System.Serializable]
    public struct SESingleInfo
    {
        [Tooltip("音频片段")]
        public AudioClip clip;
        [Tooltip("音量调整")]
        public float volumeAdjust;
        [Tooltip("音调随机浮动范围")]
        public Vector2 pitchRandomAdjust;
        /// <summary>
        /// 获取音源片段长度
        /// </summary>
        public float Length => clip.length;
    }

    /// <summary>
    /// 播放的音效重复时动作
    /// </summary>
    public enum SERepeatScheme
    {
        /// <summary>
        /// 全部播放，不作处理
        /// </summary>
        playAll,
        /// <summary>
        /// 不再播放新的音效命令
        /// </summary>
        playOld,
        /// <summary>
        /// 停止播放旧音效，立即播放新的
        /// </summary>
        playNew
    }
    
    /// <summary>
    /// 音效调用播放信息
    /// </summary>
    [Serializable]
    public struct SEInfo
    {
        [Tooltip("音效项目名")]
        public string name;
        [Tooltip("主要的音效播放信息")]
        public SESingleInfo mainSEInfo;
        [Tooltip("用于随机的其他音效组")]
        public List<SESingleInfo> randomGroup;
        [Tooltip("音效播放重复时处理办法")]
        public SERepeatScheme repeatScheme;

        /// <summary>
        /// 获取这次要播放的音效信息
        /// </summary>
        /// <returns>获取到的随机音效播放信息</returns>
        public SESingleInfo GetSingleInfo()
        {
            if (randomGroup is not null && randomGroup.Count > 0)
            {
                int index = Random.Range(0, randomGroup.Count + 1);
                return index < randomGroup.Count ? randomGroup[index] : mainSEInfo;
            }
            return mainSEInfo;
        }
    }
    
    /// <summary>
    /// 音效注册表
    /// </summary>
    [CreateAssetMenu(fileName = "SoundEffectRegistry", menuName = "数据/音频/音效注册表", order = 2)]
    public class SoundEffectRegistrySO : ScriptableObject
    {
        [SerializeField] [Tooltip("音效信息列表")] 
        private List<SEInfo> seInfos = new List<SEInfo>();

        /// <summary>
        /// 根据音效注册名获取对应的音效播放信息
        /// </summary>
        /// <param name="seName">音效注册名</param>
        /// <returns></returns>
        /// <exception cref="Exception">未找到对应音效名的音效</exception>
        public SEInfo GetSEInfo(string seName)
        {
            var info = seInfos.Find(i => i.name == seName);
            if (info.name == "") throw new Exception("未注册的音效！");
            return info;
        }
    }
}
