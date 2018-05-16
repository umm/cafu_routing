using CAFU.Core.Presentation.View;
using CAFU.Routing.Presentation.Presenter;
using UnityEngine;

namespace CAFU.Routing.Presentation.View.Launcher
{
    public abstract class Controller<TSceneName> : Controller<Controller<TSceneName>, LauncherPresenter, LauncherPresenter.Factory> where TSceneName : struct
    {
        [SerializeField] private TSceneName initialScene;

        private TSceneName InitialScene
        {
            get { return initialScene; }
        }

        protected override void Start()
        {
            this.GetPresenter<LauncherPresenter>().LaunchInitialScene(InitialScene);
        }
    }
}