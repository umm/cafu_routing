using CAFU.Core.Presentation.View;
using CAFU.Routing.Presentation.Presenter;
using UnityEngine;
using UnityModule;
// ReSharper disable ArrangeAccessorOwnerBody
// ReSharper disable UnusedMember.Global

namespace CAFU.Routing.Presentation.View.Launcher {

    public abstract class Controller<TSceneName> : Controller<Controller, LauncherPresenter, LauncherPresenter.Factory> where TSceneName : struct {

        [SerializeField]
        private TSceneName initialScene;

        private TSceneName InitialScene {
            get {
                return this.initialScene;
            }
        }

        protected override void Start() {
            this.GetPresenter().LaunchInitialScene(this.InitialScene);
        }

    }

    public class Controller : Controller<Controller, LauncherPresenter, LauncherPresenter.Factory> {

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
            return Controller.Instance.Presenter;
        }

    }

}