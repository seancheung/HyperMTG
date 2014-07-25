using HyperKore.Common;
using HyperKore.Net;
using HyperKore.Utilities;
using HyperKore.Xception;
using System;
using System.Text.RegularExpressions;

namespace HyperKore.Web
{
	internal class ParseDetail : ICardParse
	{
		private string BuildURL(string id)
		{
			if (id.Contains("|"))
			{
				id = id.Remove(id.IndexOf("|"));
			}

			return string.Format("http://gatherer.wizards.com/Pages/Card/Details.aspx?printed=true&multiverseid={0}", id);
		}

		public void Parse(Card card, LANGUAGE lang)
		{
			string webdata = Request.Instance.GetWebData(BuildURL(card.ID));

			if (!webdata.Contains("Card Name:"))
			{
				card = null;
				return;
			}

			bool issplit = false;
			bool isdoubleface = false;

			#region Card Var

			if (webdata.Contains("Other Variations"))
			{
				try
				{
					int num = webdata.IndexOf("id=", webdata.IndexOf("<a href", webdata.IndexOf("Other Variations"))) + 4;
					for (int j = 1; j < 5; j++)
					{
						int num2 = webdata.IndexOf("class=", num) - 2;
						string text2 = webdata.Substring(num, num2 - num);
						if (text2.Length > 10)
						{
							break;
						}
						card.Var += string.Format("({0}:{1})", j, text2);
						num = webdata.IndexOf("id=", num2) + 4;
					}
				}
				catch (Exception ex)
				{
					throw new ParsingXception("Parsing Error happended when parsing card Var", ex);
				}
			}
			else
			{
				card.Var = string.Empty;
			}

			#endregion Card Var

			#region MultiID?

			if (webdata.IndexOf("Card Name:") != webdata.LastIndexOf("Card Name:"))
			{
				if (webdata.IndexOf("Converted Mana Cost:") != webdata.LastIndexOf("Converted Mana Cost:"))
				{
					issplit = true;
				}
				else
				{
					isdoubleface = true;
				}

				if (isdoubleface)
				{
					int num25 = webdata.IndexOf("multiverseid=", webdata.LastIndexOf("<img src=\"../../Handlers/Image.ashx?multiverseid=")) + 13;
					int num26 = webdata.IndexOf("&amp", num25);
					string text4 = webdata.Substring(num25, num26 - num25).Trim();
					if (text4 == card.ID)
					{
						card = null;
						return;
					}
				}
			}

			#endregion MultiID?

			#region Card Name

			try
			{
				int num3 = webdata.IndexOf("<div class=\"value\">", webdata.IndexOf("Card Name:")) + 20;
				int num4 = webdata.IndexOf("</div>", num3);
				card.Name = webdata.Substring(num3, num4 - num3).Trim();
			}
			catch (Exception ex)
			{
				throw new ParsingXception("Parsing Error happended when parsing card Name", ex);
			}

			#endregion Card Name

			#region Card Cost

			if (webdata.IndexOf("Mana Cost:") > 0)
			{
				try
				{
					int num27 = webdata.IndexOf("<div class=\"value\">", webdata.LastIndexOf("Card Name:")) + 20;
					int num28 = webdata.IndexOf("</div>", num27);
					int num45 = webdata.IndexOf("<div class=\"value\">", num28) + 20;
					int num46 = webdata.IndexOf("</div>", num45);
					string text6 = webdata.Substring(num45, num46 - num45).Trim();
					int num34;
					for (int k = text6.IndexOf("<img src="); k > 0; k = text6.IndexOf("<img src=", num34))
					{
						int num33 = text6.IndexOf("alt=", k) + 4;
						num34 = text6.IndexOf("align=", num33);
						string str = text6.Substring(num33, num34 - num33).Replace("\"", string.Empty).Trim();
						text6 = text6.Insert(k, String.Format("{{{0}}}", str));
					}
					//Mark
					string cost = string.Empty;
					while (text6.Contains("<") && text6.Contains(">"))
					{
						int num11 = text6.IndexOf("<");
						int num12 = text6.IndexOf(">");
						int start = text6.IndexOf("alt=") + 5;
						int end = text6.IndexOf(@"""", start);
						var mana = text6.Substring(start, end - start).Trim();
						mana = mana.Replace("Variable Colorless", "X");
						cost += mana.Length > 1 ? String.Format("({0})", mana) : String.Format("{0}", mana);
						text6 = text6.Remove(num11, num12 - num11 + 1).Trim();
					}
					card.Cost = cost;
				}
				catch (Exception ex)
				{
					throw new ParsingXception("Parsing Error happended when parsing card Cost", ex);
				}
			}
			else
			{
				card.Cost += string.Empty;
			}

			#endregion Card Cost

			#region Card CMC

			if (webdata.IndexOf("Converted Mana Cost:") > 0)
			{
				try
				{
					int num5 = webdata.IndexOf("<div class=\"value\">", webdata.IndexOf("Converted Mana Cost:")) + 20;
					int num6 = webdata.IndexOf("<br />", num5);
					card.CMC = webdata.Substring(num5, num6 - num5).Trim();
				}
				catch (Exception ex)
				{
					throw new ParsingXception("Parsing Error happended when parsing card CMC", ex);
				}
			}
			else
			{
				card.CMC = "0";
			}

			#endregion Card CMC

			#region Card Type

			try
			{
				int num7 = webdata.IndexOf("<div class=\"value\">", webdata.IndexOf("Types:")) + 20;
				int num8 = webdata.IndexOf("</div>", num7);
				card.Type = webdata.Substring(num7, num8 - num7).Trim();
			}
			catch (Exception ex)
			{
				throw new ParsingXception("Parsing Error happended when parsing card Type", ex);
			}

			#endregion Card Type

			#region Card Text

			if (webdata.IndexOf("Card Text:") > 0)
			{
				try
				{
					int num47 = webdata.IndexOf("</div>", webdata.IndexOf("Card Text:")) + 6;
					int num48 = webdata.IndexOf("</div></div>", num47);
					card.Text = webdata.Substring(num47, num48 - num47).Trim();
					int num34;
					for (int k = card.Text.IndexOf("<img src="); k > 0; k = card.Text.IndexOf("<img src=", num34))
					{
						int num33 = card.Text.IndexOf("alt=", k) + 4;
						num34 = card.Text.IndexOf("align=", num33);
						string str = card.Text.Substring(num33, num34 - num33).Replace("\"", string.Empty).Trim();
						card.Text = card.Text.Insert(k, String.Format("{{{0}}}", str));
					}
					card.Text = card.Text.RemoveHtmlTag();
				}
				catch (Exception ex)
				{
					throw new ParsingXception("Parsing Error happended when parsing card Text", ex);
				}
			}
			else
			{
				card.Text = string.Empty;
			}

			#endregion Text

			#region Card Flavor

			if (webdata.IndexOf("Flavor Text:") > 0)
			{
				try
				{
					int num9 = webdata.IndexOf("</div>", webdata.IndexOf("Flavor Text:")) + 6;
					int num10 = webdata.IndexOf("</div></div>", num9);
					card.Flavor = webdata.Substring(num9, num10 - num9).Trim();
					card.Flavor = card.Flavor.RemoveHtmlTag();
				}
				catch (Exception ex)
				{
					throw new ParsingXception("Parsing Error happended when parsing card Flavor", ex);
				}
			}
			else
			{
				card.Flavor = string.Empty;
			}

			#endregion Card Flavor

			#region Card PT

			if (webdata.IndexOf("P/T:") > 0)
			{
				try
				{
					int num13 = webdata.IndexOf("<div class=\"value\">", webdata.IndexOf("P/T:")) + 20;
					int num14 = webdata.IndexOf("</div>", num13);
					string text3 = webdata.Substring(num13, num14 - num13);
					card.Pow = text3.Substring(0, text3.IndexOf("/")).Trim();
					card.Tgh = text3.Substring(text3.IndexOf("/") + 1).Trim();
				}
				catch (Exception ex)
				{
					throw new ParsingXception("Parsing Error happended when parsing card PT", ex);
				}
			}
			else
			{
				card.Pow = string.Empty;
				card.Tgh = string.Empty;
			}

			#endregion Card PT

			#region Card Loyalty

			if (webdata.IndexOf("Loyalty:") > 0)
			{
				try
				{
					int num15 = webdata.IndexOf("<div class=\"value\">", webdata.IndexOf("Loyalty:")) + 20;
					int num16 = webdata.IndexOf("</div>", num15);
					card.Loyalty = webdata.Substring(num15, num16 - num15).Trim();
				}
				catch (Exception ex)
				{
					throw new ParsingXception("Parsing Error happended when parsing card Loyalty", ex);
				}
			}
			else
			{
				card.Loyalty = string.Empty;
			}

			#endregion Card Loyalty

			#region Card Rarity

			try
			{
				int num17 = webdata.IndexOf("<div class=\"value\">", webdata.IndexOf("Rarity:")) + 20;
				int num18 = webdata.IndexOf("</div>", num17);
				card.Rarity = webdata.Substring(num17, num18 - num17).Trim();
				while (card.Rarity.Contains("<"))
				{
					int num11 = card.Rarity.IndexOf("<");
					int num12 = card.Rarity.IndexOf(">");
					card.Rarity = card.Rarity.Remove(num11, num12 - num11 + 1).Trim();
				}
			}
			catch (Exception ex)
			{
				throw new ParsingXception("Parsing Error happended when parsing card Rarity", ex);
			}

			#endregion Card Rarity

			#region Card Number

			try
			{
				int num19 = webdata.IndexOf("<div class=\"value\">", webdata.IndexOf("Card Number:")) + 20;
				int num20 = webdata.IndexOf("</div>", num19);
				card.Number = webdata.Substring(num19, num20 - num19).Trim();
			}
			catch (Exception ex)
			{
				throw new ParsingXception("Parsing Error happended when parsing card Number", ex);
			}

			#endregion Card Number

			#region Card Artist

			try
			{
				int num21 = webdata.IndexOf("</div>", webdata.IndexOf("Artist:")) + 6;
				int num22 = webdata.IndexOf("</div>", num21);
				card.Artist = webdata.Substring(num21, num22 - num21).Trim();
				while (card.Artist.Contains("<"))
				{
					int num11 = card.Artist.IndexOf("<");
					int num12 = card.Artist.IndexOf(">");
					card.Artist = card.Artist.Remove(num11, num12 - num11 + 1).Trim();
				}
			}
			catch (Exception ex)
			{
				throw new ParsingXception("Parsing Error happended when parsing card Artist", ex);
			}

			#endregion Card Artist

			#region Card Rulings

			if (webdata.IndexOf(">Rulings<") > 0)
			{
				try
				{
					int num23 = webdata.IndexOf("<table", webdata.IndexOf(">Rulings<"));
					int num24 = webdata.IndexOf("</table>", num23);
					card.Rulings = webdata.Substring(num23, num24 - num23).Trim();
					card.Rulings = card.Rulings.RemoveHtmlTag();
				}
				catch (Exception ex)
				{
					throw new ParsingXception("Parsing Error happended when parsing card Rulings", ex);
				}
			}
			else
			{
				card.Rulings = string.Empty;
			}

			#endregion Card Rulings

			#region Card Rating

			if (webdata.IndexOf("class=\"textRatingValue\">") > 0)
			{
				try
				{
					int num49 = webdata.IndexOf("class=\"textRatingValue\">") + 24;
					int num50 = webdata.IndexOf("</span>", num49);
					card.Rating = webdata.Substring(num49, num50 - num49).Trim();
				}
				catch (Exception ex)
				{
					throw new ParsingXception("Parsing Error happended when parsing card Rating", ex);
				}
			}
			else
			{
				card.Rating = string.Empty;
			}

			#endregion Card Rating

			if (isdoubleface)
			{
				#region bID

				try
				{
					int num25 = webdata.IndexOf("multiverseid=", webdata.LastIndexOf("<img src=\"../../Handlers/Image.ashx?multiverseid=")) + 13;
					int num26 = webdata.IndexOf("&amp", num25);
					string bid = webdata.Substring(num25, num26 - num25).Trim();
					card.ID = String.Format("{0}|{1}", card.ID, bid);
				}
				catch (Exception ex)
				{
					throw new ParsingXception("Parsing Error happended when parsing card bID", ex);
				}

				#endregion bID

				#region bName

				try
				{
					int num27 = webdata.IndexOf("<div class=\"value\">", webdata.LastIndexOf("Card Name:")) + 20;
					int num28 = webdata.IndexOf("</div>", num27);
					card.Name = String.Format("{0}|{1}", card.Name, webdata.Substring(num27, num28 - num27).Trim());
				}
				catch (Exception ex)
				{
					throw new ParsingXception("Parsing Error happended when parsing card bName", ex);
				}

				#endregion bName

				#region bType

				try
				{
					int num29 = webdata.IndexOf("<div class=\"value\">", webdata.LastIndexOf("Types:")) + 20;
					int num30 = webdata.IndexOf("</div>", num29);
					card.Type = String.Format("{0}|{1}", card.Type, webdata.Substring(num29, num30 - num29).Trim());
				}
				catch (Exception ex)
				{
					throw new ParsingXception("Parsing Error happended when parsing card bType", ex);
				}

				#endregion bType

				#region bText

				if (webdata.IndexOf("Card Text:") > 0 && webdata.IndexOf("Card Text:") != webdata.LastIndexOf("Card Text:"))
				{
					try
					{
						int num31 = webdata.IndexOf("</div>", webdata.LastIndexOf("Card Text:")) + 6;
						int num32 = webdata.IndexOf("</div></div>", num31);
						card.Text = String.Format("{0}|{1}", card.Text, webdata.Substring(num31, num32 - num31).Trim());
						int num34;
						for (int k = card.Text.IndexOf("<img src="); k > 0; k = card.Text.IndexOf("<img src=", num34))
						{
							int num33 = card.Text.IndexOf("alt=", k) + 4;
							num34 = card.Text.IndexOf("align=", num33);
							string str = card.Text.Substring(num33, num34 - num33).Replace("\"", string.Empty).Trim();
							card.Text = card.Text.Insert(k, String.Format("{{{0}}}", str));
						}

						card.Text = card.Text.RemoveHtmlTag();
					}
					catch (Exception ex)
					{
						throw new ParsingXception("Parsing Error happended when parsing card bText", ex);
					}
				}
				else
				{
					card.Text += "|";
				}

				#endregion bText

				#region bFlavor

				if (webdata.IndexOf("Flavor Text:") > 0 && webdata.IndexOf("Flavor Text:") != webdata.LastIndexOf("Flavor Text:"))
				{
					try
					{
						int num35 = webdata.IndexOf("</div>", webdata.LastIndexOf("Flavor Text:")) + 6;
						int num36 = webdata.IndexOf("</div></div>", num35);
						card.Flavor = String.Format("{0}|{1}", card.Flavor, webdata.Substring(num35, num36 - num35).Trim());

						card.Flavor = card.Flavor.RemoveHtmlTag();
					}
					catch (Exception ex)
					{
						throw new ParsingXception("Parsing Error happended when parsing card bFlavor", ex);
					}
				}
				else
				{
					card.Flavor += "|";
				}

				#endregion bFlavor

				#region bPT

				if (webdata.IndexOf("P/T:") > 0 && webdata.IndexOf("P/T:") != webdata.LastIndexOf("P/T:"))
				{
					try
					{
						int num37 = webdata.IndexOf("<div class=\"value\">", webdata.LastIndexOf("P/T:")) + 20;
						int num38 = webdata.IndexOf("</div>", num37);
						string text5 = webdata.Substring(num37, num38 - num37);
						card.Pow = String.Format("{0}|{1}", card.Pow, text5.Substring(0, text5.IndexOf("/")).Trim());
						card.Tgh = String.Format("{0}|{1}", card.Tgh, text5.Substring(text5.IndexOf("/") + 1).Trim());
					}
					catch (Exception ex)
					{
						throw new ParsingXception("Parsing Error happended when parsing card bPT", ex);
					}
				}
				else
				{
					card.Pow += "|";
					card.Tgh += "|";
				}

				#endregion bPT

				#region bRarity

				try
				{
					int num39 = webdata.IndexOf("<div class=\"value\">", webdata.LastIndexOf("Rarity:")) + 20;
					int num40 = webdata.IndexOf("</div>", num39);
					card.Rarity = String.Format("{0}|{1}", card.Rarity, webdata.Substring(num39, num40 - num39).Trim());
					while (card.Rarity.Contains("<") && card.Rarity.Contains(">"))
					{
						int num11 = card.Rarity.IndexOf("<");
						int num12 = card.Rarity.IndexOf(">");
						card.Rarity = card.Rarity.Remove(num11, num12 - num11 + 1).Trim();
					}
				}
				catch (Exception ex)
				{
					throw new ParsingXception("Parsing Error happended when parsing card bRarity", ex);
				}

				#endregion bRarity

				#region bNumber

				if (webdata.Contains("Card Number:") && webdata.IndexOf("Card Number:") != webdata.LastIndexOf("Card Number:"))
				{
					try
					{
						int num41 = webdata.IndexOf("<div class=\"value\">", webdata.LastIndexOf("Card Number:")) + 20;
						int num42 = webdata.IndexOf("</div>", num41);
						card.Number = String.Format("{0}|{1}", card.Number, webdata.Substring(num41, num42 - num41).Trim());
					}
					catch (Exception ex)
					{
						throw new ParsingXception("Parsing Error happended when parsing card bNumber", ex);
					}
				}
				else
				{
					card.Number += "|";
				}

				#endregion bNumber

				#region bColor

				if (webdata.LastIndexOf("Color Indicator:") > 0)
				{
					try
					{
						int num43 = webdata.IndexOf("<div class=\"value\">", webdata.IndexOf("Color Indicator:")) + 20;
						int num44 = webdata.IndexOf("</div>", num43);
						card.ColorBside = webdata.Substring(num43, num44 - num43).Trim();
					}
					catch (Exception ex)
					{
						throw new ParsingXception("Parsing Error happended when parsing card bColor", ex);
					}
				}
				else
				{
					card.ColorBside = null;
				}

				#endregion bColor
			}
			else if (issplit)
			{
				#region sType

				try
				{
					int num27 = webdata.IndexOf("<div class=\"value\">", webdata.LastIndexOf("Card Name:")) + 20;
					int num28 = webdata.IndexOf("</div>", num27);
					int num29 = webdata.IndexOf("<div class=\"value\">", webdata.LastIndexOf("Types:")) + 20;
					int num30 = webdata.IndexOf("</div>", num29);
					card.Type = String.Format("{0}|{1}", card.Type, webdata.Substring(num29, num30 - num29).Trim());
				}
				catch (Exception ex)
				{
					throw new ParsingXception("Parsing Error happended when parsing card sType", ex);
				}

				#endregion sType

				#region sCMC

				if (webdata.IndexOf("Converted Mana Cost:") > 0 && webdata.IndexOf("Converted Mana Cost:") != webdata.LastIndexOf("Converted Mana Cost:"))
				{
					try
					{
						int num5 = webdata.IndexOf("<div class=\"value\">", webdata.LastIndexOf("Converted Mana Cost:")) + 20;
						int num6 = webdata.IndexOf("<br />", num5);
						card.CMC = String.Format("{0}|{1}", card.CMC, webdata.Substring(num5, num6 - num5).Trim());
					}
					catch (Exception ex)
					{
						throw new ParsingXception("Parsing Error happended when parsing card sCMC", ex);
					}
				}
				else
				{
					card.CMC += "|";
				}

				#endregion sCMC

				#region sCost

				if (webdata.IndexOf("Mana Cost:") > 0 && webdata.IndexOf("Mana Cost:") != webdata.LastIndexOf("Mana Cost:"))
				{
					try
					{
						int num27 = webdata.IndexOf("<div class=\"value\">", webdata.LastIndexOf("Card Name:")) + 20;
						int num28 = webdata.IndexOf("</div>", num27);
						int num45 = webdata.IndexOf("<div class=\"value\">", num28) + 20;
						int num46 = webdata.IndexOf("</div>", num45);
						string text6 = webdata.Substring(num45, num46 - num45).Trim();
						int num34;
						for (int k = text6.IndexOf("<img src="); k > 0; k = text6.IndexOf("<img src=", num34))
						{
							int num33 = text6.IndexOf("alt=", k) + 4;
							num34 = text6.IndexOf("align=", num33);
							string str = text6.Substring(num33, num34 - num33).Replace("\"", string.Empty).Trim();
							text6 = text6.Insert(k, String.Format("{{{0}}}", str));
						}
						//Mark
						string cost = string.Empty;
						while (text6.Contains("<") && text6.Contains(">"))
						{
							int num11 = text6.IndexOf("<");
							int num12 = text6.IndexOf(">");
							int start = text6.IndexOf("alt=") + 5;
							int end = text6.IndexOf(@"""", start);
							var mana = text6.Substring(start, end - start).Trim();
							mana = mana.Replace("Variable Colorless", "X");
							cost += mana.Length > 1 ? String.Format("({0})", mana) : String.Format("{0}", mana);
							text6 = text6.Remove(num11, num12 - num11 + 1).Trim();
						}
						card.Cost = String.Format("{0}|{1}", card.Cost, cost);
					}
					catch (Exception ex)
					{
						throw new ParsingXception("Parsing Error happended when parsing card sCost", ex);
					}
				}
				else
				{
					card.Cost += "|";
				}

				#endregion sCost

				#region sRarity

				try
				{
					int num39 = webdata.IndexOf("<div class=\"value\">", webdata.LastIndexOf("Rarity:")) + 20;
					int num40 = webdata.IndexOf("</div>", num39);
					card.Rarity = String.Format("{0}|{1}", card.Rarity, webdata.Substring(num39, num40 - num39).Trim());
					while (card.Rarity.Contains("<") && card.Rarity.Contains(">"))
					{
						int num11 = card.Rarity.IndexOf("<");
						int num12 = card.Rarity.IndexOf(">");
						card.Rarity = card.Rarity.Remove(num11, num12 - num11 + 1).Trim();
					}
				}
				catch (Exception ex)
				{
					throw new ParsingXception("Parsing Error happended when parsing card sRarity", ex);
				}

				#endregion sRarity

				#region sNumber

				if (webdata.Contains("Card Number:") && webdata.IndexOf("Card Number:") != webdata.LastIndexOf("Card Number:"))
				{
					try
					{
						int num41 = webdata.IndexOf("<div class=\"value\">", webdata.LastIndexOf("Card Number:")) + 20;
						int num42 = webdata.IndexOf("</div>", num41);
						card.Number = String.Format("{0}|{1}", card.Number, webdata.Substring(num41, num42 - num41).Trim());
					}
					catch (Exception ex)
					{
						throw new ParsingXception("Parsing Error happended when parsing card sNumber", ex);
					}
				}
				else
				{
					card.Number += "|";
				}

				#endregion sNumber

				if (webdata.LastIndexOf("Color Indicator:") > 0)
				{
					int num43 = webdata.IndexOf("<div class=\"value\">", webdata.IndexOf("Color Indicator:")) + 20;
					int num44 = webdata.IndexOf("</div>", num43);
					card.ColorBside = webdata.Substring(num43, num44 - num43).Trim();
				}
				else
				{
					card.ColorBside = null;
				}
			}
		}
	}
}