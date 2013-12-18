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
		
		mAttacker._ShipPositionContainer.position += Camera.main.transform.rotation * Vector3.right * 5.0f;
		mDefender._ShipPositionContainer.position += Camera.main.transform.rotation * Vector3.left * 5.0f;
		
		mAttacker.mStats.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1,1,0)) + Vector3.down * 15.0f;
		mDefender.mStats.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.22f,1,0)) + Vector3.down * 15.0f;
		
		mAttacker.mStats.SetActive(false);
		mDefender.mStats.SetActive(false);
		
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

			MainManager.GetInstance()._PlayerData.SetStats(mDefender.IsAlive(), mAttacker.IsAlive());

			if  ( mDefender.IsAlive() )
			{
				LootManager.GetInstance().GetLoot();
			}


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
		Ship NPCShip_ = FleetManager.GetShip( FleetManager.GetInstance().GetRandomScan() );
		FleetManager.GetInstance().RegisterShip(NPCShip_);
		
		Ship PlayerShip_ = FleetManager.GetShip( FleetManager.GetInstance().GetPlayerScan() );
		FleetManager.GetInstance().RegisterShip(PlayerShip_);
		
		//StartBattle (FleetManager.GetShip(), newShip_);
		StartBattle (NPCShip_, PlayerShip_);
		_TurnButtonSlider.SlideIn = true;
		HangarManager.GetInstance()._OpenButtonContainerSlider.SlideIn = true;
		_OpenHangarButton.Visible = true;	
		_OpenHangarButton.Caption = "FLEE";
		
		_TurnButton.Active = true;
		_TurnButton.Visible = true;
		
		//MainManager.GetInstance()._BattleCamera.OnFinished(gameObject, "ShowBattleFinished");	
	
		// Dummy instead of ship itself
		//MainManager.GetInstance()._BattleCamera.Show(FleetManager.GetShip().transform);
		MainManager.GetInstance()._BattleCamera.Show(PlayerShip_.transform.parent);
		MainManager.GetInstance()._EnemyCamera.Show(NPCShip_.transform.parent);
		
		//FleetManager.GetShip().DebugRotate = true;
		PlayerShip_.DebugRotate = true;
		NPCShip_.DebugRotate = true;
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
			GameObject.Destroy(mAttacker.mStats.gameObject);
			GameObject.Destroy(mAttacker.transform.parent.parent.gameObject);
			GameObject.Destroy(mDefender.mStats.gameObject);
			GameObject.Destroy(mDefender.transform.parent.parent.gameObject);
			
			//mAttacker._Shield.SetVisibility(false);
			//mDefender._Shield.SetVisibility(false);
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
