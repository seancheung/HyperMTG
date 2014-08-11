using System;
using System.Data.Linq.Mapping;

namespace HyperKore.Common
{
	[Table(Name = "Set")]
	public class Set
	{
		/// <summary>
		/// Set full name as 'Theros(THS)'
		/// </summary>
		public string FullName
		{
			get
			{
				return String.Format("{0}({1})", SetName, SetCode);
			}
		}

		/// <summary>
		/// Last update time
		/// </summary>
		[Column(Name = "LastUpdate")]
		public DateTime LastUpdate
		{
			get;
			set;
		}

		/// <summary>
		/// Whether this set is stored locally
		/// </summary>
		[Column(Name = "Local")]
		public bool Local
		{
			get;
			set;
		}

		/// <summary>
		/// Set code
		/// </summary>
		[Column(Name = "SetCode")]
		public string SetCode
		{
			get;
			set;
		}

		/// <summary>
		/// Set name
		/// </summary>
		[Column(Name = "SetName", IsPrimaryKey = true)]
		public string SetName
		{
			get;
			set;
		}

		#region Overrides of Object

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		public override string ToString()
		{
			return FullName;
		}

		#endregion
	}
}