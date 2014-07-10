namespace HyperMTG.ViewModel
{
	using HyperKore.Common;
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public class FilterViewModel
	{
		/// <summary>
		/// Initializes a new instance of the FilterViewModel class.
		/// </summary>
		public FilterViewModel()
		{
			Cost = 0;
			Power = 0;
			Rating = 0f;
			Toughness = 0;
			Types = new Dictionary<TYPE, bool>();
			GetFromEnum<TYPE>(Types);
		}

		public int Cost { get; set; }

		public int Power { get; set; }

		public float Rating { get; set; }

		public int Toughness { get; set; }

		public Dictionary<TYPE, bool> Types { get; set; }

		public void GetFromEnum<T>(IDictionary dict)
		{
			if (dict == null) return;

			foreach (var item in Enum.GetNames(typeof(T)))
			{
				dict.Add(Enum.Parse(typeof(T), item), true);
			}
		}
	}
}