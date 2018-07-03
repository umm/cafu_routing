using System;
using System.Collections.Generic;
using CAFU.Core.Data.DataStore;
using CAFU.Routing.Data.Entity;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CAFU.Routing.Data.DataStore
{
    [PublicAPI]
    public interface ISceneDataStore : ISingletonDataStore
    {
        IObservable<SceneEntity> LoadSceneAsObservable(string sceneName, LoadSceneMode loadSceneMode);

        IObservable<SceneEntity> UnloadSceneAsObservable(string sceneName);
    }

    [PublicAPI]
    public abstract class SceneDataStoreBase : ISceneDataStore
    {
        private Dictionary<string, SceneEntity> SceneEntityCacheMap { get; } = new Dictionary<string, SceneEntity>();

        public virtual IObservable<SceneEntity> LoadSceneAsObservable(string sceneName, LoadSceneMode loadSceneMode)
        {
            if (SceneEntityCacheMap.ContainsKey(sceneName))
            {
                return Observable.Throw<SceneEntity>(new ArgumentException($"Scene '{sceneName}' already has loaded."));
            }

            return SceneManager.LoadSceneAsync(sceneName, loadSceneMode)
                .AsObservable()
                .Select(
                    (_) =>
                    {
                        var sceneEntity = new SceneEntity()
                        {
                            Name = sceneName,
                            UnityScene = SceneManager.GetSceneByName(sceneName)
                        };
                        SceneEntityCacheMap[sceneName] = sceneEntity;
                        return sceneEntity;
                    }
                );
        }

        public virtual IObservable<SceneEntity> UnloadSceneAsObservable(string sceneName)
        {
            if (!SceneEntityCacheMap.ContainsKey(sceneName))
            {
                // エディタの場合に「読み込まれていない」旨を警告する
                if (Application.isEditor)
                {
                    Debug.LogWarning($"Scene '{sceneName}' has not loaded yet.");
                }

                // エディタ実行の場合のみ、初期シーンの直接読み込みを考慮して値を疑似構築する
                SceneEntityCacheMap[sceneName] = new SceneEntity()
                {
                    Name = sceneName,
                    UnityScene = SceneManager.GetSceneByName(sceneName),
                };
            }

            return SceneManager.UnloadSceneAsync(sceneName)
                .AsObservable()
                .Select(
                    (_) =>
                    {
                        var sceneEntity = SceneEntityCacheMap[sceneName];
                        SceneEntityCacheMap.Remove(sceneName);
                        return sceneEntity;
                    }
                );
        }
    }
}
