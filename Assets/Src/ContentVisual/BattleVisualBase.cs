using UnityEngine;
using System.Collections;

public class BattleVisualBase : MonoBehaviour 
{
	public Transform _CameraRotationContainer;

	public virtual void InitializeBattle()
	{

	}

	public virtual void InitializeTurn()
	{
		ResetTurn();
	}

	public virtual void EndTurn()
	{
		BattleVisualManager.GetInstance().HandlerEnded();
	}

	public virtual void QueueFire(Part source, Part target, PartManager.AbilityType type, BattleComputer.Side side, int index_, Ship targetShip, bool intoShield, float damage)
	{

	}

	protected bool _TurnHitProceedEnabled;
	
	// when every projectile is fired
	public virtual void ProceedHit()
	{
		_TurnHitProceedEnabled = true;
	}
	
	// override without base, end turn just when there is no visual
	public virtual void AttackFinished()
	{
		EndTurn ();
	}

	protected virtual void ResetTurn()
	{
		_TurnHitProceedEnabled = false;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
