using CAFU.Core.Domain;
using CAFU.Core.Presentation;

namespace CAFU.Routing.Domain.Model {

    public class SceneModel : ModelBase {

        public string Name { get; set; }

        public IViewController ViewController { get; set; }

    }

}