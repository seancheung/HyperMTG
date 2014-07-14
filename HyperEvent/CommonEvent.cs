namespace HyperKore.Game.Event
{
	public class CommonEvent
	{
		public delegate void OnMuligan(Player player);

		public delegate void OnUpkeepBegin(Player player);

		public delegate void OnUpkeepBegin(GameCard gameCard);

		public delegate void OnUntap(Player player);

		public delegate void OnUntap(GameCard gameCard);

		public delegate void OnDraw(Player player);

		public delegate void OnDraw(GameCard gameCard);

		public delegate void OnPlay(Player player);

		public delegate void OnPlay(GameCard gameCard);

		public delegate void OnEnterBattlefield(GameCard gameCard);

		public delegate void OnLeaveBattlefield(GameCard gameCard);

		public delegate void OnTarget(Player player);

		public delegate void OnTarget(GameCard gameCard);

		public delegate void OnCounter(Player player);

		public delegate void OnCounter(GameCard gameCard);

		public delegate void OnCombatBegin(Player player);

		public delegate void OnCombatBegin(GameCard gameCard);

		public delegate void OnCombatEnd(Player player);

		public delegate void OnCombatEnd(GameCard gameCard);

		public delegate void OnAttack(Player player);

		public delegate void OnAttack(GameCard gameCard);

		public delegate void OnBlock(GameCard gameCard);

		public delegate void OnBlock(Player player);

		public delegate void OnDealDamage(GameCard gameCard);

		public delegate void OnReceiveDamage(Player player);

		public delegate void OnReceiveDamage(GameCard gameCard);

		public delegate void OnEnterGraveyard(GameCard gameCard);

		public delegate void OnExile(GameCard gameCard);

		public delegate void OnDiscard(Player player);

		public delegate void OnDiscard(GameCard gameCard);

		public delegate void OnEndStepBegin(Player player);

		public delegate void OnEndStepBegin(GameCard gameCard);
	}
}