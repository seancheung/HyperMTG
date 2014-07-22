using HyperKore.Common;
using HyperKore.Net;
using HyperKore.Utilities;
using HyperKore.Xception;
using System;

namespace HyperKore.Web
{
	internal class ParsezDetail : ICardParse
	{
		public void Parse(Card card, LANGUAGE lang)
		{
			if (!string.IsNullOrWhiteSpace(card.zID))
			{
				GetzDetail(card);
			}
			else
			{
				card.zName = string.Empty;
				card.zText = string.Empty;
				card.zType = string.Empty;
				card.zFlavor = string.Empty;
			}
		}

		private string BuildURL(string zid)
		{
			if (zid.Contains("|"))
			{
				zid = zid.Remove(zid.IndexOf("|"));
			}

			return string.Format("http://gatherer.wizards.com/Pages/Card/Details.aspx?printed=true&multiverseid={0}", zid);
		}

		private void GetzDetail(Card card)
		{
			string webdata;
			try
			{
				webdata = Request.Instance.GetWebData(BuildURL(card.zID));
			}
			catch
			{
				throw;
			}

			if (!webdata.Contains("../../Handlers/Image.ashx?multiverseid="))
			{
				card.zName = string.Empty;
				card.zType = string.Empty;
				card.zText = string.Empty;
				card.zFlavor = string.Empty;
				return;
			}

			#region zVar

			if (webdata.Contains("Other Variations"))
			{
				try
				{
					int num = webdata.IndexOf("id=", webdata.IndexOf("<a href", webdata.IndexOf("Other Variations"))) + 4;
					for (int i = 1; i < 5; i++)
					{
						int num2 = webdata.IndexOf("class=", num) - 2;
						card.Var += string.Format("({0}:{1})", i, webdata.Substring(num, num2 - num));
						num = webdata.IndexOf("id=", num2) + 4;
					}
				}
				catch (Exception ex)
				{
					throw new ParsingXception("Parsing Error happended when parsing card zVar:", ex);
				}
			}

			#endregion zVar

			#region zName

			try
			{
				int num3 = webdata.IndexOf("<div class=\"value\">", webdata.IndexOf("Card Name:")) + 20;
				int num4 = webdata.IndexOf("</div>", num3);
				card.zName = webdata.Substring(num3, num4 - num3).Trim();
			}
			catch (Exception ex)
			{
				throw new ParsingXception("Parsing Error happended when parsing card zName:", ex);
			}

			#endregion zName

			#region zType

			try
			{
				int num5 = webdata.IndexOf("<div class=\"value\">", webdata.IndexOf("Types:")) + 20;
				int num6 = webdata.IndexOf("</div>", num5);
				card.zType = webdata.Substring(num5, num6 - num5).Replace("-", string.Empty).Replace(" ", string.Empty).Trim();
			}
			catch (Exception ex)
			{
				throw new ParsingXception("Parsing Error happended when parsing card zType:", ex);
			}

			#endregion zType

			#region zFlavor

			if (webdata.IndexOf("Flavor Text:") > 0)
			{
				try
				{
					int num7 = webdata.IndexOf("</div>", webdata.IndexOf("Flavor Text:")) + 6;
					int num8 = webdata.IndexOf("</div></div>", num7);
					card.zFlavor = webdata.Substring(num7, num8 - num7);
					while (card.zFlavor.Contains("<"))
					{
						int num9 = card.zFlavor.IndexOf("<");
						int num10 = card.zFlavor.IndexOf(">");
						card.zFlavor = card.zFlavor.Remove(num9, num10 - num9 + 1).Trim();
					}
				}
				catch (Exception ex)
				{
					throw new ParsingXception("Parsing Error happended when parsing card zFlavor:", ex);
				}
			}
			else
			{
				card.zFlavor = string.Empty;
			}

			#endregion zFlavor

			#region zText

			if (webdata.IndexOf("Card Text:") > 0)
			{
				try
				{
					int num11 = webdata.IndexOf("</div>", webdata.IndexOf("Card Text:")) + 6;
					int num12 = webdata.IndexOf("</div></div>", num11);
					card.zText = webdata.Substring(num11, num12 - num11);
					int num14;
					for (int j = card.zText.IndexOf("<img src="); j > 0; j = card.zText.IndexOf("<img src=", num14))
					{
						int num13 = card.zText.IndexOf("alt=", j) + 4;
						num14 = card.zText.IndexOf("align=", num13);
						string str = card.zText.Substring(num13, num14 - num13).Replace("\"", string.Empty).Trim();
						card.zText = card.zText.Insert(j, String.Format("{{{0}}}", str));
					}
					while (card.zText.Contains("<"))
					{
						int num9 = card.zText.IndexOf("<");
						int num10 = card.zText.IndexOf(">");
						if (num10 < num9)
							card.zText = card.zText.Remove(num9, 1);
						else
							card.zText = card.zText.Remove(num9, num10 - num9 + 1).Trim();
					}
				}
				catch (Exception ex)
				{
					throw new ParsingXception("Parsing Error happended when parsing card zText:", ex);
				}
			}
			else
			{
				card.zText = string.Empty;
			}

			#endregion zText

			if (card.IsDoubleFaced())
			{
				#region zbName

				try
				{
					int num15 = webdata.IndexOf("<div class=\"value\">", webdata.LastIndexOf("Card Name:")) + 20;
					int num16 = webdata.IndexOf("</div>", num15);
					card.zName = String.Format("{0}|{1}", card.zName, webdata.Substring(num15, num16 - num15).Trim());
				}
				catch (Exception ex)
				{
					throw new ParsingXception("Parsing Error happended when parsing card zbName:", ex);
				}

				#endregion zbName

				#region zbType

				try
				{
					int num17 = webdata.IndexOf("<div class=\"value\">", webdata.LastIndexOf("Types:")) + 20;
					int num18 = webdata.IndexOf("</div>", num17);
					card.zType = String.Format("{0}|{1}", card.zType, webdata.Substring(num17, num18 - num17).Replace("-", string.Empty).Replace(" ", string.Empty).Trim());
				}
				catch (Exception ex)
				{
					throw new ParsingXception("Parsing Error happended when parsing card zbType:", ex);
				}

				#endregion zbType

				#region zbFlavor

				if (webdata.IndexOf("Flavor Text:") > 0 && webdata.IndexOf("Flavor Text:") != webdata.LastIndexOf("Flavor Text:"))
				{
					try
					{
						int num19 = webdata.IndexOf("</div>", webdata.LastIndexOf("Flavor Text:")) + 6;
						int num20 = webdata.IndexOf("</div></div>", num19);
						card.zFlavor = String.Format("{0}|{1}", card.zFlavor, webdata.Substring(num19, num20 - num19));
						while (card.zFlavor.Contains("<"))
						{
							int num9 = card.zFlavor.IndexOf("<");
							int num10 = card.zFlavor.IndexOf(">");
							card.zFlavor = card.zFlavor.Remove(num9, num10 - num9 + 1).Trim();
						}
					}
					catch (Exception ex)
					{
						throw new ParsingXception("Parsing Error happended when parsing card zbFlavor:", ex);
					}
				}
				else
				{
					card.zFlavor += "|";
				}

				#endregion zbFlavor

				#region zbText

				if (webdata.IndexOf("Card Text:") > 0 && webdata.IndexOf("Card Text:") != webdata.LastIndexOf("Card Text:"))
				{
					try
					{
						int num21 = webdata.IndexOf("multiverseid=", webdata.LastIndexOf("<img src=\"../../Handlers/Image.ashx?multiverseid=")) + 13;
						int num22 = webdata.IndexOf("&amp", num21);
						card.zID = String.Format("{0}|{1}", card.zID, webdata.Substring(num21, num22 - num21).Trim());
						int num23 = webdata.IndexOf("</div>", webdata.LastIndexOf("Card Text:")) + 6;
						int num24 = webdata.IndexOf("</div></div>", num23);
						card.zText = String.Format("{0}|{1}", card.zText, webdata.Substring(num23, num24 - num23));
						int num14;
						for (int j = card.zText.IndexOf("<img src="); j > 0; j = card.zText.IndexOf("<img src=", num14))
						{
							int num13 = card.zText.IndexOf("alt=", j) + 4;
							num14 = card.zText.IndexOf("align=", num13);
							string str = card.zText.Substring(num13, num14 - num13).Replace("\"", string.Empty).Trim();
							card.zText = card.zText.Insert(j, String.Format("{{{0}}}", str));
						}
						while (card.zText.Contains("<"))
						{
							int num9 = card.zText.IndexOf("<");
							int num10 = card.zText.IndexOf(">");
							card.zText = card.zText.Remove(num9, num10 - num9 + 1).Trim();
						}
					}
					catch (Exception ex)
					{
						throw new ParsingXception("Parsing Error happended when parsing card zbText:", ex);
					}
				}
				else
				{
					card.zText += "|";
				}

				#endregion zbText
			}
			else if (card.IsSplit())
			{
				try
				{
					int num17 = webdata.IndexOf("<div class=\"value\">", webdata.LastIndexOf("Types:")) + 20;
					int num18 = webdata.IndexOf("</div>", num17);
					card.zType = String.Format("{0}|{1}", card.zType, webdata.Substring(num17, num18 - num17).Replace("-", string.Empty).Replace(" ", string.Empty).Trim());
				}
				catch (Exception ex)
				{
					throw new ParsingXception("Parsing Error happended when parsing card zsType:", ex);
				}
			}
		}
	}
}