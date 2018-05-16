using CAFU.Core.Domain.Repository;
using CAFU.Routing.Data.DataStore;
using JetBrains.Annotations;
using UniRx;

namespace CAFU.Routing.Domain.Repository
{
    [PublicAPI]
    public class RoutingRepository : IRepository
    {
        public class Factory : DefaultRepositoryFactory<RoutingRepository>
        {
            protected override void Initialize(RoutingRepository instance)
            {
                base.Initialize(instance);
                instance.SceneDataStoreResolver = new SceneDataStoreResolver();
            }
        }

        private SceneDataStoreResolver SceneDataStoreResolver { get; set; }

        public IObservable<Data.Entity.SceneEntity> LoadSceneAsObservable(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
        {
            return SceneDataStoreResolver.ResolveSceneDataStore(sceneName).LoadSceneAsObservable(sceneName, loadSceneMode);
        }

        public IObservable<Data.Entity.SceneEntity> UnloadSceneAsObservable(string sceneName)
        {
            return SceneDataStoreResolver.ResolveSceneDataStore(sceneName).UnloadSceneAsObservable(sceneName);
        }
    }
}