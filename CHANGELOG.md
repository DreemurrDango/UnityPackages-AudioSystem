# 更新日志

> 此文件记录了该软件包所有重要的变更
> 文件格式基于 [Keep a Changelog](http://keepachangelog.com/en/1.0.0/) 更新日志规范，且此项目版本号遵循 [语义化版本](http://semver.org/spec/v2.0.0.html) 规范

## [2.0.0] - 2025-12-25
### 新增
- **音效系统 (`SFXManager`)**:
    - 添加了 `SERepeatScheme` 枚举，用于处理同一音效重复播放时的策略（全部播放、忽略新的、重播新的）
    - `SEInfo` 结构体中增加了 `randomGroup`，支持在播放音效时从一组音频中随机选择一个，以增加多样性
- **背景音乐系统 (`BGMManager`)**:
    - 添加了带淡入淡出时间的 `Play` 和 `Stop` 方法重载，以实现平滑的音乐过渡
- **环境音系统**:
    - 新增了 `AmbientManager` 和 `AmbientRegistrySO`，用于独立管理和播放循环的环境音效

### 更改
- **项目结构**: 将 BGM、SFX 和新增的 Ambient 功能分别注册到独立的 `ScriptableObject` 中（`BGMInfoRegistrySO`, `SoundEffectRegistrySO`, `AmbientRegistrySO`），使音频资源的管理更加模块化和清晰

## [1.4.0] - 2025-10-24
### 新增
- **`BGMManager`**: 添加了 `interval` 字段，支持在循环播放背景音乐时设置一个固定的播放间隔

### 修复
- **`BGMManager`**: 修复了在特定条件下，通过 `continuePlayMaxTime` 设置的续播功能无法正确恢复到上次播放进度的问题

## [1.3.0] - 2025-07-31
### 新增
- **`AudioManager`**: 添加了 `keepAliveSource`，通过在后台播放一个静音音频来保持 Unity 音频系统始终激活，解决了从无声状态下首次播放音效可能失败的问题

### 更改
- **项目结构**: 将 `SoundEffectManager` 重命名为 `SFXManager`，以简化命名
- **依赖关系**: 解除了对外部 `Utilities` 和 `CustomAttribute` 包的强耦合引用

## [1.2.0] - 2025-07-29
### 更改
- **项目结构**: 将所有音频系统脚本移动到 `Assets/Plugins/AudioSystem` 目录下，并添加了 `DreemurrStudio.AudioSystem` 命名空间，使其更符合包管理规范

### 修复
- **`SFXManager`**: 修复了音效对象池在回收音源实例时没有正确销毁游戏对象，可能导致内存泄漏的问题

## [1.1.0] - 2025-03-15
### 新增
- **`SFXManager`**: 在 Inspector 面板中暴露了 `maxOverlaySFXNum` 和 `maxWorldSFXNum` 字段，允许根据项目需求限制同时播放的全局音效和世界音效的最大数量
- **动画事件**: 添加了 `AnimationEvent_Audio` 脚本，提供了 `PlayWorldSFX` 和 `PlayOverlaySFX` 等方法，方便在动画剪辑中直接通过事件来触发音效播放

## [1.0.0] - 2025-03-11
### 新增
- **初始版本发布**: 提供了完整的音频播放解决方案
- **`AudioManager`**: 支持通过 `AudioMixer` 控制主音量、BGM、环境音和音效的音量，并支持快照切换
- **`BGMManager`**: 支持背景音乐的播放、暂停、恢复和停止
- **`SFXManager`**: 支持使用对象池播放全局音效（2D）和世界音效（3D）