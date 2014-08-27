using System.ServiceModel;

namespace HyperService
{
	[ServiceContract(CallbackContract = typeof (IGameCallback), SessionMode = SessionMode.Required)]
	public interface IGame
	{
		[OperationContract(IsOneWay = false, IsInitiating = true, IsTerminating = false)]
		Team[] Join(Player player);

		[OperationContract(IsOneWay = true, IsInitiating = false, IsTerminating = true)]
		void Leave();

		[OperationContract(IsOneWay = true, IsInitiating = false, IsTerminating = false)]
		void Play(Player player);
	}
}