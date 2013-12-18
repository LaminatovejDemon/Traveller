using UnityEngine;
using System.Collections;

public class PopupManager : MonoBehaviour {

	private static PopupManager mInstance = null;
	public static PopupManager GetInstance()
	{
		if ( mInstance == null )
		{
			GameObject instanceObject_ = GameObject.Find("#PopupManager");
			if ( instanceObject_ != null )
				mInstance = instanceObject_.transform.GetComponent<PopupManager>();
			else
				mInstance =  new GameObject("#PopupManager").AddComponent<PopupManager>();
		}
		return mInstance;
	}

	public PartPopup _PartPopupWindow;

	public void CreatePartPopup(GameObject partResource, PartManager.Pattern pattern, string title = "")
	{
		PartPopup newPopup_ = ((GameObject)GameObject.Instantiate(_PartPopupWindow.gameObject)).GetComponent<PartPopup>();

		newPopup_.Initialize();

		newPopup_.SetPartObject(partResource);

		string text_ = pattern.mName;

		newPopup_.SetTitleTextObject(text_);

		text_ = "";

		text_ += PartManager.GetInstance().GetEnergyLabel(pattern);
		text_ += PartManager.GetInstance().GetEvadeLabel(pattern);
		text_ += PartManager.GetInstance().GetHPLabel(pattern);
		text_ += PartManager.GetInstance().GetDamageLabel(pattern);

		newPopup_.SetContentTextObject(text_);
	}



}
