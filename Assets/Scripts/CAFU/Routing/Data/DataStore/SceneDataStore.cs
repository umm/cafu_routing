using System;
using System.Collections.Generic;
using CAFU.Core.Data.DataStore;
using CAFU.Routing.Data.Entity;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
// ReSharper disable UseStringInterpolation

namespace CAFU.Routing.Data.DataStore {

    public interface ISceneDataStore : ISingletonDataStore {

        UniRx.IObservable<SceneEntity> LoadSceneAsObservable(string sceneName, LoadSceneMode loadSceneMode);

        UniRx.IObservable<SceneEntity> UnloadSceneAsObservable(string sceneName);

    }

    // FIXME: Scenes in Build 版と AssetBundle 版とでクラスを分ける
    public class SceneDataStore : ISceneDataStore {

        public class Factory : DefaultDataStoreFactory<SceneDataStore> {

        }

        private Dictionary<string, SceneEntity> sceneEntityCacheMap;

        private Dictionary<string, SceneEntity> SceneEntityCacheMap {
            get {
                if (this.sceneEntityCacheMap == default(Dictionary<string, SceneEntity>)) {
                    this.sceneEntityCacheMap = new Dictionary<string, SceneEntity>();
                }
                return this.sceneEntityCacheMap;
            }
        }

        public UniRx.IObservable<SceneEntity> LoadSceneAsObservable(string sceneName, LoadSceneMode loadSceneMode) {
            if (this.SceneEntityCacheMap.ContainsKey(sceneName)) {
                return Observable.Throw<SceneEntity>(new ArgumentException(string.Format("Scene '{0}' already has loaded.", sceneName)));
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

        public UniRx.IObservable<SceneEntity> UnloadSceneAsObservable(string sceneName) {
            if (!this.SceneEntityCacheMap.ContainsKey(sceneName)) {
                // エディタ実行でない場合には「読み込まれていない」旨を Exception として Throw する
                if (!Application.isEditor) {
                    return Observable.Throw<SceneEntity>(new ArgumentException(string.Format("Scene '{0}' has not loaded yet.", sceneName)));
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