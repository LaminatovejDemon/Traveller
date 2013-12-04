using UnityEngine;
using System.Collections;

public class CameraHandler : MonoBehaviour 
{
	public Camera _RealCamera;
	public LayerMask _RealCameraMask;
	int _RealCameraLayer;

	public Camera _RTTCamera;
	public LayerMask _RTTMask;
	public Transform _RTTCameraTarget;
	RenderTexture _RTTCameraTexture;
	int _RTTCameraLayer;
	
	public bool _FillScreen = false;
	public bool _CenterOnObject = false;
	public float _ViewPortOffset = 0.0f;
	
	GameObject _OnFinished;
	string _OnFinishedMessage;
	
	bool _Initialized = false;
	
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
		_RealCameraLayer = CalculateLayer(_RealCameraMask);
		_RTTCameraLayer = CalculateLayer(_RTTMask);
		
		_RealCamera.cullingMask = _RealCameraMask;
		Utils.SetLayer(_RealCamera.transform, _RealCameraLayer);
		_RTTCamera.cullingMask = _RTTMask;
		
		if ( Utils.RenderToTexture )
		{	
			_RTTCameraTexture = ((RenderTexture)Object.Instantiate(Resources.Load("Camera/RTTCameraTarget")));
			_RealCamera.targetTexture = _RTTCameraTexture;
		}
		else if ( !_FillScreen )
		{
			Rect cameraRect_ = _RealCamera.rect;
			cameraRect_.width = 0.5f;
			cameraRect_.x = _ViewPortOffset;
			_RealCamera.rect = cameraRect_;
		}
		
		
		if ( Utils.RenderToTexture )
		{
			_RealCamera.gameObject.layer = _RTTCameraLayer;
		}			

		_RTTCameraTarget.renderer.material.mainTexture = _RTTCameraTexture;
		_RTTCameraTarget.gameObject.layer = _RTTCameraLayer;
		
		if ( _FillScreen )
		{
			_RTTCameraTarget.transform.localScale = Vector3.one * _RTTCamera.aspect;
			_RTTCameraTarget.renderer.material.mainTextureScale = Vector3.one * _RTTCamera.aspect;
			_RTTCameraTarget.renderer.material.mainTextureOffset = - Vector3.one * ((_RTTCamera.aspect-1) * 0.5f);
		}
		
		_Initialized = true;
	}
	
	public int GetRealLayer()
	{
		if ( Utils.RenderToTexture )
		{
			Initialize();
			return _RealCameraLayer;
		}
		else
		{
			return 0;
		}
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
	
	public void Show(Transform targetShip)
	{		
		Initialize();
		
		if ( Utils.RenderToTexture )
		{
			gameObject.SetActive(true);
		}
		
		Utils.SetLayer(targetShip, _RealCameraLayer);
		
		_RealCamera.gameObject.SetActive(true);
		
		
		//Refresh aspect
		_RealCamera.enabled = false;
		_RealCamera.enabled = true;
		
		if ( _OnFinished != null )
		{
			_OnFinished.SendMessage(_OnFinishedMessage, SendMessageOptions.RequireReceiver);
		}
		
		if ( _CenterOnObject )
		{
			_RealCamera.transform.position = targetShip.transform.position + _RealCamera.transform.rotation * (Vector3.back * 5.0f + Vector3.up * 1.0f);
		}
		
		//Refresh main camera
		Camera mainCamera_ = Camera.main;
		mainCamera_.enabled = false;
		mainCamera_.enabled = true;
		Camera.main.clearFlags = CameraClearFlags.Nothing;
		
	//	MainManager.GetInstance()._BattleCamera.GetComponent<FrameSlider>().SlideIn = true;
	//	MainManager.GetInstance()._BattleCamera.GetComponent<FrameSlider>().OnFinished(gameObject, "SlideToBattleFinished");	
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
