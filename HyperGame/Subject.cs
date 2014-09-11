namespace HyperKore.Game
{
	public delegate void UpdateDelegate();

	public interface ISubject
	{
		event UpdateDelegate UpdateHandler;

		void Attach(UpdateDelegate update);

		void Detach(UpdateDelegate update);

		void Notify();
	}

	public class Subject : ISubject
	{
		#region Implementation of ISubject

		public event UpdateDelegate UpdateHandler;

		public void Attach(UpdateDelegate update)
		{
			UpdateHandler -= update;
			UpdateHandler += update;
		}

		public void Detach(UpdateDelegate update)
		{
			UpdateHandler -= update;
		}

		public void Notify()
		{
			if (UpdateHandler != null)
			{
				UpdateHandler();
			}
		}

		#endregion
	}
}