using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleVisualManager : MonoBehaviour
{
	public const float ANIMATION_SPEED = 4.0f;
	public const float SHOOT_DELTA_TIME = 0.10f;
	
	private static BattleVisualManager mInstance = null;

	List<BattleVisualBase> _HandlerList = new List<BattleVisualBase>();
	public int _FireFinishedHandlerCount;
	public int _HitFinishedHandlerCount;

	public static BattleVisualManager GetInstance()
	{
		if ( mInstance == null )
		{
			GameObject instanceObject_ = GameObject.Find("#BattleVisualManager");
			if ( instanceObject_ != null )
				mInstance = instanceObject_.transform.GetComponent<BattleVisualManager>();
			else
				mInstance =  new GameObject("#BattleVisualManager").AddComponent<BattleVisualManager>();
		}
		return mInstance;
	}

	public void InitializeBattle()
	{
		_HandlerList.Clear();
	}

	public void InitializeTurn()
	{
		_FireFinishedHandlerCount = 0;
		_HitFinishedHandlerCount = 0;
	}

	public void RegisterHandler(BattleVisualBase handler)
	{
		_HandlerList.Add(handler);

		handler.InitializeBattle();
	}

	public void HandlerFireEnded()
	{
		_FireFinishedHandlerCount++;
		CheckFireFinished();
	}

	void CheckFireFinished()
	{
		if ( _FireFinishedHandlerCount >= _HandlerList.Count )
		{
			ProceedHit();
		}
	}

	public void HandlerEnded()
	{
		_HitFinishedHandlerCount++;
		CheckHitFinished();
	}

	void ProceedHit()
	{
		for ( int i = 0 ; i < _HandlerList.Count; ++i )
		{
			_HandlerList[i].ProceedHit();
		}
	}

	void CheckHitFinished()
	{
		if ( _HitFinishedHandlerCount >= _HandlerList.Count )
		{
			ProceedEndTurn();
		}
	}

	void ProceedEndTurn()
	{
		BattleManager.GetInstance().TurnEnded();
	}
}
