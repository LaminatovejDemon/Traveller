using UnityEngine;
using System.Collections;

public class TouchForward : MonoBehaviour 
{
	void OnTouchDown(int fingerID)
	{
		GameObject parent_ = FindParent(transform);
		
		if ( parent_ != null )
		{
			parent_.SendMessage("OnTouchDown",fingerID);
		}	
	}
	
	void OnTouchMove(int fingerID)
	{
		GameObject parent_ = FindParent(transform);
		
		if ( parent_ != null )
		{
			parent_.SendMessage("OnTouchMove",fingerID);
		}	
	}
	
	void OnTouchUp(int fingerID)
	{
		GameObject parent_ = FindParent(transform);
		
		if ( parent_ != null )
		{
			parent_.SendMessage("OnTouchUp",fingerID);
		}	
	}
	
	GameObject FindParent(Transform child)
	{
		if ( child == null )
		{
			return null;
		}
			
		if ( child.GetComponent<Part>() != null )
		{
			return child.gameObject;
		}
		
		return FindParent(child.parent);
	}
}
