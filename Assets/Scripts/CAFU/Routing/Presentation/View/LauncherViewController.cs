using CAFU.Core.Domain;
using CAFU.Core.Presentation;
using CAFU.Routing.Domain.UseCase;
using CAFU.Routing.Presentation.Presenter;
using UnityEngine;
using UnityModule;

namespace CAFU.Routing.Presentation.View {

    public class LauncherViewController : ViewControllerBase<LauncherPresenter>, IViewControllerBuilder {

        [SerializeField][SceneName]
        private string initialScene;

        private string InitialScene {
            get {
                return this.initialScene;
            }
        }

        protected override void Start() {
            base.Start();
            this.Presenter.LaunchInitialScene(this.InitialScene);
        }

        public void Build() {
            this.Presenter = new LauncherPresenter() {
                RoutingUseCase = UseCaseFactory.GetOrCreateInstance<RoutingUseCase>(),
            };
        }

    }

}