using UnityEngine;
using System.Collections;

public class PopupPart : PopupWindow
{
	GameObject _PartObject;
	GameObject _PartObjectOriginal;
	public Transform _PartHandler;
	public Transform _TitleTextHandler;
	public Transform _ContentTextHandler;
	public Transform _HeaderHandler;

	public void SetPartObject(GameObject partResource, PartManager.Pattern pattern)
	{
		_PartObjectOriginal = partResource;

		GameObject.Destroy(_PartObject);

		_PartObject = (GameObject)GameObject.Instantiate(partResource);
		_PartObject.GetComponent<Part>().mPattern = pattern;

		_PartObject.transform.parent = _PartHandler;
		_PartObject.transform.localRotation = Quaternion.AngleAxis(270, Vector3.right) * partResource.transform.localRotation;
		_PartObject.transform.localPosition = Vector3.zero;
		Utils.SetLayer(_PartObject.transform, gameObject.layer);
		Utils.SetColissionEnabled(_PartObject.transform, false);
	}

	public override void ButtonPressed (Button target)
	{
		if ( target._Handle == ButtonHandle.POPUP_TRASH_ITEM )
		{
			InventoryManager.Instance.RetrievePart(_PartObjectOriginal.transform);
			GameObject.Destroy(_PartObjectOriginal);
			GameObject.Destroy(gameObject);
		}
	}

	public void SetHeaderTextObject(string content)
	{
		SetTextObject(_HeaderHandler, content, 0.6f);
	}

	public void SetContentTextObject(string content)
	{
		SetTextObject(_ContentTextHandler, content, 0.8f);
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
