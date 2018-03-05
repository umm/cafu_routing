using CAFU.Core.Data.DataStore;
using CAFU.Routing.Data.Entity;
using UniRx;
using UnityEngine.SceneManagement;
using UnityModule.AssetBundleManagement;
using UnityModule.ContextManagement;

namespace CAFU.Routing.Data.DataStore {

    public class AssetBundleSceneDataStore : SceneDataStoreBase {

        public class Factory : DefaultDataStoreFactory<AssetBundleSceneDataStore> {

        }

        public override IObservable<SceneEntity> LoadSceneAsObservable(string sceneName, LoadSceneMode loadSceneMode) {
            // 先に AssetBundle からシーンを読み込む
            UnityEngine.Debug.Log(ContextManager.CurrentProject.As<IDownloadableProjectContext>());
            return Loader.GetInstance(ContextManager.CurrentProject.As<IDownloadableProjectContext>())
                .LoadAssetAsObservable<SceneObject>(sceneName)
                .SelectMany(_ => base.LoadSceneAsObservable(sceneName, loadSceneMode));
        }

    }

}