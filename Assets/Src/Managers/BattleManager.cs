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
		
		mAttacker.CalculateShipCenter();
		mDefender.CalculateShipCenter();
		
		mAttacker.transform.position += Vector3.left * 3.0f;
		mDefender.transform.position += Vector3.right * 3.0f;
		
		Shoot(mAttacker, mDefender);
		Shoot(mDefender, mAttacker);
	}
	
	void Shoot(Ship shooter, Ship catcher)
	{
		int direction_, value_;
		for ( int i = 0; i < shooter.mWeaponList.Count; ++i )
		{
			direction_ = Random.Range(0,3);
			value_ = /*Random.Range(0, catcher.GetRange(direction_));*/catcher.GetRange(direction_);
			shooter.Shoot(shooter.mWeaponList[i], direction_, value_);
		}
	}
}
