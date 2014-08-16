namespace HyperKore.Game.Event
{
	public class CommonDelegate
	{
		#region Delegates

		public delegate void OnAttack(GameCard gameCard);

		public delegate void OnBlock(Player player);

		public delegate void OnCombatBegin(GameCard gameCard);

		public delegate void OnCombatEnd(GameCard gameCard);

		public delegate void OnCounter(GameCard gameCard);

		public delegate void OnDealDamage(GameCard gameCard);

		public delegate void OnDiscard(GameCard gameCard);

		public delegate void OnDraw(GameCard gameCard);

		public delegate void OnEndStepBegin(GameCard gameCard);

		public delegate void OnEnterBattlefield(GameCard gameCard);

		public delegate void OnEnterGraveyard(GameCard gameCard);

		public delegate void OnExile(GameCard gameCard);

		public delegate void OnLeaveBattlefield(GameCard gameCard);

		public delegate void OnMuligan(Player player);

		public delegate void OnPlay(GameCard gameCard);

		public delegate void OnReceiveDamage(GameCard gameCard);

		public delegate void OnTarget(GameCard gameCard);

		public delegate void OnUntap(GameCard gameCard);

		public delegate void OnUpkeepBegin(GameCard gameCard);

		#endregion
	}
}