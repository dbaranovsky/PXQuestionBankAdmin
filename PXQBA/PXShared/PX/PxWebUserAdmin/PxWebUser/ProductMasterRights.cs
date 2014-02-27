namespace PxWebUser
{
	public class ProductMasterAccess
	{

		public bool None
		{
			get { return adminRights.Is(enumProductMasterAdminRights.None); }
			internal set
			{
				if (value) adminRights = adminRights.Remove(adminRights);
			}
		}

		public bool AllowAddProduct
		{
			get { return adminRights.Has(enumProductMasterAdminRights.AllowAddProduct); }
			internal set
			{
				adminRights = value ? adminRights.Add(enumProductMasterAdminRights.AllowAddProduct) : adminRights.Remove(enumProductMasterAdminRights.AllowAddProduct);
			}
		}

		public bool AllowEditProduct
		{
			get { return adminRights.Has(enumProductMasterAdminRights.AllowEditProduct); }
			internal set
			{
				adminRights = value ? adminRights.Add(enumProductMasterAdminRights.AllowEditProduct) : adminRights.Remove(enumProductMasterAdminRights.AllowEditProduct);
			}
		}

		public bool ShowProductHistory
		{
			get { return adminRights.Has(enumProductMasterAdminRights.ShowProductHistory); }
			internal set
			{
				adminRights = value ? adminRights.Add(enumProductMasterAdminRights.ShowProductHistory) : adminRights.Remove(enumProductMasterAdminRights.ShowProductHistory);
			}
		}

		public bool ShowProductManager
		{
			get { return adminRights.Has(enumProductMasterAdminRights.ShowProductManager); }
			internal set
			{
				adminRights = value ? adminRights.Add(enumProductMasterAdminRights.ShowProductManager) : adminRights.Remove(enumProductMasterAdminRights.ShowProductManager);
			}
		}



		internal enumProductMasterAdminRights adminRights;

		internal ProductMasterAccess(enumProductMasterAdminRights rights)
		{
			adminRights = rights;
		}

	}
}
