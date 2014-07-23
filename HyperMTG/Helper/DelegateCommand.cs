using System;
using System.Windows.Input;

namespace HyperMTG.Helper
{
	public class DelegateCommand : ICommand
	{
		private readonly Action<object> _action;

		public DelegateCommand(Action<object> action)
		{
			_action = action;
		}

		#region ICommand Members

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public event EventHandler CanExecuteChanged
		{
			add { }
			remove { }
		}

		public void Execute(object parameter)
		{
			_action(parameter);
		}

		#endregion
	}
}