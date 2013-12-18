using UnityEngine;
using System.Collections;

public class PopupMessage : PopupWindow 
{
	GameObject _PartObject;
	public Transform _TitleTextHandler;
	public Transform _ContentTextHandler;
	
	public void SetContentTextObject(string content)
	{
		SetTextObject(_ContentTextHandler, content, 1.0f);
	}

	public void SetTitleTextObject(string content)
	{
		SetTextObject(_TitleTextHandler, content.ToUpper(), 1.5f);
	}


	void SetTextObject(Transform parent, string content, float scale)
	{
		if ( content == null || content == "" )
		{
			return;
		}

		GameObject textObject_ = TextManager.GetInstance().GetText(content, scale);
		textObject_.transform.parent = parent;
		textObject_.transform.localScale = Vector3.one;
		textObject_.transform.localRotation = Quaternion.identity;
		textObject_.transform.localPosition = Vector3.back * 0.1f;
		Utils.SetLayer(textObject_.transform, gameObject.layer);
	}

}
