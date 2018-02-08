using CAFU.Core.Domain.Repository;
using CAFU.Routing.Data.DataStore;
using UniRx;

namespace CAFU.Routing.Domain.Repository {

    public class RoutingRepository : IRepository {

        public class Factory : DefaultRepositoryFactory<RoutingRepository> {

            protected override void Initialize(RoutingRepository instance) {
                base.Initialize(instance);
                instance.SceneDataStore = new SceneDataStore.Factory().Create();
            }

        }

        private ISceneDataStore SceneDataStore { get; set; }

        public IObservable<Data.Entity.SceneEntity> LoadSceneAsObservable(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode) {
            return this.SceneDataStore.LoadSceneAsObservable(sceneName, loadSceneMode);
        }

        public IObservable<Data.Entity.SceneEntity> UnloadSceneAsObservable(string sceneName) {
            return this.SceneDataStore.UnloadSceneAsObservable(sceneName);
        }

    }

}