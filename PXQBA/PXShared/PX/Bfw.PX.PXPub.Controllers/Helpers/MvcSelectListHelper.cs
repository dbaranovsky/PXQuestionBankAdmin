using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Bfw.PX.PXPub.Controllers.Helpers
{
	public static class MvcSelectListHelper
	{

		/// <summary>
		/// Returns an IEnumerable<SelectListItem> by using the specified items for data value field.
		/// </summary>
		/// <param name="enumerable">The items.</param>
		/// <param name="value">The data value field.</param>
		public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> value)
		{
			return enumerable.ToSelectList(value, value, null);
		}

		/// <summary>
		/// Returns an IEnumerable<SelectListItem> ; by using the specified items for data value field and a selected value.
		/// </summary>
		/// <param name="enumerable">The items.</param>
		/// <param name="value">The data value field.</param>
		/// <param name="selectedValue">The selected value.</param>
		public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> value, object selectedValue)
		{
			return enumerable.ToSelectList(value, value, selectedValue);
		}

		/// <summary>
		/// Returns an IEnumerable<SelectListItem>  by using the specified items for data value field and the data text field.
		/// </summary>
		/// <param name="enumerable">The items.</param>
		/// <param name="value">The data value field.</param>
		/// <param name="text">The data text field.</param>
		public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> value, Func<T, string> text)
		{
			//System.Web.Mvc.MultiSelectList
			return enumerable.ToSelectList(value, text, null);
		}

		/// <summary>
		/// Returns an IEnumerable<SelectListItem>  by using the specified items for data value field, the data text field, and a selected value.
		/// </summary>
		/// <param name="enumerable">The items.</param>
		/// <param name="value">The data value field.</param>
		/// <param name="text">The data text field.</param>
		/// <param name="selectedValue">The selected value.</param>
		public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> value, Func<T, string> text, object selectedValue)
		{
			var listItems=enumerable.Select(f => new SelectListItem()
			                              	{
			                              		Value = value(f),
			                              		Text = text(f),
			                              		Selected = value(f).Equals(selectedValue)
			                              	});

			//SelectList list = new SelectList(listItems, "Value", "Text");
			return listItems;	
		}


		static public IEnumerable<SelectListItem> Add(this IEnumerable<SelectListItem> list, string text, string value = "", ListPosition listPosition = ListPosition.First)
		{
			if (string.IsNullOrEmpty(value))
			{
				value = text;
			}
			var listItems = list.ToList();
			var lp = (int)listPosition;
			switch (lp)
			{
				case -1:
					lp = list.Count();
					break;
				case -2:
					lp = list.Count() / 2;
					break;
				case -3:
					var random = new Random();
					lp = random.Next(0, list.Count());
					break;
			}
			listItems.Insert(lp, new SelectListItem { Value = value, Text = text });
			//list = new SelectList(listItems, "Value", "Text");
			return listItems;
		}

		public enum ListPosition
		{
			First = 0,
			Last = -1,
			Middle = -2,
			Random = -3
		}

	}
}
