using UnityEngine;
using System.Collections;

public class HangarManager : ButtonHandler 
{
	public FrameSlider _ButtonContainerSlider;
	public FrameSlider _OpenButtonContainerSlider;
	
	public FrameSlider _HangarContainer;
	private static HangarManager mInstance = null;
	public static HangarManager GetInstance()
	{
		if ( mInstance == null )
		{
			GameObject instanceObject_ = GameObject.Find("#HangarManager");
			if ( instanceObject_ != null )
				mInstance = instanceObject_.transform.GetComponent<HangarManager>();
			else
				mInstance =  new GameObject("#HangarManager").AddComponent<HangarManager>();
		}
		return mInstance;
	}
	
	bool mInitlialzed = false;
//	GameObject mDoneButton = null;
//	GameObject mHangarButton = null;
//	GameObject mClearButton = null;
	
	public void InformShipValidity(bool state)
	{
	//	mDoneButton.SetActive(state);
	}
	
	void Initialize()
	{
		if ( mInitlialzed )
		{
			return;
		}
		
		SetHangarVisibility(true);
		mInitlialzed = true;
	}
	
	void OnHangarOpenButton()
	{	
		SetHangarVisibility(true);
	}
	
	void OnDoneButton()
	{	
		SetHangarVisibility(false);	
	}
	
	void SetHangarVisibility(bool state)
	{
		//InventoryManager.GetInstance().gameObject.SetActive(state);
		FleetManager.GetInstance().SetHangarEntry(state);
		_HangarContainer.SlideIn = state;
		InventoryManager.GetInstance().SetVisibility(state);
		_ButtonContainerSlider.SlideIn = state;
		_OpenButtonContainerSlider.SlideIn = !state;
	}
	
	void OnClearButton()
	{
		FleetManager.GetShip().EraseShip();
	}
	
	void Start () 
	{
		Initialize();
	}
	
	void Update () {
	
	}
	
	
	// BUTTON HANDLER EXTENSION
	
	Button _OpenHangarButton;
	Button _CloseHangarButton;
	
	public override void ButtonStarted (Button target)
	{
		base.ButtonStarted (target);
		
		switch (target._Handle)
		{
		case ButtonHandle.HANGAR_OPEN:
			_OpenHangarButton = target;
			target.Visible = false;
			break;
		case ButtonHandle.HANGAR_DONE:
			
			_CloseHangarButton = target;
			break;
		}
		
	}
	
	public override void ButtonPressed (Button target)
	{
		base.ButtonPressed (target);
		
		switch (target._Handle)
		{
		case ButtonHandler.ButtonHandle.HANGAR_ERASE_ALL:
			OnClearButton();
			break;
		case ButtonHandler.ButtonHandle.HANGAR_OPEN:
			OnHangarOpenButton();
			target.Visible = false;
			_CloseHangarButton.Visible = true;
			break;
		case ButtonHandler.ButtonHandle.HANGAR_DONE:
			OnDoneButton();
			target.Visible = false;
			_OpenHangarButton.Visible = true;
			break;
		}
	}
}
