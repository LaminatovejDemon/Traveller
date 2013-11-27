using UnityEngine;
using System.Collections;

public class FrameSlider : ButtonHandler 
{
	public Transform _Container;
	public Transform _EndPosition;
	
	Vector3 _StartPosition;
	
	public bool _SlideInLocal = false;
	
	bool _SlideIn{
		get {	return _SlideInLocal;}
		set {
			if ( value == _SlideInLocal )
			{
				return;
			}
			
			_SlideInLocal = value;
			if ( _SlideInLocal )
			{
				SlideIn();
			}
			else
			{
				SlideOut();
			}
		}
	}
	
	public void Start()
	{
		_StartPosition = _Container.position;
		if ( _SlideInLocal )
		{
			_Container.position = _TargetPosition = _EndPosition.position;
		}
		else
		{
			_Container.position = _TargetPosition = _StartPosition;
		}
	}
	
	void SlideIn()
	{
		_TargetPosition = _EndPosition.position;
	}
	
	void SlideOut()
	{
		_TargetPosition = _StartPosition;
	}
	
	Vector3 _TargetPosition;
	
	public void Update()
	{
		if ( _TargetPosition != _Container.position )
		{
			_Container.position = Utils.Interpolate(_Container.position, _TargetPosition);
		}
	}
	
	public override void ButtonPressed (Button target)
	{
		base.ButtonPressed (target);
		
		_SlideIn = !_SlideIn;
	}
}
