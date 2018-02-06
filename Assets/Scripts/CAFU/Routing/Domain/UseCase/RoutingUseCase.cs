using CAFU.Core.Domain.UseCase;
using CAFU.Routing.Domain.Model;
using CAFU.Routing.Domain.Repository;
using CAFU.Routing.Domain.Translator;
using UniRx;
using UnityEngine.SceneManagement;

namespace CAFU.Routing.Domain.UseCase {

    public class RoutingUseCase : ISingletonUseCase {

        // FIXME: Use Zenject
        public class Factory : DefaultUseCaseFactory<Factory, RoutingUseCase> {

            protected override void Initialize(RoutingUseCase instance) {
                base.Initialize(instance);
                instance.LoadSceneSubject = new Subject<SceneModel>();
                instance.UnloadSceneSubject = new Subject<SceneModel>();
                instance.RoutingRepository = RoutingRepository.Factory.Instance.Create();
                instance.RoutingTranslator = RoutingTranslator.Factory.Instance.Create();
            }

        }

        private RoutingRepository RoutingRepository { get; set; }

        private RoutingTranslator RoutingTranslator { get; set; }

        private Subject<SceneModel> LoadSceneSubject { get; set; }

        private Subject<SceneModel> UnloadSceneSubject { get; set; }

        public void LoadScene(string sceneName, LoadSceneMode loadSceneMode) {
            this.LoadSceneAsObservable(sceneName, loadSceneMode).Subscribe();
        }

        public void UnloadScene(string sceneName) {
            this.UnloadSceneAsObservable(sceneName).Subscribe();
        }

        public IObservable<SceneModel> LoadSceneAsObservable(string sceneName, LoadSceneMode loadSceneMode) {
            IObservable<SceneModel> stream = this.RoutingRepository
                .LoadSceneAsObservable(sceneName, loadSceneMode)
                .SelectMany(x => this.RoutingTranslator.TranslateAsObservable(x))
                .Share();
            // OnComplete を流してしまうと、Subject が閉じてしまうので OnNext, OnError のみを流す
            stream
                .Subscribe(
                    this.LoadSceneSubject.OnNext,
                    this.LoadSceneSubject.OnError
                );
            return stream;
        }

        public IObservable<SceneModel> UnloadSceneAsObservable(string sceneName) {
            IObservable<SceneModel> stream = this.RoutingRepository
                .UnloadSceneAsObservable(sceneName)
                .SelectMany(x => this.RoutingTranslator.TranslateAsObservable(x))
                .Share();
            // OnComplete を流してしまうと、Subject が閉じてしまうので OnNext, OnError のみを流す
            stream
                .Subscribe(
                    this.UnloadSceneSubject.OnNext,
                    this.UnloadSceneSubject.OnError
                );
            return stream;
        }

        public IObservable<SceneModel> OnLoadSceneAsObservable() {
            return this.LoadSceneSubject.AsObservable();
        }

        public IObservable<SceneModel> OnLoadSceneAsObservable(string sceneName) {
            return this.OnLoadSceneAsObservable().Where(x => x.Name == sceneName).AsObservable();
        }

        public IObservable<SceneModel> OnUnloadSceneAsObservable() {
            return this.UnloadSceneSubject.AsObservable();
        }

        public IObservable<SceneModel> OnUnloadSceneAsObservable(string sceneName) {
            return this.OnUnloadSceneAsObservable().Where(x => x.Name == sceneName).AsObservable();
        }

    }

}
