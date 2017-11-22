using System.Collections.Generic;
using CAFU.Core.Data;
using CAFU.Routing.Data.Entity;
using UniRx;

namespace CAFU.Routing.Data.DataStore {

    public interface ISceneDataStore : IDataStore {

        IObservable<SceneEntity> LoadSceneAsObservable(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode);

        IObservable<SceneEntity> UnloadSceneAsObservable(string sceneName);

    }

    // FIXME: Scenes in Build 版と AssetBundle 版とでクラスを分ける
    public class SceneDataStore : ISceneDataStore {

        private Dictionary<string, SceneEntity> sceneEntityCacheMap;

        public Dictionary<string, SceneEntity> SceneEntityCacheMap {
            get {
                if (this.sceneEntityCacheMap == default(Dictionary<string, SceneEntity>)) {
                    this.sceneEntityCacheMap = new Dictionary<string, SceneEntity>();
                }
                return this.sceneEntityCacheMap;
            }
            set {
                this.sceneEntityCacheMap = value;
            }
        }

        public IObservable<SceneEntity> LoadSceneAsObservable(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode) {
            if (this.SceneEntityCacheMap.ContainsKey(sceneName)) {
                return Observable.Throw<SceneEntity>(new System.ArgumentException(string.Format("Scene '{0}' already has loaded.", sceneName)));
            }
            return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, loadSceneMode)
                .AsObservable()
                .Select(
                    (_) => {
                        SceneEntity sceneEntity = new SceneEntity() {
                            UnityScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName)
                        };
                        this.SceneEntityCacheMap[sceneName] = sceneEntity;
                        return sceneEntity;
                    }
                );
        }

        public IObservable<SceneEntity> UnloadSceneAsObservable(string sceneName) {
            if (!this.SceneEntityCacheMap.ContainsKey(sceneName)) {
                // エディタ実行でない場合には「読み込まれていない」旨を Exception として Throw する
                if (!UnityEngine.Application.isEditor) {
                    return Observable.Throw<SceneEntity>(new System.ArgumentException(string.Format("Scene '{0}' has not loaded yet.", sceneName)));
                }
                // エディタ実行の場合のみ、初期シーンの直接読み込みを考慮して値を疑似構築する
                this.SceneEntityCacheMap[sceneName] = new SceneEntity() {
                    UnityScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName),
                };
            }
            return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName)
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