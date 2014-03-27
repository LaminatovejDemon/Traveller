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

	//	mAttacker._ShipPositionContainer.position += Camera.main.transform.rotation * Vector3.right * 5.0f;
	//	mDefender._ShipPositionContainer.position += Camera.main.transform.rotation * Vector3.left * 5.0f;
		
		mAttacker.mStats.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1,1,0)) + Vector3.down * 15.0f;
		mDefender.mStats.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.22f,1,0)) + Vector3.down * 15.0f;
		
		mAttacker.mStats.SetActive(false);
		mDefender.mStats.SetActive(false);
		
		mAttackerComputer = mAttacker.GetComponent<BattleComputer>();
		mDefenderComputer = mDefender.GetComponent<BattleComputer>();

		mAttackerComputer.InitBattle();
		mDefenderComputer.InitBattle();
	}

	bool _Simulation = false;

	public void SetSimulation(bool state)
	{
		_Simulation = state;
	}

	bool _AutoTurn = false;
	
	public void SetAutoTurn(bool state)
	{
		_AutoTurn = state;
	}
	
	void Turn()
	{
		BattleVisualManager.GetInstance().InitializeTurn();
		mAttacker.GetComponent<BattleVisualBase>().InitializeTurn();
		mDefender.GetComponent<BattleVisualBase>().InitializeTurn();

		mAttackerComputer.Attack(mDefender);
		mDefenderComputer.Attack(mAttacker);

		mAttackerComputer.AttackFinished();
		mDefenderComputer.AttackFinished();
	}
	
	public void TurnEnded()
	{
		mAttacker.SetStats();
		mDefender.SetStats();
		mDefenderComputer.EndTurn();
		mAttackerComputer.EndTurn();
		
		CheckStats();
	}

	void BattleCleanup()
	{
		GameObject.Destroy(mAttacker.mStats.gameObject);
		GameObject.Destroy(mAttacker.transform.parent.parent.gameObject);
		GameObject.Destroy(mDefender.mStats.gameObject);
		GameObject.Destroy(mDefender.transform.parent.parent.gameObject);

		BattlePending = false;
	}
	
	bool CheckStats()
	{
		if ( !mAttacker.IsAlive() || !mDefender.IsAlive() || !(mAttacker.DoesDamage() || mDefender.DoesDamage()) 
		    || mAttacker._ScanParent == null )
		{
			BattleFinished();
			return false;
		}
		else
		{
			if ( _AutoTurn )
			{
				Turn ();
			}
			else
			{
				_OpenHangarButton.Active = true;
				_TurnButton.Active = true;
			}
		}
		return true;
	}

	void BattleFinished()
	{
		// settings scan stats
		if ( mAttacker._ScanParent != null )
		{
			mAttacker._ScanParent.GetTierData().SetStats(mAttacker.IsAlive(), mDefender.IsAlive());
			mAttacker._ScanParent.name = "" + mAttacker._ScanParent.GetTierData().GetVictoryCoef().ToString("0.00") + "_ShipScan";
		}

		mDefender._ScanParent.GetTierData().SetStats(mDefender.IsAlive(), mAttacker.IsAlive());


		if ( !_Simulation )
		{
			// and parent stats
			FleetManager.GetShip().GetTierData().SetStats(mDefender.IsAlive(), mAttacker.IsAlive());

			PopupManager.GetInstance().DisplayRewardPopup(mDefender.IsAlive(), mAttacker.IsAlive());
			
			if  ( mDefender.IsAlive() )
			{
				LootManager.GetInstance().GetLoot(mAttacker._ScanParent);
			}
			
			_TurnButtonSlider.SlideIn = false;
			HangarManager.GetInstance()._OpenButtonContainerSlider.SlideIn = true;
			_OpenHangarButton.Visible = true;	
			_OpenHangarButton.Caption = "OPEN DOCK";
			_OpenHangarButton.Active = true;	
		}
		else
		{
			BattleCleanup();
		}
	}

	bool BattlePending = false;
	
	public void SimulateBattle(ShipScan ownerScan, int count)
	{
		if ( BattlePending || ownerScan == null )
		{
			return;
		}

		float time_ = Time.time;
		for ( int i = 0; i < count; ++i )
		{
			ShipScan NPCScan1_ = FleetManager.GetInstance().GetAverageScan(ownerScan.GetTierData().GetVictoryCoef());

			if ( NPCScan1_ == null )
			{
				return;
			}

			BattlePending = true;

			BattleVisualManager.GetInstance().InitializeBattle();

			Ship NPCShip1_ = FleetManager.GetShip( NPCScan1_ );
			Ship NPCShip2_ = FleetManager.GetShip( ownerScan );

			BattleVisualManager.GetInstance().RegisterHandler(NPCShip1_.gameObject.AddComponent<BattleVisualBase>());
			BattleVisualManager.GetInstance().RegisterHandler(NPCShip2_.gameObject.AddComponent<BattleVisualBase>());

			StartBattle (NPCShip1_, NPCShip2_);

			SetAutoTurn(true);
			SetSimulation(true);

			bool battleFinished_ = CheckStats();

		//	Debug.Log ("\t\t...Doing some simulations between " + NPCShip1_ + " and " + NPCShip2_ + " and " + battleFinished_ + " with " + ownerScan.GetTierData().GetVictoryCoef());

		}
		//Debug.Log ("Simulation lasted " + ((Time.time - time_) * 100.0f) + "s.");
	}
	
	public bool ShowBattle()
	{
		if ( BattlePending )
		{
			return false;
		}

		BattlePending = true;

		BattleVisualManager.GetInstance().InitializeBattle();

		Ship NPCShip_ = FleetManager.GetShip( FleetManager.GetInstance().GetAverageScan(FleetManager.GetInstance().GetPlayerScan().GetTierData().GetVictoryCoef()) );
		BattleVisualManager.GetInstance().RegisterHandler(NPCShip_.gameObject.AddComponent<BattleVisualHandler>());

		Ship PlayerShip_ = FleetManager.GetShip( FleetManager.GetInstance().GetPlayerScan() );
		BattleVisualManager.GetInstance().RegisterHandler(PlayerShip_.gameObject.AddComponent<BattleVisualHandler>());

		//Random.seed = 15;

		SetAutoTurn(false);
		SetSimulation(false);

		StartBattle (NPCShip_, PlayerShip_);
		_TurnButtonSlider.SlideIn = true;
		HangarManager.GetInstance()._OpenButtonContainerSlider.SlideIn = true;
		_OpenHangarButton.Visible = true;	
		_OpenHangarButton.Caption = "FLEE";
		
		_TurnButton.Active = true;
		_TurnButton.Visible = true;
		
		MainManager.GetInstance()._BattleCamera.Show(PlayerShip_.transform.parent);
		PlayerShip_.gameObject.GetComponent<BattleVisualHandler>()._CameraRotationContainer = MainManager.GetInstance()._BattleCamera._RealCamera.transform.parent;
		PlayerShip_.SetCamera(MainManager.GetInstance()._BattleCamera);


		MainManager.GetInstance()._EnemyCamera.Show(NPCShip_.transform.parent);
		NPCShip_.gameObject.GetComponent<BattleVisualHandler>()._CameraRotationContainer = MainManager.GetInstance()._EnemyCamera._RealCamera.transform.parent;
		NPCShip_.SetCamera(MainManager.GetInstance()._EnemyCamera);


		CheckStats();

		return true;
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
			// FIXME: flee anytime _OpenHangarButton.Active = false;
			break;
			case ButtonHandler.ButtonHandle.HANGAR_OPEN:

			HangarManager.GetInstance().OnHangarOpenButton();
			_TurnButtonSlider.SlideIn = false;
			target.Visible = false;
			BattleCleanup();
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
