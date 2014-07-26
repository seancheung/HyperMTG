using System;
using System.Data.Linq.Mapping;

namespace HyperKore.Common
{
	[Table(Name = "File")]
	public class Bin
	{
		/// <summary>
		/// Initializes a new instance of the Bin class.
		/// </summary>
		public Bin()
		{
			Data = null;
			ID = String.Empty;
			Length = 0;
		}

		/// <summary>
		/// Initializes a new instance of the Bin class.
		/// </summary>
		public Bin(byte[] data, string iD, int length)
		{
			Data = data;
			ID = iD;
			Length = length;
		}

		[Column(Name = "data")]
		public byte[] Data
		{
			get;
			set;
		}

		[Column(Name = "id", IsPrimaryKey = true)]
		public string ID
		{
			get;
			set;
		}

		[Column(Name = "length")]
		public int Length
		{
			get;
			set;
		}
	}
}