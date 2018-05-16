using System.Collections.Generic;
using CAFU.Core.Domain.UseCase;
using CAFU.Routing.Domain.Model;
using CAFU.Routing.Domain.Repository;
using CAFU.Routing.Domain.Translator;
using JetBrains.Annotations;
using UniRx;
using UnityEngine.SceneManagement;
using UnityModule.ContextManagement;

namespace CAFU.Routing.Domain.UseCase
{
    [PublicAPI]
    public interface IRoutingUseCase : ISingletonUseCase
    {
        void LoadScene<TSceneName>(TSceneName sceneName, LoadSceneMode loadSceneMode) where TSceneName : struct;

        void LoadScene(string sceneName, LoadSceneMode loadSceneMode);

        void UnloadScene<TSceneName>(TSceneName sceneName) where TSceneName : struct;

        void UnloadScene(string sceneName);

        IObservable<SceneModel> LoadSceneAsObservable<TSceneName>(TSceneName sceneName, LoadSceneMode loadSceneMode) where TSceneName : struct;

        IObservable<SceneModel> LoadSceneAsObservable(string sceneName, LoadSceneMode loadSceneMode);

        IObservable<SceneModel> UnloadSceneAsObservable<TSceneName>(TSceneName sceneName) where TSceneName : struct;

        IObservable<SceneModel> UnloadSceneAsObservable(string sceneName);

        IObservable<SceneModel> OnLoadSceneAsObservable();

        IObservable<SceneModel> OnLoadSceneAsObservable<TSceneName>(TSceneName sceneName) where TSceneName : struct;

        IObservable<SceneModel> OnLoadSceneAsObservable(string sceneName);

        IObservable<SceneModel> OnUnloadSceneAsObservable();

        IObservable<SceneModel> OnUnloadSceneAsObservable<TSceneName>(TSceneName sceneName) where TSceneName : struct;

        IObservable<SceneModel> OnUnloadSceneAsObservable(string sceneName);

        bool HasLoaded<TSceneName>(TSceneName sceneName) where TSceneName : struct;

        bool HasLoaded(string sceneName);
    }

    [PublicAPI]
    public class RoutingUseCase : IRoutingUseCase
    {
        public class Factory : DefaultUseCaseFactory<RoutingUseCase>
        {
            protected override void Initialize(RoutingUseCase instance)
            {
                base.Initialize(instance);
                instance.LoadSceneSubject = new Subject<SceneModel>();
                instance.UnloadSceneSubject = new Subject<SceneModel>();
                instance.RoutingRepository = new RoutingRepository.Factory().Create();
                instance.RoutingTranslator = new RoutingTranslator.Factory().Create();
                instance.Initialize();
            }
        }

        private RoutingRepository RoutingRepository { get; set; }

        private RoutingTranslator RoutingTranslator { get; set; }

        private Subject<SceneModel> LoadSceneSubject { get; set; }

        private Subject<SceneModel> UnloadSceneSubject { get; set; }

        private List<SceneModel> LoadedSceneModelList { get; set; }

        public void LoadScene<TSceneName>(TSceneName sceneName, LoadSceneMode loadSceneMode) where TSceneName : struct
        {
            LoadScene(ContextManager.CurrentProject.CreateSceneName(sceneName), loadSceneMode);
        }

        public void LoadScene(string sceneName, LoadSceneMode loadSceneMode)
        {
            LoadSceneAsObservable(sceneName, loadSceneMode).Subscribe();
        }

        public void UnloadScene<TSceneName>(TSceneName sceneName) where TSceneName : struct
        {
            UnloadScene(ContextManager.CurrentProject.CreateSceneName(sceneName));
        }

        public void UnloadScene(string sceneName)
        {
            UnloadSceneAsObservable(sceneName).Subscribe();
        }

        public IObservable<SceneModel> LoadSceneAsObservable<TSceneName>(TSceneName sceneName, LoadSceneMode loadSceneMode) where TSceneName : struct
        {
            return LoadSceneAsObservable(ContextManager.CurrentProject.CreateSceneName(sceneName), loadSceneMode);
        }

        public IObservable<SceneModel> LoadSceneAsObservable(string sceneName, LoadSceneMode loadSceneMode)
        {
            var stream = RoutingRepository
                .LoadSceneAsObservable(sceneName, loadSceneMode)
                .SelectMany(x => RoutingTranslator.TranslateAsObservable(x))
                .Share();
            // OnComplete を流してしまうと、Subject が閉じてしまうので OnNext, OnError のみを流す
            stream
                .Subscribe(
                    LoadSceneSubject.OnNext,
                    LoadSceneSubject.OnError
                );
            return stream;
        }

        public IObservable<SceneModel> UnloadSceneAsObservable<TSceneName>(TSceneName sceneName) where TSceneName : struct
        {
            return UnloadSceneAsObservable(ContextManager.CurrentProject.CreateSceneName(sceneName));
        }

        public IObservable<SceneModel> UnloadSceneAsObservable(string sceneName)
        {
            var stream = RoutingRepository
                .UnloadSceneAsObservable(sceneName)
                .SelectMany(x => RoutingTranslator.TranslateAsObservable(x))
                .Share();
            // OnComplete を流してしまうと、Subject が閉じてしまうので OnNext, OnError のみを流す
            stream
                .Subscribe(
                    UnloadSceneSubject.OnNext,
                    UnloadSceneSubject.OnError
                );
            return stream;
        }

        public IObservable<SceneModel> OnLoadSceneAsObservable()
        {
            return LoadSceneSubject.AsObservable();
        }

        public IObservable<SceneModel> OnLoadSceneAsObservable<TSceneName>(TSceneName sceneName) where TSceneName : struct
        {
            return OnLoadSceneAsObservable(ContextManager.CurrentProject.CreateSceneName(sceneName));
        }

        public IObservable<SceneModel> OnLoadSceneAsObservable(string sceneName)
        {
            return OnLoadSceneAsObservable().Where(x => x.Name == sceneName).AsObservable();
        }

        public IObservable<SceneModel> OnUnloadSceneAsObservable()
        {
            return UnloadSceneSubject.AsObservable();
        }

        public IObservable<SceneModel> OnUnloadSceneAsObservable<TSceneName>(TSceneName sceneName) where TSceneName : struct
        {
            return OnUnloadSceneAsObservable(ContextManager.CurrentProject.CreateSceneName(sceneName));
        }

        public IObservable<SceneModel> OnUnloadSceneAsObservable(string sceneName)
        {
            return OnUnloadSceneAsObservable().Where(x => x.Name == sceneName).AsObservable();
        }

        public bool HasLoaded<TSceneName>(TSceneName sceneName) where TSceneName : struct
        {
            return HasLoaded(ContextManager.CurrentProject.CreateSceneName(sceneName));
        }

        public bool HasLoaded(string sceneName)
        {
            return LoadedSceneModelList.Exists(x => x.Name == sceneName);
        }

        private void Initialize()
        {
            LoadedSceneModelList = new List<SceneModel>();
            OnLoadSceneAsObservable().Subscribe(x => LoadedSceneModelList.Add(x));
            OnUnloadSceneAsObservable().Subscribe(x => LoadedSceneModelList.RemoveAll(y => y.Name == x.Name));
        }
    }
}