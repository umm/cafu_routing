using System;
using CAFU.Core.Presentation.Presenter;
using CAFU.Routing.Domain.UseCase;
using UnityEngine.SceneManagement;

namespace CAFU.Routing.Presentation.Presenter {

    public class LauncherPresenter : IPresenter {

        // FIXME: Use Zenject
        public class Factory : DefaultPresenterFactory<Factory, LauncherPresenter> {

            protected override void Initialize(LauncherPresenter instance) {
                base.Initialize(instance);
                instance.RoutingUseCase = RoutingUseCase.Factory.Instance.Create();
            }

        }

        private const string LAUNCHER_SCENE_NAME = "Launcher";

        private RoutingUseCase RoutingUseCase { get; set; }

        public void LaunchInitialScene(string sceneName) {
            if (sceneName == LAUNCHER_SCENE_NAME) {
                throw new ArgumentException(string.Format("Scene '{0}' cannot set as initial scene.", LAUNCHER_SCENE_NAME));
            }
            this.RoutingUseCase.LoadScene(sceneName, LoadSceneMode.Single);
        }

    }

}