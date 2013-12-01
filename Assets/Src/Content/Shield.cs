﻿using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour 
{
	Renderer _Visual;
	bool _Initialized = false;
	Ship _ParentShip;
	float _Income;
	float _Recharge;
	string _RechargeText;
	float _Outcome;
	float _OutcomeVisual;
	
	float _RealCapacity = -1;
	Color _BasicColor;
	float _BasicColorAlpha;
	
	public GameObject _ShieldCapacityCaption;
	
	public void EraseIncomes()
	{
		_Recharge = 0;
		_Income = 0;
	}
	
	public void AddActualCapacity(float capacity)
	{
		_Income += capacity;
	}
	
	public void AddActualRecharge(float recharge)
	{
		_Recharge += recharge;
	}
	
	public float GetCapacity()
	{
		return (_Income - _Outcome);
	}
	
	public void Initialize()
	{
		if ( _Initialized )
		{
			return;
		}
		
		_Visual = ((GameObject)GameObject.Instantiate((GameObject)Resources.Load("Content/Shield"))).renderer;
		_ParentShip = gameObject.GetComponent<Ship>();
		
		_Visual.transform.parent = _ParentShip.transform.parent;
		
		_BasicColor = _Visual.material.color;
		_BasicColorAlpha = _BasicColor.a;
		
		_ShieldCapacityCaption = TextManager.GetInstance().GetText("SHIELD: N/A", 0.5f);
		
		_BasicColor.a = 1.0f;
		_ShieldCapacityCaption.GetComponent<TextMesh>().color = _BasicColor;
		
		RecalculateBoundary();
		
		SetVisibility(false);
		
		_Initialized = true;
	}
	
	public void RecalculateBoundary()
	{
		_Visual.transform.localPosition = new Vector3( 0,  0.5f , 0);
		_Visual.transform.localRotation = Quaternion.identity;
		_Visual.transform.localScale = new Vector3(_ParentShip._BoundaryHorizontal + 0.2f, 2.0f, _ParentShip._BoundaryVertical + 0.2f);
		_ShieldCapacityCaption.transform.position = _Visual.transform.position + Camera.main.transform.rotation * Vector3.up * 3.0f;
		_ShieldCapacityCaption.transform.rotation = Camera.main.transform.rotation;
		_ShieldCapacityCaption.transform.parent = _ParentShip.transform.parent.parent;
		
	}
	
	public void SetVisibility(bool state)
	{
		_ShieldCapacityCaption.gameObject.SetActive(state);
		_Visual.gameObject.SetActive(state);
	}
	
	public void ChangeVisualCapacity(float relative)
	{
		_OutcomeVisual += relative;
	}
	
	public void ChangeOutcomeCapacity(float relative)
	{
		_Outcome += relative;
	}
	
	void Update()
	{
		if ( _RealCapacity != (_Income - _OutcomeVisual) )
		{
			_RealCapacity = Mathf.Max((_Income - _OutcomeVisual), 0);
			_BasicColor.a = (_RealCapacity/(_Income)) * _BasicColorAlpha;
			
			_Visual.material.color = _BasicColor;
			
			_ShieldCapacityCaption.GetComponent<TextMesh>().text = "SHIELD: " + (_RechargeText != "" ? ""+(_RealCapacity-_Recharge) + _RechargeText : ""+_RealCapacity );
		}
		
		if ( _RechargeTimestamp != -1 && Time.time - _RechargeTimestamp > 1.0f )
		{
			_RechargeTimestamp = -1;
			_RechargeText = "";
			_ShieldCapacityCaption.GetComponent<TextMesh>().text = "SHIELD: " + _RealCapacity;
		}
	}
	
	float _RechargeTimestamp = -1;
	
	public void Recharge()
	{
		if ( _Recharge > 0 && _Outcome > 0 )
		{
			_RechargeText = " + " + _Recharge;
			
			_RechargeTimestamp = Time.time;
			_Outcome -= _Recharge;
			_OutcomeVisual -= _Recharge;
		}
	}

}