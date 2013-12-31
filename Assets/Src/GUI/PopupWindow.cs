using UnityEngine;
using System.Collections;

public class PopupWindow : MonoBehaviour 
{
	public BoxCollider _BackgroundCollider;
	public float _Depth;
	public Renderer _TintBackgroundResource;
	Renderer _TintBackgroundInstance;

	bool _Initialized = false;

	void Start()
	{
		Initialize();
	}

	public void Initialize()
	{
		if ( _Initialized )
		{
			return;
		}

		Camera targetCamera_ = MainManager.GetInstance()._GUICamera;

		_BackgroundCollider.size = new Vector3(targetCamera_.orthographicSize * targetCamera_.aspect * 2.0f, targetCamera_.orthographicSize * 2.0f, 0.1f);

		if ( _BackgroundCollider.transform != transform )
		{
			_BackgroundCollider.gameObject.AddComponent<TouchForward>()._Target = transform;
		}

		transform.parent = targetCamera_.transform;
		gameObject.layer = transform.parent.gameObject.layer;
		transform.localRotation = Quaternion.identity;
		transform.localPosition = Vector3.back * _Depth;

		_TintBackgroundInstance = ((GameObject)GameObject.Instantiate(_TintBackgroundResource.gameObject)).renderer;
		Vector3 scale_ = _BackgroundCollider.size;
		Vector3 orientedScale_ = new Vector3(scale_.x, scale_.z, scale_.y);
		_TintBackgroundInstance.transform.parent = transform;
		_TintBackgroundInstance.transform.localPosition = Vector3.zero;
		_TintBackgroundInstance.transform.localEulerAngles = new Vector3(270,0,0);
		_TintBackgroundInstance.transform.localScale =  Utils.ScalarDivide(orientedScale_, _TintBackgroundInstance.bounds.extents * 1.62f);
		_TintBackgroundInstance.gameObject.layer = gameObject.layer;

		_Initialized = true;
	}

	int mFingerID;

	void OnTouchDown(int fingerID)
	{
		MainManager.GetInstance().AttachListner(gameObject);
		mFingerID = fingerID;
	}

	protected virtual void OnTouchUp(int fingerID)
	{
		if ( fingerID != mFingerID )
		{
			return;
		}

		GameObject.Destroy(gameObject);
	}
}
