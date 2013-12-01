﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleComputer : MonoBehaviour 
{
	Ship _ParentShip;
	
	struct Weapon
	{
		public Weapon (Part part, PartManager.AbilityType ability)
		{
			_Owner = part;
			_Ability = ability;
			_Repeater = false;
			_Damage = 0;
		}
		
		public Part _Owner;	
		public PartManager.AbilityType _Ability;
		public bool _Repeater;
		public float _Damage;
	};
	
	List <Weapon> mWeaponList = new List<Weapon>(); 
	
	public void ClearWeaponList()
	{
		// NOT Outside Battle
		if ( _ParentShip == null )
		{
			return;
		}
		
		mWeaponList.Clear();
		_ParentShip._Shield.EraseIncomes();
	}
	
	public void InitBattle()
	{
		if ( _ParentShip == null )
		{
			_ParentShip = GetComponent<Ship>();
		}
		
		_ParentShip.SetStats();
	}
	
	public void EndTurn()
	{
		_ParentShip._Shield.Recharge();
	}
	
	public void AddConsumer(Part part)
	{
		// Not when not in battle
		if ( _ParentShip == null )
		{
			return;
		}
		
		bool repeater_ = false;
		for ( int i = 0; i < part.mPattern.mAbilityList.Count; ++i )
		{
			if ( part.mPattern.mAbilityList[i].mType == PartManager.AbilityType.BeamRepeater )
			{
				repeater_ = true;	
			}	
		}
		
		for ( int i = 0; i < part.mPattern.mAbilityList.Count; ++i )
		{
			switch (part.mPattern.mAbilityList[i].mType )
			{
			case PartManager.AbilityType.Beam:
				Weapon new_ = new Weapon(part, part.mPattern.mAbilityList[i].mType);
				new_._Repeater = repeater_;
				new_._Damage = part.mPattern.mAbilityList[i].mValue;
				mWeaponList.Add(new_);			
				Debug.Log ("\t" + part);
				break;
			case PartManager.AbilityType.ShieldCapacity:
				// When powering/disabling shields during a battle
				_ParentShip._Shield.AddActualCapacity(part.mPattern.mAbilityList[i].mValue);
				_ParentShip._Shield.SetVisibility(true);
				break;
			case PartManager.AbilityType.ShieldRecharge:
				// When powering/disabling shields during a battle
				_ParentShip._Shield.AddActualRecharge(part.mPattern.mAbilityList[i].mValue);
				break;
			}
		}
	
	}
	
	public enum Side
	{
		Top = 0,
		Right = 1,
		Bottom = 2,
		Left = 3,
		COUNT = 4,
	};
	
	Side RollSide()
	{
		return (Side)(Random.Range(0, (int)Side.COUNT));
	}
	
	int RollIndex(Ship target, Side side)
	{
		int max_ = ((side == Side.Left || side == Side.Right) ? target._BoundaryVertical : target._BoundaryHorizontal);
		
		return Random.Range(0, max_);
	}
	
	public void Attack(Ship target)
	{
		for ( int i = 0; i < mWeaponList.Count; ++i )
		{
			Side side_ = RollSide();

			if (mWeaponList[i]._Repeater )
			{	
				Shoot(target, mWeaponList[i], side_);
				Shoot(target, mWeaponList[i], side_);
			}
			Shoot(target, mWeaponList[i], side_);
			
		}
	}
	
	void Shoot(Ship target, Weapon weapon, Side side)
	{
		int index_ = RollIndex(target, side);
		
		Part targetPart_ = target.GetComponent<BattleComputer>().GetTarget(side, index_);
		
		if ( targetPart_ != null )
		{
			Debug.Log ("evade of " + target + " is " + target.GetEvade() );
			if ( Random.value < target.GetEvade() )
			{
				targetPart_ = null;
				index_ = -1;
			}
		}
		
		
		if ( targetPart_ != null )
		{
			if (target._Shield.GetCapacity() > 0 )
			{
				BattleVisualManager.GetInstance().QueueFire(weapon._Owner, targetPart_, weapon._Ability , side, index_, target, true, 1);
				target._Shield.ChangeOutcomeCapacity(weapon._Damage);
			}
			else
			{
				BattleVisualManager.GetInstance().QueueFire(weapon._Owner, targetPart_, weapon._Ability , side, index_, target, false, 1);
				targetPart_.mHP -= weapon._Damage;
			}
		}
		else
		{
			BattleVisualManager.GetInstance().QueueFire(weapon._Owner, null, weapon._Ability , side, index_, target, false, 0);
		}
			
//		Debug.Log ("Shooting at " + target + "'s " + side + " "+ index_ +" with " + weapon._Ability + "("+weapon._Damage+") of " + weapon._Owner.name );
	}
	
	Part GetTarget(Side side, int index_)
	{
		if ( _ParentShip == null )
		{
			_ParentShip = GetComponent<Ship>();
		}
	
		int inc_ = (side == Side.Left || side == Side.Bottom) ? 1 : -1;
		
		int i_ = inc_ == 1 ? 0 : HangarManager.HANGAR_SIZE-1;
		int top_ = inc_ == 1 ? HangarManager.HANGAR_SIZE-1 : 0;
		Part target_ = null;
		for ( ; i_ != top_; i_ += inc_ )
		{
			if ( side == Side.Left || side == Side.Right )
			{
				target_ = _ParentShip.GetPartAt(i_, index_ + _ParentShip._OffsetVertical);
			}
			else
			{
				target_ = _ParentShip.GetPartAt(index_ + _ParentShip._OffsetHorizontal, i_);
			}
			
			if ( target_ != null && target_.mHP > 0 )
			{
				return target_;
			}
		}
		
		return null;
	}
	
}
