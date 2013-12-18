using UnityEngine;
using System.Collections;

public class PopupConfirm : PopupMessage 
{
	public Button _ConfirmButton;
	public Button _CancelButton;

	public void SetHandler(ButtonHandler handler)
	{
		_ConfirmButton._Handler = handler;
		_CancelButton._Handler = handler;
	}

	protected override void OnTouchUp (int fingerID)
	{
		return;
	}
}
