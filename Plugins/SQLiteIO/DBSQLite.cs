using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using HyperKore.Common;
using HyperKore.Utilities;

namespace HyperPlugin
{
	public class DBSQLite : IDBReader, IDBWriter
	{
		private const string BuildCmd =
			"CREATE TABLE IF NOT EXISTS 'Card'('id' TEXT NOT NULL,'name' TEXT NOT NULL,'set' TEXT NOT NULL,'setcode' TEXT NOT NULL," +
			"'cost' TEXT,'cmc' TEXT,'type' TEXT NOT NULL,'pow' TEXT,'tgh' TEXT,'loyalty' TEXT,'text' TEXT," +
			"'flavor' TEXT,'artist' TEXT,'rarity' TEXT NOT NULL,'number' TEXT NOT NULL,PRIMARY KEY('id'));" +
			"CREATE TABLE IF NOT EXISTS 'Set'('SetName' TEXT NOT NULL,'SetCode' TEXT,'LastUpdate' TEXT,'Local' INTEGER,PRIMARY KEY('SetName'));" +
			"CREATE TABLE IF NOT EXISTS 'File'('id' TEXT NOT NULL,'data' BLOB,'length' INTEGER,PRIMARY KEY('id'))";

		//private const string ConnString = "data source=DATA.db;password=5AEB55D5-F169-4EB2-A768-B20EBD20151E";

		/// <summary>
		///     Single instance of SQLiteConnection
		/// </summary>
		private readonly SQLiteConnection _conn = new SQLiteConnection();

		private readonly SQLiteConnectionStringBuilder _connectionStringBuilder = new SQLiteConnectionStringBuilder();

		private Language _language;

		public DBSQLite()
		{
			Create();
		}

		#region IDBReader Members

		public string DBType
		{
			get { return "SQLite"; }
		}

		/// <summary>
		///     Language of the database
		/// </summary>
		public Language Language
		{
			get { return _language; }
			set
			{
				//If language is changed, switch database
				if (_language != value)
				{
					_language = value;
					Create();
				}
			}
		}

		public string Description
		{
			get { return "Store Card/Image/Set database"; }
		}

		public string Name
		{
			get { return "DBSQLite"; }
		}

		public IEnumerable<Card> LoadCards()
		{
			//use lock(this) for singleton class
			lock (this)
			{
				using (DataContext datacontext = new DataContext(_conn))
				{
					Table<Card> tab = datacontext.GetTable<Card>();
					return tab.ToList();
				}
			}
		}

		public byte[] LoadFile(string id, ICompressor compressor)
		{
			lock (this)
			{
				using (DataContext datacontext = new DataContext(_conn))
				{
					Table<Bin> tab = datacontext.GetTable<Bin>();
					Bin[] datas = tab.Where(i => i.ID == id).ToArray();

					if (datas.Count() != 1)
					{
						return null;
					}

					return compressor == null ? datas[0].Data : compressor.Decompress(datas[0].Data, datas[0].Length);
				}
			}
		}

		public IEnumerable<Set> LoadSets()
		{
			lock (this)
			{
				using (DataContext datacontext = new DataContext(_conn))
				{
					Table<Set> tab = datacontext.GetTable<Set>();
					return tab.ToList();
				}
			}
		}

		#endregion

		#region IDBWriter Members

		public bool Delete(Card card)
		{
			lock (this)
			{
				using (DataContext datacontext = new DataContext(_conn))
				{
					Table<Card> tab = datacontext.GetTable<Card>();
					IQueryable<Card> que = tab.Where(c => c.ID == card.ID);

					if (!que.Any())
					{
						return false;
					}

					tab.DeleteAllOnSubmit(que);
					datacontext.SubmitChanges();

					return true;
				}
			}
		}

		public bool Delete(string id)
		{
			lock (this)
			{
				using (DataContext datacontext = new DataContext(_conn))
				{
					Table<Bin> tab = datacontext.GetTable<Bin>();
					IQueryable<Bin> que = tab.Where(i => i.ID == id);

					if (!que.Any())
					{
						return false;
					}

					tab.DeleteAllOnSubmit(que);
					datacontext.SubmitChanges();

					return true;
				}
			}
		}

		public bool Insert(Card card)
		{
			lock (this)
			{
				using (DataContext datacontext = new DataContext(_conn))
				{
					Table<Card> tab = datacontext.GetTable<Card>();
					IQueryable<Card> que = tab.Where(c => c.ID == card.ID);

					if (que.Any())
					{
						foreach (Card item in que)
						{
							item.CopyFrom(card);
						}
					}
					else
					{
						tab.InsertOnSubmit(card);
					}
					datacontext.SubmitChanges();
					return true;
				}
			}
		}

		public void Insert(IEnumerable<Card> cards)
		{
			lock (this)
			{
				using (DataContext datacontext = new DataContext(_conn))
				{
					Table<Card> tab = datacontext.GetTable<Card>();

					foreach (Card card in cards)
					{
						IQueryable<Card> que = tab.Where(c => c.ID == card.ID);
						if (que.Any())
						{
							foreach (Card item in que)
							{
								item.CopyFrom(card);
							}
						}
						else
						{
							tab.InsertOnSubmit(card);
						}
					}

					datacontext.SubmitChanges();
				}
			}
		}

		public void Insert(IEnumerable<Set> sets)
		{
			lock (this)
			{
				using (DataContext datacontext = new DataContext(_conn))
				{
					Table<Set> tab = datacontext.GetTable<Set>();
					foreach (Set set in sets)
					{
						if (!tab.Any(s => s.SetName == set.SetName))
						{
							tab.InsertOnSubmit(set);
						}
					}
					datacontext.SubmitChanges();
				}
			}
		}

		public bool Insert(string id, byte[] data, ICompressor compressor)
		{
			lock (this)
			{
				using (DataContext datacontext = new DataContext(_conn))
				{
					Table<Bin> tab = datacontext.GetTable<Bin>();
					byte[] compdata = compressor == null ? data : compressor.Compress(data);

					if (tab.Any(i => i.ID == id))
					{
						return Update(id, data, compressor);
					}

					tab.InsertOnSubmit(new Bin(compdata, id, data.Length));
					datacontext.SubmitChanges();
					return true;
				}
			}
		}

		public bool Update(Card card)
		{
			lock (this)
			{
				using (DataContext datacontext = new DataContext(_conn))
				{
					Table<Card> tab = datacontext.GetTable<Card>();
					IQueryable<Card> que = tab.Where(c => c.ID == card.ID);

					if (!que.Any())
					{
						return false;
					}

					foreach (Card item in que)
					{
						item.CopyFrom(card);
					}
					datacontext.SubmitChanges();
					return true;
				}
			}
		}

		public bool Update(string id, byte[] data, ICompressor compressor)
		{
			lock (this)
			{
				using (DataContext datacontext = new DataContext(_conn))
				{
					Table<Bin> tab = datacontext.GetTable<Bin>();
					byte[] compdata = compressor == null ? data : compressor.Compress(data);
					IQueryable<Bin> que = tab.Where(i => i.ID == id);

					if (!que.Any())
					{
						return false;
					}

					foreach (Bin item in que)
					{
						item.Data = compdata;
						item.Length = data.Length;
					}
					datacontext.SubmitChanges();
					return true;
				}
			}
		}

		public bool Update(Set set)
		{
			lock (this)
			{
				using (DataContext datacontext = new DataContext(_conn))
				{
					Table<Set> tab = datacontext.GetTable<Set>();
					IQueryable<Set> que = tab.Where(c => c.SetName == set.SetName);

					if (!que.Any())
					{
						return false;
					}

					foreach (Set item in que)
					{
						item.SetCode = set.SetCode;
						item.LastUpdate = set.LastUpdate;
						item.Local = set.Local;
					}
					datacontext.SubmitChanges();
					return true;
				}
			}
		}

		#endregion

		public void Reset(IEnumerable<Set> sets)
		{
			lock (this)
			{
				using (DataContext datacontext = new DataContext(_conn))
				{
					Table<Set> tab = datacontext.GetTable<Set>();
					if (tab.Any())
					{
						tab.DeleteAllOnSubmit(tab);
						tab.InsertAllOnSubmit(sets);
						datacontext.SubmitChanges();
					}
				}
			}
		}

		private void Create()
		{
			lock (this)
			{
				if (!Directory.Exists("Data"))
				{
					Directory.CreateDirectory("Data");
				}
				_connectionStringBuilder.DataSource = string.Format("Data/datasource.{0}.db", Language.GetLangCode());
				_connectionStringBuilder.Password = "5AEB55D5-F169-4EB2-A768-B20EBD20151E";
				_conn.ConnectionString = _connectionStringBuilder.ConnectionString;

				using (DataContext datacontext = new DataContext(_conn))
				{
					datacontext.ExecuteCommand(BuildCmd);
				}
			}
		}
	}
}