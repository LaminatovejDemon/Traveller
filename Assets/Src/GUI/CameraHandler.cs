using UnityEngine;
using System.Collections;

public class CameraHandler : MonoBehaviour 
{
	public Camera _RealCamera;
	public Camera _RTTCamera;

	public LayerMask _RTTMask;
	public Transform _RTTCameraTarget;
	RenderTexture _RTTCameraTexture;
	int _RTTCameraLayer;

	public bool _CenterOnObject = false;
	public bool _RenderToTexture;

	bool _RenderToTextureLocal = false;
	
	public float _ViewPortOffset = 0.0f;
	
	GameObject _OnFinished;
	string _OnFinishedMessage;
	
	bool _Initialized = false;

	public static CameraHandler Create()
	{
		CameraHandler instance_ = (Instantiate(Resources.Load("Camera/CameraContainer")) as GameObject).GetComponent<CameraHandler>();

		return instance_;
	}

	void Update()
	{
		if ( _RenderToTextureLocal != _RenderToTexture )
		{
			SetRenderToTexture(_RenderToTexture);
		}
	}

	void SetRenderToTexture(bool state)
	{
		_RenderToTextureLocal = state;

		if ( !state )
		{
			_RealCamera.targetTexture = null;
			_RTTCamera.gameObject.SetActive(false);
			_RealCamera.clearFlags = CameraClearFlags.Depth;
			return;
		}

		if ( _RTTCameraTexture == null )
		{
			_RTTCameraTexture = ((RenderTexture)Object.Instantiate(Resources.Load("Camera/RTTCameraTarget")));
		}

		_RealCamera.targetTexture = _RTTCameraTexture;
		_RTTCamera.gameObject.SetActive(true);
		_RealCamera.clearFlags = CameraClearFlags.Color;
		_RTTCameraTarget.renderer.material.mainTexture = _RTTCameraTexture;
		_RTTCameraTarget.localScale = new Vector3(_RealCamera.aspect, 1, 1);
	}
	
	void Initialize()
	{		
		if ( _Initialized )
		{
			return;
		}
		
		if ( _RealCamera == null )
		{
			_RealCamera = ((GameObject)GameObject.Instantiate(Resources.Load("Camera/RTTSourceCamera"))).camera;
			_RealCamera.name = _RTTCamera.name + "_source";
		}

		_RTTCameraLayer = CalculateLayer(_RTTMask);
		_RTTCamera.cullingMask = _RTTMask;

		_RTTCameraTarget.renderer.material.mainTexture = _RTTCameraTexture;
		_RTTCameraTarget.gameObject.layer = _RTTCameraLayer;

		SetRenderToTexture(_RenderToTexture);

		_Initialized = true;
	}

	int CalculateLayer(int layerMask)
	{
		int ret_ = 0;
		
		while ( (layerMask & 1) == 0 && layerMask != 0)
		{
			ret_++;
			layerMask = layerMask >> 1;
		}
		
		return ret_;
	}
	
	public void Show(Transform targetRoot)
	{		
		Initialize();

		_RealCamera.cullingMask = 
			(1 << targetRoot.gameObject.layer)
			+ (1 << LayerMask.NameToLayer("Default"));

		_RealCamera.gameObject.layer = targetRoot.gameObject.layer;
		_RealCamera.gameObject.SetActive(true);
		
		_RealCamera.enabled = false;
		_RealCamera.enabled = true;
		
		if ( _OnFinished != null )
		{
			_OnFinished.SendMessage(_OnFinishedMessage, SendMessageOptions.RequireReceiver);
		}

		//Refresh main camera
		Camera mainCamera_ = Camera.main;
		mainCamera_.enabled = false;
		mainCamera_.enabled = true;
		Camera.main.clearFlags = CameraClearFlags.Nothing;		
	}
	
	public void Hide()
	{
		Initialize();
		
		gameObject.SetActive(false);
		_RealCamera.gameObject.SetActive(false);
		
		if ( _OnFinished != null )
		{
			_OnFinished.SendMessage(_OnFinishedMessage, SendMessageOptions.RequireReceiver);
		}
	}
	
	public void OnFinished(GameObject target, string message)
	{
		_OnFinished = target;
		_OnFinishedMessage = message;
	}
			
}
