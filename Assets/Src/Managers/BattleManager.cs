using UnityEngine;
using System.Collections;

public class BattleManager : ButtonHandler 
{
	private static BattleManager mInstance = null;
	public static BattleManager GetInstance()
	{
		if ( mInstance == null )
		{
			GameObject instanceObject_ = GameObject.Find("#BattleManager");
			if ( instanceObject_ != null )
				mInstance = instanceObject_.transform.GetComponent<BattleManager>();
			else
				mInstance =  new GameObject("#BattleManager").AddComponent<BattleManager>();
		}
		return mInstance;
	}
	
	public FrameSlider _TurnButtonSlider;
	public FrameSlider _OpenHangarSlider;
	
	Ship mAttacker, mDefender;
	BattleComputer mAttackerComputer, mDefenderComputer;

	public void StartBattle(Ship attacker, Ship defender)
	{		
		mAttacker = attacker;
		mDefender = defender;
		
		mAttacker.mStats.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1,1,0)) + Vector3.down * 15.0f;
		mDefender.mStats.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.22f,1,0)) + Vector3.down * 15.0f;
		
		mAttacker.mStats.SetActive(true);
		mDefender.mStats.SetActive(true);
		
		mAttackerComputer = mAttacker.GetComponent<BattleComputer>();
		mDefenderComputer = mDefender.GetComponent<BattleComputer>();
	}
	
	void Turn()
	{
		mAttackerComputer.Attack(mDefender);
		mDefenderComputer.Attack(mAttacker);
		
		mAttacker.RemoveDestroyedParts();
		mDefender.RemoveDestroyedParts();
		
		CheckStats();
	}
	
	void CheckStats()
	{
		if ( !mAttacker.IsAlive() )
		{
			_TurnButtonSlider.SlideIn = false;
			HangarManager.GetInstance()._OpenButtonContainerSlider.SlideIn = true;
		}
	}
	
		
	public void ShowBattle()
	{
		if ( mDefender != null )
		{
			FleetManager.GetInstance().DestroyShipInstance(mDefender);
		}
		Ship newShip_ = FleetManager.GetShip( FleetManager.GetInstance().GetScan("TestShip") );
		FleetManager.GetInstance().RegisterShip(newShip_);
		
		StartBattle (FleetManager.GetShip(), newShip_);
		_TurnButtonSlider.SlideIn = true;
		
		MainManager.GetInstance()._BattleCamera.OnFinished(gameObject, "ShowBattleFinished");	
		MainManager.GetInstance()._BattleCamera.Show(FleetManager.GetShip().transform);
			
		MainManager.GetInstance()._EnemyCamera.Show(newShip_.transform);
	}
	
	public void ShowBattleFinished()
	{
		mDefender.DebugRotate_ = true;
	    // BEGIN_TURN
	}
	
	//BUTTONS
	
	Button _TurnButton;
	
	public override void ButtonPressed (Button target)
	{
		base.ButtonPressed (target);
		
		switch (target._Handle)
		{
			case ButtonHandler.ButtonHandle.BATTLE_TURN:
			Turn();
			break;
			case ButtonHandler.ButtonHandle.HANGAR_OPEN:
			HangarManager.GetInstance().OnHangarOpenButton();
			target.Visible = false;
			
			break;
		}
	}
	
	public override void ButtonStarted (Button target)
	{
		base.ButtonStarted (target);
		
		switch (target._Handle)
		{
		case ButtonHandler.ButtonHandle.BATTLE_TURN:
			_TurnButton = target;
			break;
		}
	}
	
}
