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
		_RTTCamera.cullingMask = _RTTMask;
		_RTTCameraTexture = ((RenderTexture)Object.Instantiate(Resources.Load("Camera/RTTCameraTarget")));
		_RealCamera.targetTexture = _RTTCameraTexture;
		
		//_RealCamera.gameObject.layer = _RTTCameraLayer;

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
		Initialize();
		return _RealCameraLayer;
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
		
		gameObject.SetActive(true);
		
		Utils.SetLayer(targetShip, _RealCameraLayer);
		
		_RealCamera.gameObject.SetActive(true);
		
		
		//Refresh aspect
		_RealCamera.enabled = false;
		_RealCamera.enabled = true;
		
		if ( _OnFinished != null )
		{
			_OnFinished.SendMessage(_OnFinishedMessage, SendMessageOptions.RequireReceiver);
		}
		//MainManager.GetInstance()._BattleCamera.GetComponent<FrameSlider>().SlideIn = true;
		//MainManager.GetInstance()._BattleCamera.GetComponent<FrameSlider>().OnFinished(gameObject, "SlideToBattleFinished");	
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
