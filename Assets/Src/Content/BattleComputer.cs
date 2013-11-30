using UnityEngine;
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
			_Consumption = 0;
		}
		
		public Part _Owner;	
		public PartManager.AbilityType _Ability;
		public bool _Repeater;
		public float _Damage;
		public float _Consumption;
	};
	
	List <Weapon> mWeaponList = new List<Weapon>(); 
	
	public void CheckPart(Part part, bool addition)
	{
		if ( !addition )
		{
			for ( int i = 0; i < mWeaponList.Count; ++i )
			{
				if ( mWeaponList[i]._Owner == part )
				{
					Debug.Log (name +  ": Removing " + part + " from weapon list");
					mWeaponList.RemoveAt(i);
					return;
				}
			}
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
				new_._Consumption = part.mPattern.mPower;
				mWeaponList.Add(new_);				
				break;
			}
		}
	}
	
	public enum Side
	{
		Top,
		Left,
		Right,
		Bottom,
		COUNT,
	};
	
	Side RollSide()
	{
		return 0;// FIXME(Side)(Random.Range(0, (int)Side.COUNT));
	}
	
	int RollIndex(Ship target, Side side)
	{
		int max_ = ((side == Side.Left || side == Side.Right) ? target._BoundaryVertical : target._BoundaryHorizontal);
		
		Debug.Log ("Rolling on " + side + " <" + 0 + ", " + max_ +")" );
			
		return Random.Range(0, max_);
	}
	
	public void Attack(Ship target)
	{
		if ( _ParentShip == null )
		{
			_ParentShip = GetComponent<Ship>();
		}
		
		float GeneratorEnergy_ = _ParentShip.mEnergyProduction;
		
		Debug.Log ("Attacking with energy " + GeneratorEnergy_);
		
		for ( int i = 0; i < mWeaponList.Count; ++i )
		{
			Side side_ = RollSide();
			if ( GeneratorEnergy_ < mWeaponList[i]._Consumption )
			{
				continue;
			}
			
			if (mWeaponList[i]._Repeater )
			{	
				Shoot(target, mWeaponList[i], side_);
				Shoot(target, mWeaponList[i], side_);
			}
			Shoot(target, mWeaponList[i], side_);
			
			GeneratorEnergy_ -= mWeaponList[i]._Consumption;
			
		}
	}
	
	void Shoot(Ship target, Weapon weapon, Side side)
	{
		int index_ = RollIndex(target, side);
		
		Part targetPart_ = target.GetComponent<BattleComputer>().GetTarget(side, index_);
		
		if ( targetPart_ != null )
		{
			BattleVisualManager.GetInstance().QueueFire(weapon._Owner, targetPart_, weapon._Ability , side);
			targetPart_.mHP--;
		}
		else
		{
			BattleVisualManager.GetInstance().QueueFire(weapon._Owner, null, weapon._Ability , side, index_, target);
		}
			
		Debug.Log ("Shooting at " + target + "'s " + side + " "+ index_ +" with " + weapon._Ability + "("+weapon._Damage+") of " + weapon._Owner.name );
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
				target_ = _ParentShip.GetPartAt(index_ + _ParentShip._OffsetVertical, i_);
			}
			
			if ( target_ != null && target_.mHP > 0 )
			{
				return target_;
			}
		}
		
		return null;
	}
	
}
