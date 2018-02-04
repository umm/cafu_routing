using CAFU.Core.Presentation.Presenter;
using CAFU.Core.Presentation.View;
using CAFU.Routing.Presentation.Presenter;
using UnityEngine;
using UnityModule;

namespace CAFU.Routing.Presentation.View {

    public class Controller : Controller<LauncherPresenter, LauncherPresenter.Factory> {

        [SerializeField][SceneName]
        private string initialScene;

        private string InitialScene {
            get {
                return this.initialScene;
            }
        }

        protected override void Start() {
            this.GetPresenter().LaunchInitialScene(this.InitialScene);
        }

    }

    public static class ViewExtension {

        public static LauncherPresenter GetPresenter(this IView view) {
            return ControllerInstanceManager.Instance.Get(view.GetType().Namespace).Presenter.As<LauncherPresenter>();
        }

    }

}