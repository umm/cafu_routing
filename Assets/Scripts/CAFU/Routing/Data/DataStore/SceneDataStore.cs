using System;
using System.Collections.Generic;
using CAFU.Core.Data.DataStore;
using CAFU.Routing.Data.Entity;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CAFU.Routing.Data.DataStore {

    public interface ISceneDataStore : ISingletonDataStore {

        UniRx.IObservable<SceneEntity> LoadSceneAsObservable(string sceneName, LoadSceneMode loadSceneMode);

        UniRx.IObservable<SceneEntity> UnloadSceneAsObservable(string sceneName);

    }

    [Obsolete("Please use `InBuildSceneDataStore` instead of this class.")]
    public class SceneDataStore : InBuildSceneDataStore {

    }

    public abstract class SceneDataStoreBase : ISceneDataStore {

        private Dictionary<string, SceneEntity> SceneEntityCacheMap { get; } = new Dictionary<string, SceneEntity>();

        public virtual UniRx.IObservable<SceneEntity> LoadSceneAsObservable(string sceneName, LoadSceneMode loadSceneMode) {
            if (this.SceneEntityCacheMap.ContainsKey(sceneName)) {
                return Observable.Throw<SceneEntity>(new ArgumentException($"Scene '{sceneName}' already has loaded."));
            }
            return SceneManager.LoadSceneAsync(sceneName, loadSceneMode)
                .AsObservable()
                .Select(
                    (_) => {
                        SceneEntity sceneEntity = new SceneEntity() {
                            Name = sceneName,
                            UnityScene = SceneManager.GetSceneByName(sceneName)
                        };
                        this.SceneEntityCacheMap[sceneName] = sceneEntity;
                        return sceneEntity;
                    }
                );
        }

        public virtual UniRx.IObservable<SceneEntity> UnloadSceneAsObservable(string sceneName) {
            if (!this.SceneEntityCacheMap.ContainsKey(sceneName)) {
                // エディタ実行でない場合には「読み込まれていない」旨を Exception として Throw する
                if (!Application.isEditor) {
                    return Observable.Throw<SceneEntity>(new ArgumentException($"Scene '{sceneName}' has not loaded yet."));
                }
                // エディタ実行の場合のみ、初期シーンの直接読み込みを考慮して値を疑似構築する
                this.SceneEntityCacheMap[sceneName] = new SceneEntity() {
                    Name = sceneName,
                    UnityScene = SceneManager.GetSceneByName(sceneName),
                };
            }
            return SceneManager.UnloadSceneAsync(sceneName)
                .AsObservable()
                .Select(
                    (_) => {
                        SceneEntity sceneEntity = this.SceneEntityCacheMap[sceneName];
                        this.SceneEntityCacheMap.Remove(sceneName);
                        return sceneEntity;
                    }
                );
        }

    }

}