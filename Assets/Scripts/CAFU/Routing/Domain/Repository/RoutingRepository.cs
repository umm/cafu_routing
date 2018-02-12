using CAFU.Core.Domain.Repository;
using CAFU.Routing.Data.DataStore;
using UniRx;
using Zenject;

namespace CAFU.Routing.Domain.Repository {

    public interface IRoutingRepository : IRepository {

        IObservable<Data.Entity.SceneEntity> LoadSceneAsObservable(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode);

        IObservable<Data.Entity.SceneEntity> UnloadSceneAsObservable(string sceneName);

    }

    public class RoutingRepository : IRoutingRepository {

        public class Factory : DefaultRepositoryFactory<RoutingRepository> {

            protected override void Initialize(RoutingRepository instance) {
                base.Initialize(instance);
                instance.SceneDataStore = new SceneDataStore.Factory().Create();
            }

        }

        [Inject]
        private ISceneDataStore SceneDataStore { get; set; }

        public IObservable<Data.Entity.SceneEntity> LoadSceneAsObservable(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode) {
            return this.SceneDataStore.LoadSceneAsObservable(sceneName, loadSceneMode);
        }

        public IObservable<Data.Entity.SceneEntity> UnloadSceneAsObservable(string sceneName) {
            return this.SceneDataStore.UnloadSceneAsObservable(sceneName);
        }

    }

}
