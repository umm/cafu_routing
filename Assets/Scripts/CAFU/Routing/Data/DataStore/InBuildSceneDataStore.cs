using CAFU.Core.Data.DataStore;
using JetBrains.Annotations;

namespace CAFU.Routing.Data.DataStore
{
    [PublicAPI]
    public class InBuildSceneDataStore : SceneDataStoreBase
    {
        public class Factory : DefaultDataStoreFactory<InBuildSceneDataStore>
        {
        }
    }
}