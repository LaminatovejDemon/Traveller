using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour 
{
	
	public Transform _ActiveContainer;
	public Transform _DeactiveContainer;
	
	public ButtonHandler _Handler;
	public ButtonHandler.ButtonHandle _Handle;
	
	public TextMesh _ActiveTextHandle;
	public TextMesh _InActiveTextHandle;
	
	public GameObject _VisibilityContainer;
	
	public bool _Active = true;
	public bool Active
	{
		set 
		{ 
			if ( _Active == value ) 
			{
				return;
			}
			_Active = value;
			
			UpdateVisibility();
		}
		get{ 
			return _Active;
		}
	}
	
	void UpdateVisibility()
	{
		_ActiveContainer.gameObject.SetActive(_Active);
		_DeactiveContainer.gameObject.SetActive(!_Active);
	}
	
	bool _Visible = true;
	public bool Visible
	{
		set
		{ 
			if ( _Visible == value )
			{
				return;
			}
			_Visible = value; 
			_VisibilityContainer.SetActive(value);
		}
		get
		{ 
			return _Visible; 
		}
	}
	
	public string _Caption;
	
	private int mFingerID = -1;
	
	void Start()
	{
		if ( _ActiveTextHandle != null )
		{
			_ActiveTextHandle.text = _Caption;
		}
		
		if ( _InActiveTextHandle != null )
		{
			_InActiveTextHandle.text = _Caption;
		}
		
		UpdateVisibility();
		
		_Handler.SendMessage("ButtonStarted", this, SendMessageOptions.RequireReceiver);		
	}
		
	void OnTouchDown(int fingerID)
	{
		MainManager.GetInstance().AttachListner(gameObject);
		mFingerID = fingerID;
	}
	
	void OnTouchUp(int fingerID)
	{
		if ( fingerID != mFingerID || Active == false || Visible == false)
		{
			return;
		}
		
	 	_Handler.SendMessage("ButtonPressed", this, SendMessageOptions.RequireReceiver);
		mFingerID = -1;
		MainManager.GetInstance().DetachListener(gameObject);
	}
}
