using System;
using System.Collections.Specialized;
using System.Xml.Linq;
namespace HTS
{
	public class CCorrect
	{
		internal string _correct = "";
		internal string _format = "";
		internal string _tolerance = "";
		internal string _type = "";
		internal string _rule = "any";
		internal string _points = "1";
		internal CHTSProblem _problem;
		public CCorrect(CHTSProblem problem, XElement xEl, string atype, string apoints, string atolerance, string arule)
		{
			this._problem = problem;
			this._format = CUtils.getXElementAttribureValue(xEl, "format", "");
			this._type = atype;
			this._tolerance = CUtils.getXElementAttribureValue(xEl, "tolerance", atolerance);
			this._tolerance = this._tolerance.Replace(" ", "");
			this._rule = CUtils.getXElementAttribureValue(xEl, "answerrule", arule);
			this._correct = CUtils.getXElementAttribureValue(xEl, "correct", "");
			this._correct = this._correct.Trim();
			this._points = CUtils.getXElementAttribureValue(xEl, "points", apoints);
		}
		internal double getTolerance(double pForValue)
		{
			double num = 0.02;
			string text = this._tolerance;
			if (this._tolerance.EndsWith("%"))
			{
				text = text.Replace("%", "");
				num = 2.0;
			}
			try
			{
				num = Convert.ToDouble(text) + 1E-10;
			}
			catch (Exception)
			{
				try
				{
					num = Convert.ToDouble(text.Replace(".", ","));
				}
				catch
				{
				}
			}
			if (this._tolerance.EndsWith("%"))
			{
				num = pForValue * (num / 100.0);
			}
			return num + 1E-10;
		}
		internal string getToleranceForPopup()
		{
			return this._tolerance;
		}
		public bool checkAnswer(string uAnswer)
		{
			bool result = false;
			string text = this._correct;
			uAnswer = uAnswer.Trim();
			if (text == uAnswer)
			{
				return true;
			}
			string type;
			if ((type = this._type) != null)
			{
				bool result2;
				if (!(type == "numeric"))
				{
					if (type == "math")
					{
						string a = CUtils.evalMath(text, uAnswer, this._rule);
						return a == "Correct";
					}
					if (!(type == "text"))
					{
						return result;
					}
					uAnswer = uAnswer.ToUpper();
					text = text.ToUpper();
					uAnswer = CUtils.mergeBlanks(uAnswer);
					uAnswer = uAnswer.Replace(";", ",");
					text = CUtils.mergeBlanks(text);
					uAnswer = uAnswer.Replace(" ,", ",");
					uAnswer = uAnswer.Replace(", ", ",");
					if (text == uAnswer)
					{
						return true;
					}
					if (text.IndexOf(';') != -1)
					{
						text = text.Replace(" ;", ";");
						text = text.Replace("; ", ";");
						string[] array = text.Split(new char[]
						{
							';'
						});
						StringCollection stringCollection = new StringCollection();
						stringCollection.AddRange(uAnswer.Split(new char[]
						{
							','
						}));
						try
						{
							if (array.Length == stringCollection.Count)
							{
								int num = -1;
								for (int i = 0; i < array.Length; i++)
								{
									for (int j = 0; j < stringCollection.Count; j++)
									{
										if (array[i] == stringCollection[j])
										{
											num = j;
											break;
										}
									}
									if (num == -1)
									{
										result2 = false;
										return result2;
									}
									stringCollection.RemoveAt(num);
								}
								result2 = true;
								return result2;
							}
						}
						catch
						{
						}
						return false;
					}
					if (text.IndexOf(',') != -1)
					{
						text = text.Replace(" ,", ",");
						text = text.Replace(", ", ",");
						string[] array2 = text.Split(new char[]
						{
							','
						});
						StringCollection stringCollection2 = new StringCollection();
						stringCollection2.AddRange(uAnswer.Split(new char[]
						{
							','
						}));
						try
						{
							if (array2.Length == stringCollection2.Count)
							{
								for (int k = 0; k < array2.Length; k++)
								{
									if (array2[k] != stringCollection2[k])
									{
										result2 = false;
										return result2;
									}
								}
								result2 = true;
								return result2;
							}
						}
						catch
						{
						}
						return false;
					}
					try
					{
						result2 = (text == uAnswer);
						return result2;
					}
					catch
					{
					}
					return false;
				}
				else
				{
					uAnswer = uAnswer.ToUpper();
					uAnswer = CUtils.removeBlanks(uAnswer);
					uAnswer = uAnswer.Replace(";", ",");
					text = CUtils.removeBlanks(text);
					CUtils.detectBrackets(uAnswer);
					CUtils.detectBrackets(text);
					if (text == uAnswer)
					{
						return true;
					}
					if (text.IndexOf(';') != -1)
					{
						uAnswer = CUtils.removeBrackets(uAnswer);
						text = CUtils.removeBrackets(text);
						string[] array3 = text.Split(new char[]
						{
							';'
						});
						StringCollection stringCollection3 = new StringCollection();
						stringCollection3.AddRange(uAnswer.Split(new char[]
						{
							','
						}));
						try
						{
							if (array3.Length == stringCollection3.Count)
							{
								int num = -1;
								for (int l = 0; l < array3.Length; l++)
								{
									double num2;
									if (this._problem != null)
									{
										object value = this._problem.doCalculate(array3[l]);
										num2 = Convert.ToDouble(value);
										num2 = CUtils.formatNumber(num2, this._format);
									}
									else
									{
										num2 = Convert.ToDouble(text);
									}
									double tolerance = this.getTolerance(num2);
									for (int m = 0; m < stringCollection3.Count; m++)
									{
										double num3 = Convert.ToDouble(stringCollection3[m]);
										double num4 = num2 - num3;
										num4 = Math.Abs(Math.Round(num4, 12));
										tolerance = this.getTolerance(num4);
										if (num4 <= tolerance)
										{
											num = m;
											break;
										}
									}
									if (num == -1)
									{
										result2 = false;
										return result2;
									}
									stringCollection3.RemoveAt(num);
								}
								result2 = true;
								return result2;
							}
						}
						catch
						{
						}
						return false;
					}
					if (text.IndexOf(',') != -1)
					{
						uAnswer = CUtils.removeBrackets(uAnswer);
						text = CUtils.removeBrackets(text);
						string[] array4 = text.Split(new char[]
						{
							','
						});
						StringCollection stringCollection4 = new StringCollection();
						stringCollection4.AddRange(uAnswer.Split(new char[]
						{
							','
						}));
						try
						{
							if (array4.Length == stringCollection4.Count)
							{
								for (int n = 0; n < array4.Length; n++)
								{
									double num5;
									if (this._problem != null)
									{
										object value2 = this._problem.doCalculate(array4[n]);
										num5 = Convert.ToDouble(value2);
										num5 = CUtils.formatNumber(num5, this._format);
									}
									else
									{
										num5 = Convert.ToDouble(text);
									}
									double tolerance = this.getTolerance(num5);
									double num6 = Convert.ToDouble(stringCollection4[n]);
									double num7 = num5 - num6;
									num7 = Math.Abs(Math.Round(num7, 12));
									if (num7 > tolerance)
									{
										result2 = false;
										return result2;
									}
								}
								result2 = true;
								return result2;
							}
						}
						catch
						{
						}
						return false;
					}
					try
					{
						double num8 = Convert.ToDouble(uAnswer);
						double num9;
						if (this._problem != null)
						{
							object value3 = this._problem.doCalculate(text);
							num9 = Convert.ToDouble(value3);
							num9 = CUtils.formatNumber(num9, this._format);
						}
						else
						{
							num9 = Convert.ToDouble(text);
						}
						double tolerance = this.getTolerance(num9);
						double num10 = num9 - num8;
						num10 = Math.Abs(Math.Round(num10, 12));
						result2 = (num10 <= tolerance);
						return result2;
					}
					catch (Exception)
					{
					}
					return false;
				}
				return result2;
			}
			return result;
		}
	}
}
