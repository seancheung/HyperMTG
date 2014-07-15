using System;
using System.Collections.Generic;

namespace HyperKore.Game.Event
{
	public class CommonAction
	{
		public bool Cast(ISpell card)
		{
			throw new NotImplementedException();
		}

		public bool Destroy(GameCard card)
		{
			throw new NotImplementedException();
		}

		public bool Tap(GameCard card)
		{
			throw new NotImplementedException();
		}

		public bool Draw(Player target, int amount = 1)
		{
			throw new NotImplementedException();
		}

		public bool Discard(Player target, int amount = 1)
		{
			throw new NotImplementedException();
		}

		public bool Discard(Player target, GameCard card)
		{
			throw new NotImplementedException();
		}

		public bool UnTap(GameCard card)
		{
			throw new NotImplementedException();
		}

		public bool PutToHand(GameCard card)
		{
			throw new NotImplementedException();
		}

		public bool Exile(GameCard card)
		{
			throw new NotImplementedException();
		}

		public bool PutToLib(GameCard card, int order = 0)
		{
			throw new NotImplementedException();
		}

		public bool Attack(ICreature attacker, Player target)
		{
			throw new NotImplementedException();
		}

		public bool Attack(ICreature attacker, IPlaneswalker target)
		{
			throw new NotImplementedException();
		}

		public bool Block(ICreature blocker, ICreature attacker)
		{
			throw new NotImplementedException();
		}

		public bool Block(IEnumerable<ICreature> blockers, ICreature attacker)
		{
			throw new NotImplementedException();
		}

		public bool DealDamage(GameCard source, Player target, int amount)
		{
			throw new NotImplementedException();
		}

		public bool GainLife(Player target, int amount)
		{
			throw new NotImplementedException();
		}

		public bool LoseLife(Player target, int amount)
		{
			throw new NotImplementedException();
		}
	}
}