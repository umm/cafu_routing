using CAFU.Core.Presentation.View;
using CAFU.Routing.Presentation.Presenter;
using UnityEngine;

// ReSharper disable ArrangeAccessorOwnerBody
// ReSharper disable UnusedMember.Global

namespace CAFU.Routing.Presentation.View.Launcher {

    public abstract class Controller<TSceneName> : Controller<Controller<TSceneName>, LauncherPresenter, LauncherPresenter.Factory> where TSceneName : struct {

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

}