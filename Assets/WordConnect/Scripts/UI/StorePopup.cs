using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BBG;

#if BBG_IAP
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
#endif

namespace WordConnect
{
	public class StorePopup : Popup
	{
		#region Member Variables

		private bool areAdsRemoved;

		#endregion

		#region Public Methods

		public override void OnShowing(object[] inData)
		{
		/*	#if BBG_MT_IAP
			BBG.MobileTools.IAPManager.Instance.OnProductPurchased += OnProductPurchases;
			#endif*/
		}

		public override void OnHiding()
		{
			base.OnHiding();

			/*#if BBG_MT_IAP
			BBG.MobileTools.IAPManager.Instance.OnProductPurchased -= OnProductPurchases;
			#endif*/
		}

		#endregion

		#region Private Methods

		private void OnProductPurchases(string productId)
		{
			Hide(false);

			PopupManager.Instance.Show("product_purchased");
		}

		#endregion
	}
}
