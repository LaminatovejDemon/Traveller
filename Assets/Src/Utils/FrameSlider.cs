using UnityEngine;
using System.Collections;

public class FrameSlider : ButtonHandler 
{
	public Transform _Container;
	public Transform _EndPosition;
	
	Vector3 _StartPosition;
	
	public bool _SlideIn = false;
	
	public bool SlideIn{
		get {	return _SlideIn;}
		set {
			if ( value == _SlideIn )
			{
				return;
			}
			
			_SlideIn = value;
			Slide(_SlideIn);
		}
	}
	
	public void Start()
	{
		_StartPosition = _Container.localPosition;
		if ( _SlideIn )
		{
			_Container.localPosition = _TargetPosition = _EndPosition.localPosition;
		}
		else
		{
			_Container.localPosition  = _TargetPosition = _StartPosition;
		}
	}
	
	void Slide(bool direction)
	{
		_TargetPosition = direction ? _EndPosition.localPosition : _StartPosition;
	}
	
	Vector3 _TargetPosition;
	
	public void Update()
	{
		if ( _TargetPosition != _Container.localPosition  )
		{
			_Container.localPosition = Utils.Interpolate(_Container.localPosition , _TargetPosition);
		}
	}
	
	public override void ButtonPressed (Button target)
	{
		base.ButtonPressed (target);
		
		SlideIn = !SlideIn;
	}
}
