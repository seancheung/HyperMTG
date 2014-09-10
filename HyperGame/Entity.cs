using System;
using System.Collections.Generic;

namespace HyperKore.Game
{
	public interface IEntity
	{
		Guid ID { get; }

		void HandleMessage(Message msg);

		void OnUpdate();
	}

	public class EntityManager
	{
		public static readonly EntityManager Instance = new EntityManager();
		private readonly Dictionary<Guid, IEntity> entities;

		private EntityManager()
		{
			entities = new Dictionary<Guid, IEntity>();
		}

		public void RegisterEntity(IEntity entity)
		{
			if (entity != null && !entities.ContainsKey(entity.ID))
			{
				entities.Add(entity.ID, entity);
			}
		}

		public IEntity GetEntity(Guid id)
		{
			return entities.ContainsKey(id) ? entities[id] : null;
		}

		public void RemoveEntity(Guid id)
		{
			if (entities.ContainsKey(id))
			{
				entities.Remove(id);
			}
		}
	}
}