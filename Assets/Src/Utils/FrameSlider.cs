using UnityEngine;
using System.Collections;

public class FrameSlider : ButtonHandler 
{
	public Transform _Container;
	public Transform _EndPosition;
	
	Vector3 _StartPosition;
	
	public bool _SlideIn = false;
	public bool _InstantStart = false;
	
	public bool SlideIn{
		get {	return _SlideIn;}
		set {
			
			
			_SlideIn = value;
			Slide(_SlideIn);
		}
	}
	
	Object _OnFinished = null;
	string _OnFinishedMethod;
	bool _OnFinishedParameter;
	
	public void OnFinished(Object target, string method, bool parameter = true)
	{
		_OnFinished = target;
		_OnFinishedMethod = method;
		_OnFinishedParameter = parameter;
	}
	
	public void Start()
	{
		_StartPosition = _Container.localPosition;
		if ( _SlideIn )
		{
			_TargetPosition = _EndPosition.localPosition;
			if ( _InstantStart )
			{
				_Container.localPosition = _TargetPosition;
			}
		}
		else
		{
			_TargetPosition = _StartPosition;
			if ( _InstantStart )
			{
				_Container.localPosition = _TargetPosition;
			}
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
		else
		{
			if ( _OnFinished != null )
			{
				((GameObject)_OnFinished).SendMessage(_OnFinishedMethod, _OnFinishedParameter, SendMessageOptions.RequireReceiver);
				_OnFinished = null;
			}
		}
	}
	
	public override void ButtonPressed (Button target)
	{
		base.ButtonPressed (target);
		
		SlideIn = !SlideIn;
	}
}
