using TocCategory = Bfw.PX.Biz.DataContracts.TocCategory;

namespace Bfw.PXWebAPI.Mappers
{
	/// <summary>
	/// TocCategoryMapper
	/// </summary>
	public static class TocCategoryMapper
	{
		/// <summary>
		/// Convert to a toc category.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns></returns>
		public static TocCategory ToTocCategory(this PX.PXPub.Models.TocCategory model)
		{
			var biz = new TocCategory()
			{
				Id = model.Id,
				Text = model.Text,
				ItemParentId = model.ItemParentId,
				Sequence = model.Sequence
			};

			return biz;
		}
		/// <summary>
		/// Convert to a Toc category from a biz TocCategory.
		/// </summary>
		/// <param name="biz">The biz.</param>
		/// <param name="active">The active.</param>
		/// <returns></returns>
		public static PX.PXPub.Models.TocCategory ToTocCategory(this TocCategory biz, string active)
		{
			if (active == null)
				active = string.Empty;

			var model = new PX.PXPub.Models.TocCategory()
			{
				Id = biz.Id,
				Text = biz.Text,
				Active = biz.Id.ToLowerInvariant() == active.ToLowerInvariant(),
				ItemParentId = biz.ItemParentId,
				Sequence = biz.Sequence
			};

			return model;
		}

		/// <summary>
		/// Convert to a Toc category from a Biz TocCategory.
		/// </summary>
		/// <param name="biz">The biz.</param>
		/// <returns></returns>
		public static PX.PXPub.Models.TocCategory ToTocCategory(this TocCategory biz)
		{
			return ToTocCategory(biz, string.Empty);
		}

	}
}
