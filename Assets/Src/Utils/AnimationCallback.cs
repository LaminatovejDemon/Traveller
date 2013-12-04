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
	
	void Update () 
	{
		if ( !animation.isPlaying && !_Sleeping )
		{
			_EndSleep += Time.time;
			_Sleeping = true;
		}		
		else if ( _Sleeping && Time.time > _EndSleep)
		{
			_TargetObject.SendMessage(_TargetMessage, _TargetParameter, SendMessageOptions.RequireReceiver);
			if  (_DestroyWhenFinishedObject != null )
			{
				GameObject.Destroy(_DestroyWhenFinishedObject);
			}
		}
	}
}
