using CAFU.Core.Domain.Repository;
using CAFU.Routing.Data.DataStore;
using UniRx;

namespace CAFU.Routing.Domain.Repository {

    public class RoutingRepository : IRepository {

        private ISceneDataStore sceneDataStore;

        private ISceneDataStore SceneDataStore {
            get {
                if (this.sceneDataStore == default(ISceneDataStore)) {
                    this.sceneDataStore = CreateSceneDataStore();
                }
                return this.sceneDataStore;
            }
        }

        public IObservable<Data.Entity.SceneEntity> LoadSceneAsObservable(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode) {
            return this.SceneDataStore.LoadSceneAsObservable(sceneName, loadSceneMode);
        }

        public IObservable<Data.Entity.SceneEntity> UnloadSceneAsObservable(string sceneName) {
            return this.SceneDataStore.UnloadSceneAsObservable(sceneName);
        }

        // 暫定的に Repository 側に Factory メソッドを移動させてみる
        private static ISceneDataStore CreateSceneDataStore() {
            return new SceneDataStore();
        }

    }

}