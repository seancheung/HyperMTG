using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HyperService.Game
{
	public interface IPermanent
	{
		Dictionary<CardStatus, bool> Status { get; }
	}

	public interface IArtifact : IPermanent
	{
	}

	public interface IEquipment : IArtifact
	{
	}

	public interface ICreature : IPermanent
	{
		[DataMember]
		int Power { get; }

		[DataMember]
		int Toughness { get; }
	}

	public interface IEnchantment : IPermanent
	{
	}

	public interface IAura : IEnchantment
	{
		[DataMember]
		Guid EnchantedTarget { get; }
	}

	public interface ICurse : IAura
	{
	}

	public interface ILand : IPermanent
	{
	}

	public interface IPlaneswalker : IPermanent
	{
		[DataMember]
		int Loyalty { get; }
	}

	public interface ILegendary
	{
	}

	public interface IToken
	{
	}

	public interface ISpell
	{
	}
}