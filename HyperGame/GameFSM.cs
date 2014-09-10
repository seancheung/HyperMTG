using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyperKore.Game
{
	public interface IState<T> where T : IEntity
	{
		void Enter(T entity);
		void Execute(T entity);
		void Exit(T entity);
		bool OnMessage(T entity, Message msg);
	}

	public class FSM<T> where T : IEntity
	{
		private readonly T owner;

		public FSM(T owner)
		{
			this.owner = owner;
		}

		public IState<T> CurrentState { get; private set; }
		public IState<T> PreviousState { get; private set; }
		public IState<T> GloabalState { get; private set; }

		public bool IsInState(IState<T> state)
		{
			return state.GetType() == CurrentState.GetType();
		}

		public void SetCurrentState(IState<T> state)
		{
			CurrentState = state;
		}

		public void SetGlobalState(IState<T> state)
		{
			GloabalState = state;
		}

		public void SetPreviousState(IState<T> state)
		{
			PreviousState = state;
		}

		public void OnUpdate()
		{
			if (GloabalState != null)
			{
				GloabalState.Execute(owner);
			}

			if (CurrentState != null)
			{
				CurrentState.Execute(owner);
			}
		}

		public void ChangeState(IState<T> state)
		{
			if (state != null)
			{
				PreviousState = CurrentState;
				if (CurrentState != null)
				{
					CurrentState.Exit(owner);
				}
				CurrentState = state;
				CurrentState.Enter(owner);
			}
		}

		public void RevertState()
		{
			ChangeState(PreviousState);
		}

		public bool HandleMessage(Message msg)
		{
			if (CurrentState != null && CurrentState.OnMessage(owner, msg))
			{
				return true;
			}
			if (GloabalState != null && GloabalState.OnMessage(owner, msg))
			{
				return true;
			}
			return false;
		}
	}
}
