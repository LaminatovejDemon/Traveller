//#define RTT

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
		
#if RTT
		
#else	
		mAttacker._ShipPositionContainer.position += Camera.main.transform.rotation * Vector3.right * 5.0f;
		mDefender._ShipPositionContainer.position += Camera.main.transform.rotation * Vector3.left * 5.0f;
#endif
		
		mAttacker.mStats.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1,1,0)) + Vector3.down * 15.0f;
		mDefender.mStats.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.22f,1,0)) + Vector3.down * 15.0f;
		
		mAttacker.mStats.SetActive(true);
		mDefender.mStats.SetActive(true);
		
		mAttackerComputer = mAttacker.GetComponent<BattleComputer>();
		mDefenderComputer = mDefender.GetComponent<BattleComputer>();
		
		mAttackerComputer.InitBattle();
		mDefenderComputer.InitBattle();
	}
	
	void Turn()
	{
		BattleVisualManager.GetInstance().ResetTurn();
		mAttackerComputer.Attack(mDefender);
		mDefenderComputer.Attack(mAttacker);
		
		//This should be used only when skip
		//mAttacker.RemoveDestroyedParts();
		//mDefender.RemoveDestroyedParts();
		//CheckStats();
		//TurnEnded();
		
	}
	
	public void TurnEnded()
	{
		Debug.Log ("Setting stats in the end of turn");
		mAttacker.SetStats();
		mDefender.SetStats();
		mDefenderComputer.EndTurn();
		mAttackerComputer.EndTurn();
		
		CheckStats();
	}
	
	void CheckStats()
	{
		if ( !mAttacker.IsAlive() || !mDefender.IsAlive() )
		{
			
			_TurnButtonSlider.SlideIn = false;
			HangarManager.GetInstance()._OpenButtonContainerSlider.SlideIn = true;
			_OpenHangarButton.Visible = true;	
			_OpenHangarButton.Caption = "OPEN DOCK";
			_OpenHangarButton.Active = true;	
		}
		else
		{
			_OpenHangarButton.Active = true;
			_TurnButton.Active = true;
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
		
		Ship newShip2_ = FleetManager.GetShip( FleetManager.GetInstance().GetScan("TestShip") );
		FleetManager.GetInstance().RegisterShip(newShip2_);
		
		
		
		//StartBattle (FleetManager.GetShip(), newShip_);
		StartBattle (newShip2_, newShip_);
		_TurnButtonSlider.SlideIn = true;
		HangarManager.GetInstance()._OpenButtonContainerSlider.SlideIn = true;
		_OpenHangarButton.Visible = true;	
		_OpenHangarButton.Caption = "FLEE";
		
		_TurnButton.Active = true;
		_TurnButton.Visible = true;
		
		//MainManager.GetInstance()._BattleCamera.OnFinished(gameObject, "ShowBattleFinished");	
		MainManager.GetInstance()._BattleCamera.Show(FleetManager.GetShip().transform);
			
		MainManager.GetInstance()._EnemyCamera.Show(newShip_.transform);
		
		FleetManager.GetShip().DebugRotate = true;
		newShip_.DebugRotate = true;
	}
	
	public void ShowBattleFinished()
	{
	}
	
	//BUTTONS
	
	Button _TurnButton;
	Button _OpenHangarButton;
	
	public override void ButtonPressed (Button target)
	{
		base.ButtonPressed (target);
		
		switch (target._Handle)
		{
			case ButtonHandler.ButtonHandle.BATTLE_TURN:
			Turn();
			target.Active = false;
			_OpenHangarButton.Active = false;
			break;
			case ButtonHandler.ButtonHandle.HANGAR_OPEN:
			HangarManager.GetInstance().OnHangarOpenButton();
			_TurnButtonSlider.SlideIn = false;
			target.Visible = false;
			mAttacker._Shield.SetVisibility(false);
			mDefender._Shield.SetVisibility(false);
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
		case ButtonHandler.ButtonHandle.HANGAR_OPEN:
			_OpenHangarButton = target;
			break;
		}
	}
	
}
