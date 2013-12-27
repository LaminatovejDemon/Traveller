using UnityEngine;
using System.Collections;

public class ProjectileVisual : MonoBehaviour 
{
	bool _Initialized = false;

	public void Initialize(Part link, float angle, BattleComputer.Side targetSide, int  index, Ship targetShip, bool intoShield, float damage)
	{
		_Initialized = true;
		_Link = link;
		_Angle = angle;
		_index = index;
		_TargetShip = targetShip;
		_TargetSide = targetSide;
		_IntoShield = intoShield;
		_Damage = damage;
	}

	public float _Damage;
	public float _Angle;
	public bool _IntoShield;

	public Part _Link;
	public Ship _TargetShip;
	public int _index;
	public BattleComputer.Side _TargetSide;
}
