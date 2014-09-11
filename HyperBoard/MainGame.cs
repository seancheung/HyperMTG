using System;
using HyperKore.Game;

namespace HyperBoard
{
	public class MainGame : BaseGame
	{
		private double delta;

		#region Overrides of Game

		protected override void Initialize()
		{
			
		}

		protected override void Start()
		{
			
		}

		protected override void Update()
		{
			delta++;
			if (delta >= FPS)
			{
				Console.WriteLine(DateTime.Now);
				delta = 0;
			}
		}

		protected override void Render()
		{
			
		}

		protected override void End()
		{
			
		}

		protected override void Finalize()
		{
			
		}

		#endregion

		public void Close()
		{
			GameOver = true;
		}
	}
}