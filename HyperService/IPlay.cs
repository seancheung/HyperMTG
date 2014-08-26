using System.ServiceModel;

namespace HyperService
{
	[ServiceContract(CallbackContract = typeof (IPlayCallback), SessionMode = SessionMode.Required)]
	public interface IPlay
	{
		[OperationContract(IsInitiating = true)]
		bool Connect(Player player);
	}
}