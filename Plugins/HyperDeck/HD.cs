using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HyperKore.Common;
using LitJson;
using IOException = HyperKore.Exception.IOException;

namespace HyperPlugin
{
	internal class HD : IDeckReader, IDeckWriter
	{
		#region Implementation of IPlugin

		public string Description
		{
			get { return "Import/Export HyperMTG format deck"; }
		}

		public string Name
		{
			get { return "HD"; }
		}

		#endregion

		#region Implementation of IDeckReader

		/// <summary>
		///     File extension
		/// </summary>
		public string FileExt
		{
			get { return "xdeck"; }
		}

		/// <summary>
		///     Type of the deck
		/// </summary>
		public string DeckType
		{
			get { return "HyperDeck"; }
		}

		/// <summary>
		///     Load deck from stream
		/// </summary>
		/// <param name="input"></param>
		/// <param name="database"></param>
		/// <returns></returns>
		public Deck Read(Stream input, IEnumerable<Card> database)
		{
			Deck deck = new Deck();
			HyperDeck xdeck = Open(input);
			if (xdeck.MainBoard.Count + xdeck.SideBoard.Count <= 0) return deck;

			foreach (Card card in xdeck.MainBoard.SelectMany(idExp => Convert(idExp, database)))
			{
				deck.MainBoard.Add(card);
			}

			foreach (Card card in xdeck.SideBoard.SelectMany(idExp => Convert(idExp, database)))
			{
				deck.SideBoard.Add(card);
			}

			deck.Name = xdeck.Name;
			deck.Comment = xdeck.Comment;
			deck.Format = xdeck.Format;
			deck.Mode = xdeck.Mode;

			return deck;
		}

		/// <summary>
		///     Save deck to stream
		/// </summary>
		/// <param name="deck"></param>
		/// <param name="output"></param>
		/// <returns></returns>
		public bool Write(Deck deck, Stream output)
		{
			try
			{
				HyperDeck xdeck = Convert(deck);
				Export(xdeck, output);
				return true;
			}
			catch
			{
				return false;
			}
		}

		#endregion

		private void Export(HyperDeck deck, Stream stream)
		{
			try
			{
				StreamWriter sw = new StreamWriter(stream);
				string data = JsonMapper.ToJson(deck);
				sw.Write(data);
				sw.Flush();
			}
			catch (Exception ex)
			{
				throw new IOException(ex);
			}
		}

		private HyperDeck Open(Stream input)
		{
			StreamReader sr = new StreamReader(input);
			sr.BaseStream.Seek(0L, SeekOrigin.Begin);
			string json = sr.ReadToEnd();
			return JsonMapper.ToObject<HyperDeck>(json);
		}

		private IEnumerable<Card> Convert(string IDExp, IEnumerable<Card> database)
		{
			string[] split = IDExp.Split(new[] {'@'});
			Card card = database.FirstOrDefault(c => c.ID == split[0]);
			int qnt = Int32.Parse(split[1]);

			for (int i = 0; i < qnt; i++)
			{
				if (card != null) yield return card;
			}
		}

		private HyperDeck Convert(Deck deck)
		{
			IEnumerable<string> main = deck.MainBoard.ToLookup(c => c.ID).Select(g => string.Format("{0}@{1}", g.Key, g.Count()));
			IEnumerable<string> side = deck.SideBoard.ToLookup(c => c.ID).Select(g => string.Format("{0}@{1}", g.Key, g.Count()));

			return new HyperDeck(deck.Name, deck.Comment, deck.Format, deck.Mode, main, side);
		}

		internal class HyperDeck
		{
			/// <summary>
			///     Initializes a new instance of the HyperDeck class with parameters.
			/// </summary>
			/// <param name="name"></param>
			/// <param name="comment"></param>
			/// <param name="format"></param>
			/// <param name="mode"></param>
			/// <param name="mainBoard"></param>
			/// <param name="sideBoard"></param>
			public HyperDeck(string name, string comment, Format format, Mode mode, IEnumerable<string> mainBoard,
				IEnumerable<string> sideBoard)
			{
				Name = name;
				Comment = comment;
				Format = format;
				Mode = mode;
				MainBoard = new List<string>(mainBoard);
				SideBoard = new List<string>(sideBoard);
			}

			/// <summary>
			///     Initializes a new instance of the HyperDeck class.
			/// </summary>
			public HyperDeck()
			{
			}

			public string Name { get; set; }
			public string Comment { get; set; }
			public Format Format { get; set; }
			public Mode Mode { get; set; }

			public List<string> MainBoard { get; set; }
			public List<string> SideBoard { get; set; }
		}
	}
}