using UnityEngine;
using System.Collections;

public class TouchForward : MonoBehaviour 
{
	public Transform _Target;
	
	void OnTouchDown(int fingerID)
	{
		if ( _Target == transform )
		{
			return;
		}

		_Target.SendMessage("OnTouchDown",fingerID);
	}
	
	void OnTouchMove(int fingerID)
	{
		if ( _Target == transform )
		{
			return;
		}

		_Target.SendMessage("OnTouchMove",fingerID);
	}
	
	void OnTouchUp(int fingerID)
	{
		if ( _Target == transform )
		{
			return;
		}

		_Target.SendMessage("OnTouchUp",fingerID);
	}
}
