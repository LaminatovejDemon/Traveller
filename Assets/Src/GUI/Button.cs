using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour 
{
	public ButtonHandler _Handler;
	public ButtonHandler.ButtonHandle _Handle;
	public TextMesh _TextHandle;
	public GameObject _VisibilityContainer;
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
		_TextHandle.text = _Caption;
		_Handler.SendMessage("ButtonStarted", this, SendMessageOptions.RequireReceiver);		
	}
		
	void OnTouchDown(int fingerID)
	{
		MainManager.GetInstance().AttachListner(gameObject);
		mFingerID = fingerID;
	}
	
	void OnTouchUp(int fingerID)
	{
		if ( fingerID != mFingerID )
		{
			return;
		}
		
	 	_Handler.SendMessage("ButtonPressed", this, SendMessageOptions.RequireReceiver);
		mFingerID = -1;
		MainManager.GetInstance().DetachListener(gameObject);
	}
}
