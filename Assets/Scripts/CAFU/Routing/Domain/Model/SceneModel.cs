using CAFU.Core.Domain.Model;
using CAFU.Core.Presentation.View;
using UnityEngine;

namespace CAFU.Routing.Domain.Model {

    public class SceneModel : IModel {

        public string Name { get; set; }

        public IController Controller { get; set; }

        public GameObject[] RootGameObjects { get; set; }

    }

}