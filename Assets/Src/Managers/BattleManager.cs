using UnityEngine;
using System.Collections;

public class BattleManager : MonoBehaviour 
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
	
	Ship mAttacker, mDefender;

	public void StartBattle(Ship attacker, Ship defender)
	{		
		mAttacker = attacker;
		mDefender = defender;
		
		mAttacker.mStats.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1,1,0)) + Vector3.down * 15.0f;
		mDefender.mStats.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.22f,1,0)) + Vector3.down * 15.0f;
		
		mAttacker.mStats.SetActive(true);
		mDefender.mStats.SetActive(true);
	//	mAttacker.CalculateShipCenter();
	//	mDefender.CalculateShipCenter();
		
	//	mAttacker.transform.position += Vector3.left * 3.0f;
	//	mDefender.transform.position += Vector3.right * 3.0f;
		
	//	Shoot(mAttacker, mDefender);
	//	Shoot(mDefender, mAttacker);
	}
	
	void Shoot(Ship shooter, Ship catcher)
	{
		int direction_, value_ = 6;
		for ( int i = 0; i < shooter.mWeaponList.Count; ++i )
		{
			direction_ = Random.Range(0,3);
			//value_ = /*Random.Range(0, catcher.GetRange(direction_));*/catcher.GetRange(direction_);
			shooter.Shoot(shooter.mWeaponList[i], direction_, value_);
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
		
		
		MainManager.GetInstance()._BattleCamera.OnFinished(gameObject, "ShowBattleFinished");	
		MainManager.GetInstance()._BattleCamera.Show(FleetManager.GetShip().transform);
			
		MainManager.GetInstance()._EnemyCamera.Show(newShip_.transform);
	}
	
	public void ShowBattleFinished()
	{
		mDefender.DebugRotate_ = true;
	    // BEGIN_TURN
	}
	
}
