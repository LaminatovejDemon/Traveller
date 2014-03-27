using UnityEngine;
using System.Collections;

public class ButtonHandler : MonoBehaviour 
{
	public enum ButtonHandle
	{
		HANGAR_ERASE_SHIP,
		HANGAR_OPEN,
		HANGAR_DONE,
		INVENTORY_SLIDER,
		BATTLE_TURN,
		HANGAR_ERASE_ALL,
		CONFIRM,
		CANCEL,
		INVENTORY_CLEANUP,
		POPUP_TRASH_ITEM,
		Invalid,
	};
		
	virtual public void ButtonPressed(Button target)
	{
//		Debug.Log (target._Handle + " is pressed at " + Time.time );
	}
	
	virtual public void ButtonStarted(Button target)
	{
//		Debug.Log (target._Handle + " starts at " + Time.time );
	}
}
