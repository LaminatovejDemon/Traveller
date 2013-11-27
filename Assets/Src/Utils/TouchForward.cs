using UnityEngine;
using System.Collections;

public class TouchForward : MonoBehaviour 
{
	public Transform _Target;
	
	void OnTouchDown(int fingerID)
	{
		_Target.SendMessage("OnTouchDown",fingerID);
	}
	
	void OnTouchMove(int fingerID)
	{
		_Target.SendMessage("OnTouchMove",fingerID);
	}
	
	void OnTouchUp(int fingerID)
	{
		_Target.SendMessage("OnTouchUp",fingerID);
	}
}
