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
		
		_HideBattleFinished = false;
		_HideHangarFinished = false;
		_HangarSlideCompleted = false;
		
		AnimateShipEntry(state);
	
		_HangarContainer.SlideIn = state;
		_HangarContainer.OnFinished(gameObject, "HangarSlideCompleted", state);
		InventoryManager.GetInstance().SetVisibility(state);
		_ButtonContainerSlider.SlideIn = state;
		_OpenButtonContainerSlider.SlideIn = !state;	
	}
	
	bool _HideBattleFinished = false;
	bool _HideHangarFinished = false;
	bool _HangarSlideCompleted = false;
	
	void HangarSlideCompleted(bool state)
	{
		_HangarSlideCompleted = true;
		CheckHangarStateChange();
	}
	
	public void HideBattleFinished()
	{
		_HideBattleFinished = true;
		CheckHangarStateChange();

	}
	
	public void HideHangarFinished()
	{
		_HideHangarFinished = true;
		CheckHangarStateChange();
	}
	
	void CheckHangarStateChange()
	{
		if ( _HideBattleFinished && _HangarSlideCompleted)
		{
			_HideBattleFinished = false;
			_HideHangarFinished = false;
			_HangarSlideCompleted = false;
			
			ShowHangar();
		}
		else if ( _HideHangarFinished && _HangarSlideCompleted)
		{
			_HideBattleFinished = false;
			_HideHangarFinished = false;
			_HangarSlideCompleted = false;
			
			ShowBattle();
		}
	}
	
	void ShowHangar()
	{
		MainManager.GetInstance()._HangarCamera.OnFinished(gameObject, "ShowHangarFinished");
		MainManager.GetInstance()._HangarCamera.Show(FleetManager.GetShip().transform);	
	}
	
	void ShowBattle()
	{
		MainManager.GetInstance()._BattleCamera.OnFinished(gameObject, "ShowBattleFinished");	
		MainManager.GetInstance()._BattleCamera.Show(FleetManager.GetShip().transform);
	}
	
	void AnimateShipEntry(bool state)
	{
		if ( !state )
		{
			MainManager.GetInstance()._HangarCamera.OnFinished(gameObject, "HideHangarFinished");	
			MainManager.GetInstance()._HangarCamera.Hide();
		}
		else
		{			
			MainManager.GetInstance()._BattleCamera.OnFinished(gameObject, "HideBattleFinished");	
			MainManager.GetInstance()._BattleCamera.Hide();
		}
	}
	
	public void ShowHangarFinished()
	{
		transform.localRotation = Quaternion.identity;		
		//Utils.SetLayer(transform, LayerMask.NameToLayer("HangarCamera"));
	}
	
	public void ShowBattleFinished()
	{
	/* Ship newShip_ = FleetManager.GetShip( FleetManager.GetInstance().GetScan("SomeShip") );
		FleetManager.GetInstance().RegisterShip(newShip_);
			
		BattleManager.GetInstance().StartBattle(this , newShip_);*/
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
