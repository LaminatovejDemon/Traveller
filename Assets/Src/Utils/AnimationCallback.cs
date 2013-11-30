using UnityEngine;
using System.Collections;

public class AnimationCallback : MonoBehaviour 
{
	public GameObject _TargetObject;
	public string _TargetMessage;
	public Object _TargetParameter;
	public GameObject _DestroyWhenFinishedObject = null;
	
	void Update () 
	{
		if ( !animation.isPlaying )
		{
			_TargetObject.SendMessage(_TargetMessage, _TargetParameter, SendMessageOptions.RequireReceiver);
			if  (_DestroyWhenFinishedObject != null )
			{
				GameObject.Destroy(_DestroyWhenFinishedObject);
			}
		}		
	}
}
