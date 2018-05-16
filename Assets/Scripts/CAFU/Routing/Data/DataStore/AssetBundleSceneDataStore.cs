using CAFU.Core.Data.DataStore;
using CAFU.Routing.Data.Entity;
using JetBrains.Annotations;
using UniRx;
using UnityEngine.SceneManagement;
using UnityModule.AssetBundleManagement;
using UnityModule.ContextManagement;

namespace CAFU.Routing.Data.DataStore
{
    [PublicAPI]
    public class AssetBundleSceneDataStore : SceneDataStoreBase
    {
        public class Factory : DefaultDataStoreFactory<AssetBundleSceneDataStore>
        {
        }

        public override IObservable<SceneEntity> LoadSceneAsObservable(string sceneName, LoadSceneMode loadSceneMode)
        {
            // 先に AssetBundle からシーンを読み込む
            return Loader.GetInstance(ContextManager.CurrentProject.As<IDownloadableProjectContext>())
                .LoadAssetAsObservable<SceneObject>(sceneName)
                .SelectMany(_ => base.LoadSceneAsObservable(sceneName, loadSceneMode));
        }
    }
}