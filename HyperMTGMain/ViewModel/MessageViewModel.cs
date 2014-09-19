using System;
using System.Windows.Input;
using HyperMTGMain.Helper;
using HyperMTGMain.View;

namespace HyperMTGMain.ViewModel
{
	public class MessageViewModel : ObservableClass
	{
		public enum MessageStyle
		{
			OK,
			YesNO,
			YesNoCancel
		}

		private static MessageViewModel _instance;
		private Action _action;
		private string _caption;
		private string _info;
		private bool _isCancelVisible;
		private bool _isNoVisible;
		private bool _isOkVisible;
		private bool _isYesVisible;
		private string _title;

		private MessageViewModel()
		{
		}

		internal static MessageViewModel Instance
		{
			get { return _instance ?? (_instance = new MessageViewModel()); }
		}

		#region Notifiable

		public string Info
		{
			get { return _info; }
			set
			{
				_info = value;
				OnPropertyChanged("Info");
			}
		}

		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				OnPropertyChanged("Title");
			}
		}

		public string Caption
		{
			get { return _caption; }
			set
			{
				_caption = value;
				OnPropertyChanged("Title");
			}
		}

		public bool IsOKVisible
		{
			get { return _isOkVisible; }
			set
			{
				_isOkVisible = value;
				OnPropertyChanged("IsOKVisible");
			}
		}

		public bool IsYesVisible
		{
			get { return _isYesVisible; }
			set
			{
				_isYesVisible = value;
				OnPropertyChanged("IsYesVisible");
			}
		}

		public bool IsNoVisible
		{
			get { return _isNoVisible; }
			set
			{
				_isNoVisible = value;
				OnPropertyChanged("IsNoVisible");
			}
		}

		public bool IsCancelVisible
		{
			get { return _isCancelVisible; }
			set
			{
				_isCancelVisible = value;
				OnPropertyChanged("IsCancelVisible");
			}
		}

		#endregion


		#region Commands

		public ICommand CancelCommand
		{
			get { return new RelayCommand(Cancel); }
		}

		public ICommand YesCommand
		{
			get { return new RelayCommand(Yes); }
		}

		public ICommand NoCommand
		{
			get { return new RelayCommand(No); }
		}

		public ICommand OkCommand
		{
			get { return new RelayCommand(Ok); }
		}

		private void Cancel()
		{
			ViewManager.MessageWindow.Close();
			_action = null;
		}

		private void Ok()
		{
			if (_action != null)
			{
				_action.Invoke();
			}
			ViewManager.MessageWindow.Close();
			_action = null;
		}

		private void Yes()
		{
			if (_action != null)
			{
				_action.Invoke();
			}
			ViewManager.MessageWindow.Close();
			_action = null;
		}

		private void No()
		{
			ViewManager.MessageWindow.Close();
			_action = null;
		}

		#endregion

		public void Clear()
		{
			Info = null;
			Title = null;
			Caption = null;
		}

		public void Inform(string format, params object[] args)
		{
			Info = string.Format(format, args);
		}

		public void Message(string format, params object[] args)
		{
			Title = "Warnning";
			Caption = string.Format(format, args);
			SetStyle(MessageStyle.OK);
			ViewManager.MessageWindow.ShowDialog();
		}

		public void Message(string title, string caption, MessageStyle style, Action action)
		{
			Title = title;
			Caption = caption;
			_action = action;
			SetStyle(style);
			ViewManager.MessageWindow.ShowDialog();
		}

		private void SetStyle(MessageStyle style)
		{
			switch (style)
			{
				case MessageStyle.OK:
					IsOKVisible = true;
					IsYesVisible = false;
					IsNoVisible = false;
					IsCancelVisible = false;
					break;
				case MessageStyle.YesNO:
					IsOKVisible = false;
					IsYesVisible = true;
					IsNoVisible = true;
					IsCancelVisible = false;
					break;
				case MessageStyle.YesNoCancel:
					IsOKVisible = false;
					IsYesVisible = true;
					IsNoVisible = true;
					IsCancelVisible = true;
					break;
				default:
					throw new ArgumentOutOfRangeException("style");
			}
		}

	}
}