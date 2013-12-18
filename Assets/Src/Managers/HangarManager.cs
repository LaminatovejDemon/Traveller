using UnityEngine;
using System.Collections;

public class HangarManager : ButtonHandler 
{
	public const int HANGAR_SIZE = 6;
	
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
	
	bool ValidShip_ = false;
	
	public void InformShipValidity(bool state)
	{
		ValidShip_ = state;
		if ( _CloseHangarButton != null )
		{
			_CloseHangarButton.Visible = true;
			_CloseHangarButton.Active = state;
		}
	}
	
	void Initialize()
	{
		if ( mInitlialzed )
		{
			return;
		}
		
		_HangarContainer.transform.GetChild(0).localScale = new Vector3(HANGAR_SIZE/10.0f, 1, HANGAR_SIZE/10.0f);
		_HangarContainer.transform.GetChild(0).renderer.material.mainTextureScale = new Vector2(HANGAR_SIZE, HANGAR_SIZE);
		
		SetHangarVisibility(true);
		
		mInitlialzed = true;
	}
	
	public void OnHangarOpenButton()
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
		FleetManager.GetShip().gameObject.SetActive(state);
		
		_HangarContainer.SlideIn = state;
		_HangarContainer.OnFinished(gameObject, "HangarSlideCompleted", state);
		InventoryManager.GetInstance().SetVisibility(state);
		_ButtonContainerSlider.SlideIn = state;
		//_OpenButtonContainerSlider.SlideIn = !state;	
		
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
			
			BattleManager.GetInstance().ShowBattle();

		}
	}
	
	void ShowHangar()
	{
		MainManager.GetInstance()._HangarCamera.OnFinished(gameObject, "ShowHangarFinished");
		MainManager.GetInstance()._HangarCamera.Show(FleetManager.GetShip().transform);	

		//sort order
		MainManager.GetInstance()._InventoryCamera.enabled = false;
		MainManager.GetInstance()._InventoryCamera.enabled = true;
		MainManager.GetInstance()._GUICamera.enabled = false;
		MainManager.GetInstance()._GUICamera.enabled = true;
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
			MainManager.GetInstance()._EnemyCamera.Hide();
			FleetManager.GetShip()._Shield.SetVisibility(false);
		}
	}
	
	public void ShowHangarFinished()
	{
		transform.localRotation = Quaternion.identity;		
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
	
	//Button _OpenHangarButton;
	public Button _CloseHangarButton;
	
	public override void ButtonStarted (Button target)
	{
		base.ButtonStarted (target);
		
		switch (target._Handle)
		{
		/*case ButtonHandle.HANGAR_OPEN:
			_OpenHangarButton = target;
			target.Visible = false;
			break;*/
		case ButtonHandle.HANGAR_DONE:
			
			_CloseHangarButton = target;
			_CloseHangarButton.Visible = true;
			_CloseHangarButton.Active = ValidShip_;

			break;
		}
		
	}
	
	public override void ButtonPressed (Button target)
	{
		base.ButtonPressed (target);
		
		switch (target._Handle)
		{
		case ButtonHandler.ButtonHandle.CONFIRM:
			MainManager.GetInstance()._PlayerData.DeleteAll();
			Utils.DestroyParentWindow(target.gameObject);

			break;

		case ButtonHandler.ButtonHandle.CANCEL:
			Utils.DestroyParentWindow(target.gameObject);
			break;

		case ButtonHandler.ButtonHandle.HANGAR_ERASE_ALL:
			PopupManager.GetInstance().CreateConfirmPopup("ERASE ALL DATA", "Are you sure to erase ALL DATA?", this);
			break;

		case ButtonHandler.ButtonHandle.HANGAR_ERASE_SHIP:
			OnClearButton();
			break;
		
		case ButtonHandler.ButtonHandle.HANGAR_DONE:
			OnDoneButton();
			target.Visible = false;
			break;
		}
	}
}
