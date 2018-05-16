﻿using System.Linq;
using CAFU.Core.Domain.Translator;
using CAFU.Core.Presentation.View;
using CAFU.Routing.Data.Entity;
using CAFU.Routing.Domain.Model;
using UniRx;

namespace CAFU.Routing.Domain.Translator
{
    public class RoutingTranslator : IAsyncModelTranslator<SceneEntity, SceneModel>
    {
        public class Factory : DefaultTranslatorFactory<RoutingTranslator>
        {
        }

        public IObservable<SceneModel> TranslateAsObservable(SceneEntity entity)
        {
            var sceneModel = new SceneModel
            {
                Name = entity.Name,
            };
            if (entity.UnityScene.IsValid())
            {
                sceneModel.RootGameObjects = entity.UnityScene.GetRootGameObjects();
                sceneModel.Controller = entity.UnityScene
                    .GetRootGameObjects()
                    .ToList()
                    .Find(x => x.GetComponent<IController>() != default(IController))
                    .GetComponent<IController>();
            }

            return Observable.Return(sceneModel);
        }
    }
}