﻿using UnityEngine;
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

	public PopupPart _PartPopupWindow;
	public PopupMessage _MessagePopupWindow;
	public PopupConfirm _ConfirmPopupWindow;

	public void DisplayRewardPopup(bool ownerIsAlive, bool enemyIsAlive)
	{
		if ( !ownerIsAlive && !enemyIsAlive )
		{
			CreateMessagePopup("DRAW GAME", "Everybody is dead, Dave...\n\n"+ FleetManager.GetShip().GetTierData().GetStatsLabel()); 
		}
		else if ( ownerIsAlive )
		{
			CreateMessagePopup("YOU WON", "Dr. Hildegarde Lanstrom would know.\n\n"+FleetManager.GetShip().GetTierData().GetStatsLabel());
		}
		else 
		{
			CreateMessagePopup("YOU LOST YOUR SHIP", "...\n\n"+FleetManager.GetShip().GetTierData().GetStatsLabel());
		}
	}
	
	public void CreateConfirmPopup(string title, string content, ButtonHandler handler)
	{
		PopupConfirm newPopup_ = ((GameObject)GameObject.Instantiate(_ConfirmPopupWindow.gameObject)).GetComponent<PopupConfirm>();
		
		newPopup_.Initialize();
		
		newPopup_.SetTitleTextObject(title);
		newPopup_.SetContentTextObject(content);
		newPopup_.SetHandler(handler);
	}

	public void CreateMessagePopup(string title, string content)
	{
		PopupMessage newPopup_ = ((GameObject)GameObject.Instantiate(_MessagePopupWindow.gameObject)).GetComponent<PopupMessage>();

		newPopup_.Initialize();

		newPopup_.SetTitleTextObject(title);
		newPopup_.SetContentTextObject(content);
	}


	public void CreatePartPopup(GameObject partResource, PartManager.Pattern pattern, string header = "")
	{
		PopupPart newPopup_ = ((GameObject)GameObject.Instantiate(_PartPopupWindow.gameObject)).GetComponent<PopupPart>();

		newPopup_.Initialize();

		newPopup_.SetPartObject(partResource);
		newPopup_.SetHeaderTextObject(header);

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
