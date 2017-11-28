using System.Linq;
using CAFU.Core.Domain;
using CAFU.Core.Presentation;
using CAFU.Routing.Data.Entity;
using CAFU.Routing.Domain.Model;
using UniRx;

namespace CAFU.Routing.Domain.Translator {

    public class RoutingTranslator : ITranslator<SceneEntity, SceneModel> {

        public SceneModel Translate(SceneEntity entity) {
            throw new System.NotImplementedException();
        }

        public SceneEntity Translate(SceneModel model) {
            throw new System.NotImplementedException();
        }

        public IObservable<SceneModel> TranslateAsync(SceneEntity entity) {
            SceneModel sceneModel = new SceneModel();
            if (entity.UnityScene.IsValid()) {
                sceneModel.Name = entity.UnityScene.name;
                sceneModel.ViewController = entity.UnityScene
                    .GetRootGameObjects()
                    .ToList()
                    .Find(x => x.GetComponent<IViewController>() != default(IViewController))
                    .GetComponent<IViewController>();
            }
            return Observable.Return(sceneModel);
        }

        public IObservable<SceneEntity> TranslateAsync(SceneModel model) {
            throw new System.NotImplementedException();
        }

    }

}
