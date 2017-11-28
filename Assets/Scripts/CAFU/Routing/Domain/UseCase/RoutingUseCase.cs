using System.Collections.Generic;
using CAFU.Core.Domain;
using CAFU.Routing.Domain.Model;
using CAFU.Routing.Domain.Repository;
using CAFU.Routing.Domain.Translator;
using UniRx;
using UnityEngine.SceneManagement;

namespace CAFU.Routing.Domain.UseCase {

    public class RoutingUseCase : IUseCaseAsSingleton, IUseCaseBuilder {

        public void Build() {
            this.RoutingRepository = new RoutingRepository();
            this.RoutingTranslator = new RoutingTranslator();
        }

        private RoutingRepository RoutingRepository { get; set; }

        private RoutingTranslator RoutingTranslator { get; set; }

        private Dictionary<string, SceneModel> sceneModelMap;

        private Dictionary<string, SceneModel> SceneModelMap {
            get {
                if (this.sceneModelMap == default(Dictionary<string, SceneModel>)) {
                    this.sceneModelMap = new Dictionary<string, SceneModel>();
                }
                return this.sceneModelMap;
            }
        }

        private Dictionary<string, Subject<SceneModel>> onLoadSceneSubjectMap;

        private Dictionary<string, Subject<SceneModel>> OnLoadSceneSubjectMap {
            get {
                if (this.onLoadSceneSubjectMap == default(Dictionary<string, Subject<SceneModel>>)) {
                    this.onLoadSceneSubjectMap = new Dictionary<string, Subject<SceneModel>>();
                }
                return this.onLoadSceneSubjectMap;
            }
        }

        private Dictionary<string, Subject<SceneModel>> onUnloadSceneSubjectMap;

        private Dictionary<string, Subject<SceneModel>> OnUnloadSceneSubjectMap {
            get {
                if (this.onUnloadSceneSubjectMap == default(Dictionary<string, Subject<SceneModel>>)) {
                    this.onUnloadSceneSubjectMap = new Dictionary<string, Subject<SceneModel>>();
                }
                return this.onUnloadSceneSubjectMap;
            }
        }

        public void LoadScene(string sceneName, LoadSceneMode loadSceneMode) {
            this.LoadSceneAsObservable(sceneName, loadSceneMode).Subscribe();
        }

        public void UnloadScene(string sceneName) {
            this.UnloadSceneAsObservable(sceneName).Subscribe();
        }

        public bool HasSceneLoaded(string sceneName) {
            return this.SceneModelMap.ContainsKey(sceneName) && this.SceneModelMap[sceneName] != default(SceneModel);
        }

        private IObservable<SceneModel> LoadSceneAsObservable(string sceneName, LoadSceneMode loadSceneMode) {
            IObservable<SceneModel> stream = this.RoutingRepository
                .LoadSceneAsObservable(sceneName, loadSceneMode)
                .SelectMany(x => this.RoutingTranslator.TranslateAsync(x))
                .Share();
            // OnComplete を流してしまうと、Subject が閉じてしまうので OnNext, OnError のみを流す
            stream.Subscribe(
                this.GetLoadSceneSubject(sceneName).OnNext,
                this.GetLoadSceneSubject(sceneName).OnError
            );
            return stream;
        }

        private IObservable<SceneModel> UnloadSceneAsObservable(string sceneName) {
            IObservable<SceneModel> stream = this.RoutingRepository
                .UnloadSceneAsObservable(sceneName)
                .SelectMany(x => this.RoutingTranslator.TranslateAsync(x))
                .Share();
            // OnComplete を流してしまうと、Subject が閉じてしまうので OnNext, OnError のみを流す
            stream.Subscribe(
                this.GetUnloadSceneSubject(sceneName).OnNext,
                this.GetUnloadSceneSubject(sceneName).OnError
            );
            return stream;
        }

        public IObservable<SceneModel> OnLoadSceneAsObservable(string sceneName) {
            return this.GetLoadSceneSubject(sceneName).AsObservable();
        }

        public IObservable<SceneModel> OnUnloadSceneAsObservable(string sceneName) {
            return this.GetUnloadSceneSubject(sceneName).AsObservable();
        }

        private Subject<SceneModel> GetLoadSceneSubject(string sceneName) {
            if (!this.OnLoadSceneSubjectMap.ContainsKey(sceneName)) {
                this.OnLoadSceneSubjectMap[sceneName] = new Subject<SceneModel>();
                this.OnLoadSceneSubjectMap[sceneName]
                    .Where(x => !string.IsNullOrEmpty(x.Name) && !this.SceneModelMap.ContainsKey(x.Name))
                    .Subscribe(x => this.SceneModelMap[x.Name] = x);
            }
            return this.OnLoadSceneSubjectMap[sceneName];
        }

        private Subject<SceneModel> GetUnloadSceneSubject(string sceneName) {
            if (!this.OnUnloadSceneSubjectMap.ContainsKey(sceneName)) {
                this.OnUnloadSceneSubjectMap[sceneName] = new Subject<SceneModel>();
                // XXX: 本来はストリームに流れてくる値を使うべきだが、 Unload の場合は値が入らないのでラムダ式キャプチャする
                this.OnUnloadSceneSubjectMap[sceneName]
                    .Where(_ => !string.IsNullOrEmpty(sceneName) && this.SceneModelMap.ContainsKey(sceneName))
                    .Subscribe(_ => this.SceneModelMap.Remove(sceneName));
            }
            return this.OnUnloadSceneSubjectMap[sceneName];
        }

    }

}
