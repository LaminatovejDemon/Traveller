using UnityEngine;
using System.Collections;

public class AnimationCallback : MonoBehaviour 
{
	public GameObject _TargetObject;
	public string _TargetMessage;
	public Object _TargetParameter;
	public GameObject _DestroyWhenFinishedObject = null;
	public float _EndSleep = 0.0f;
	bool _Sleeping = false;
	bool _MessageSent = false;

	public void Reset()
	{
		_DestroyWhenFinishedObject = null;
		_EndSleep = 0.0f;
		_Sleeping = false;
		_MessageSent = false;
	}

	void Update () 
	{
		if ( _MessageSent )
		{
			return;
		}

		if ( !animation.isPlaying && !_Sleeping )
		{
			_EndSleep += Time.time;
			_Sleeping = true;
		}		
		else if ( _Sleeping && Time.time > _EndSleep)
		{
			Debug.Log ("Sending message " + _TargetMessage + "  by " + _TargetParameter);
			_TargetObject.SendMessage(_TargetMessage, _TargetParameter, SendMessageOptions.RequireReceiver);
			if  (_DestroyWhenFinishedObject != null )
			{
				Debug.Log ("Destroying " + _DestroyWhenFinishedObject);
				GameObject.Destroy(_DestroyWhenFinishedObject);
			}

			_MessageSent = true;
		}
	}
}
