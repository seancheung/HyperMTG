using System.Runtime.InteropServices;
using System.Security;

namespace HyperKore.Game
{
	public abstract class BaseGame
	{
		private readonly double previousTime;
		private readonly PreciseTimer timer = new PreciseTimer();
		protected bool GameOver;
		private double fps = 60;
		private double time;

		public double FPS
		{
			get { return fps; }
		}

		/// <summary>
		/// Milliseconds passed since last frame
		/// </summary>
		protected double DeltaTime
		{
			get { return timer.GetElapsedTime()*1000; }
		}

		public void Run()
		{
			Initialize();
			Start();
			while (!GameOver)
			{
				time += DeltaTime;
				if (time >= 1000/fps)
				{
					Update();
					Render();
					time = 0;
				}
			}
			End();
			Finalize();
		}

		protected abstract void Initialize();

		protected abstract void Start();

		protected abstract void Update();

		protected abstract void Render();

		protected abstract void End();

		protected abstract void Finalize();

		protected void SetFPS(double newFPS)
		{
			fps = newFPS;
		}

		#region Nested type: PreciseTimer

		private class PreciseTimer
		{
			private readonly long _ticksPerSecond;
			private long _previousElapsedTime;

			public PreciseTimer()
			{
				QueryPerformanceFrequency(ref _ticksPerSecond);
				GetElapsedTime(); // Get rid of first rubbish result
			}

			[SuppressUnmanagedCodeSecurity]
			[DllImport("kernel32")]
			private static extern bool QueryPerformanceFrequency(ref long
				PerformanceFrequency);

			[SuppressUnmanagedCodeSecurity]
			[DllImport("kernel32")]
			private static extern bool QueryPerformanceCounter(ref long
				PerformanceCount);

			public double GetElapsedTime()
			{
				long time = 0;
				QueryPerformanceCounter(ref time);
				double elapsedTime = (time - _previousElapsedTime)/
				                     (double) _ticksPerSecond;
				_previousElapsedTime = time;
				return elapsedTime;
			}
		}

		#endregion
	}
}