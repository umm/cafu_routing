using CAFU.Routing.Data.DataStore;
using CAFU.Routing.Domain.Repository;
using CAFU.Routing.Domain.Translator;
using CAFU.Routing.Domain.UseCase;
using Zenject;
// ReSharper disable ClassNeverInstantiated.Global

namespace Modules.Scripts.CAFU.Routing {

    public class RoutingInstaller : Installer<RoutingInstaller> {

        public override void InstallBindings() {
            this.Container.Bind<IRoutingUseCase>().To<RoutingUseCase>().AsSingle();
            this.Container.Bind<IRoutingRepository>().To<RoutingRepository>().AsTransient();
            this.Container.Bind<IRoutingTranslator>().To<RoutingTranslator>().AsTransient();
            this.Container.Bind<ISceneDataStore>().To<SceneDataStore>().AsCached();
        }

    }

}