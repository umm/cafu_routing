using CAFU.Core.Presentation;
using CAFU.Routing.Domain.UseCase;
using CAFU.Routing.Presentation.Presenter;
using UnityEngine;
using UnityModule;

namespace CAFU.Routing.Presentation.View {

    public class LauncherViewController : ViewControllerBase<LauncherPresenter> {

        [SerializeField][SceneName]
        private string initialScene;

        private string InitialScene {
            get {
                return this.initialScene;
            }
        }

        private void Start() {
            this.Presenter.LaunchInitialScene(this.InitialScene);
        }

        protected override void Build() {
            this.Presenter = new LauncherPresenter() {
                RoutingUseCase = RoutingUseCase.GetOrCreateInstance(),
            };
        }

    }

}