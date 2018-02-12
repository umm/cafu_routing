using System.Linq;
using CAFU.Core.Domain.Translator;
using CAFU.Core.Presentation.View;
using CAFU.Routing.Data.Entity;
using CAFU.Routing.Domain.Model;
using UniRx;

namespace CAFU.Routing.Domain.Translator {

    public interface IRoutingTranslator : IAsyncModelTranslator<SceneEntity, SceneModel> {

    }

    public class RoutingTranslator : IRoutingTranslator {

        public class Factory : DefaultTranslatorFactory<RoutingTranslator> {

        }

        public IObservable<SceneModel> TranslateAsObservable(SceneEntity entity) {
            SceneModel sceneModel = new SceneModel {
                Name = entity.Name,
            };
            if (entity.UnityScene.IsValid()) {
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
