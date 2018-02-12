using CAFU.Core.Presentation.Presenter;
using CAFU.Routing.Domain.UseCase;

namespace CAFU.Routing.Presentation.Presenter {

    public interface IRoutingPresenter : IPresenter {

        IRoutingUseCase RoutingUseCase { get; }

    }

}