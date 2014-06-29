using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Part : MonoBehaviour 
{
		
	public enum Location
	{
		Inventory,
		Ship,
		Handle,
		Invalid,
	};
	
	public Location mLocation = Location.Inventory;
	int mDragFingerID = -1;
	Vector3 mDragBeginPosition;
	GameObject mHandleShadow = null;
	public float mHP = -1;

	List<Transform> _SignalLights = new List<Transform>();
	List<Transform> _RotationRoots = new List<Transform>();
	List<Transform> _ParticleRoots = new List<Transform>();
	Material _SignalLight;
	Material _SignalGlow;
	GameObject _SignalGlowTemplate;
	
	Transform _DisabledContainer;
	
	/*public Transform DisabledContainer
	{
		get 
		{
			if ( _DisabledContainer == null )
			{
				_DisabledContainer = new GameObject().transform;
				_DisabledContainer.transform.parent = transform;
				_DisabledContainer.transform.localPosition = Vector3.zero;
				_DisabledContainer.name = name + "_Disabled";
				_DisabledContainer.gameObject.SetActive(false);
			}
			return _DisabledContainer;
		}
		set 
		{
			_DisabledContainer = value;
		}
	}*/

	bool _Powered = true;

	public void SetPoweredLayer(int layer)
	{
		for ( int i = 0; i < _SignalLights.Count; ++i )
		{
			Utils.SetLayer(_SignalLights[i], layer);
		}
	}

	public void SetPowered(bool state)
	{
		if ( _Powered == state )
		{
			return;
		}

		_Powered = state;

		SetSignalLightsColor();
	}
		
	
	bool mInitialized = false;
	
	void Initialize()
	{
		if ( mInitialized )
		{
			return;
		}
		
		mHP = mPattern.mHp;
			
//		CreateDisabledContainer(transform);

		_SignalLight = Object.Instantiate(Resources.Load("Materials/SignalLight")) as Material;
		_SignalGlowTemplate = Resources.Load("Content/SignalLight") as GameObject;
		_SignalGlow = Object.Instantiate(Resources.Load("Content/Materials/SignalGlow")) as Material;

		SetRoots(ref _SignalLights, "power", transform);
		SetRoots(ref _ParticleRoots, "particle", transform);
		SetRoots(ref _RotationRoots, "turret", transform);
		SetRoots(ref _RotationRoots, "turningXYZ", transform);

		mInitialized = true;

		// needs to be after an initialization
		SetSignalLights();
	}

	void SetSignalLights()
	{
		GameObject glow_;
		for ( int i = 0; i < _SignalLights.Count; ++i )
		{
			_SignalLights[i].renderer.material = _SignalLight;
			glow_ = GameObject.Instantiate(_SignalGlowTemplate) as GameObject;
			glow_.transform.parent = _SignalLights[i];
			glow_.transform.localPosition = _SignalGlowTemplate.transform.localPosition;
			glow_.transform.localRotation = _SignalGlowTemplate.transform.localRotation;
			glow_.transform.localScale = _SignalGlowTemplate.transform.localScale;
			glow_.gameObject.layer = glow_.transform.parent.gameObject.layer;
			glow_.renderer.material = _SignalGlow;
		}

		SetSignalLightsColor();
	}

	void SetSignalLightsColor()
	{
		if ( !mInitialized )
		{
			return;
		}

		int tier_ = (int)(mPattern.mRarity / 400.0f * 10.0f);
		_SignalLight.SetColor("_TintColor", GetColor(tier_, 0.7f));
		_SignalGlow.SetColor("_TintColor", GetColor(tier_, 0.4f));

		_LightBlinkTimeStamp = Random.value + Time.time;
	}

	Color GetColor(int tier, float alpha)
	{
		Color ret_ = Color.magenta;

		if ( !_Powered )
		{
			ret_ = new Color(1.0f, 0.1f, 0.1f, 1f);
		}
		else switch (tier)
		{
		case 0:
			ret_ = new Color(243.0f / 255.0f ,244.0f / 255.0f, 161.0f / 255.0f, 0.7f);
			break;
		case 1:
			ret_ = new Color(255.0f / 255.0f ,255.0f / 255.0f, 255.0f / 255.0f, 0.85f);
			break;
		case 2:
			ret_ = new Color(67.0f / 255.0f ,212.0f / 255.0f, 206.0f / 255.0f, 0.8f);
			break;
		case 3:
			ret_ = new Color(22.0f / 255.0f ,123.0f / 255.0f, 195.0f / 255.0f, 1f);
			break;
		case 4:
			ret_ = new Color(0.0f / 255.0f ,48.0f / 255.0f, 225.0f / 255.0f, 1f);
			break;
		case 5:
			ret_ = new Color(86.0f / 255.0f ,55.0f / 255.0f, 249.0f / 255.0f, 1f);
			break;
		case 6:
			ret_ = new Color(54.0f / 255.0f ,0.0f / 255.0f, 189.0f / 255.0f, 1f);
			break;
		case 7:
			ret_ = new Color(157.0f / 255.0f ,0.0f / 255.0f, 183.0f / 255.0f, 1f);
			break;
		default:
			ret_ = new Color(255.0f / 255.0f ,24.0f / 255.0f, 210.0f / 255.0f, 1f);
			break;
		}
		ret_.a *= alpha;
		return ret_;
	}

	void SetRoots(ref List<Transform> target, string key, Transform parent)
	{
		for ( int i = 0; i < parent.childCount; ++i )
		{
			SetRoots(ref target, key, parent.GetChild(i));

			if ( parent.GetChild(i).name.Contains(key) )
			{
				target.Add(parent.GetChild(i));
			}
		}
	}
	
/*	void CreateDisabledContainer(Transform parent)
	{
		for ( int i = 0; i < parent.childCount; ++i )
		{
			CreateDisabledContainer(parent.GetChild(i));
		}
		
		if ( parent.collider != null )
		{
			GameObject new_ = (GameObject)GameObject.Instantiate((Resources.Load("Content/DisabledPart")));
			new_.transform.parent = parent;
			new_.transform.localPosition = new Vector3(-0.5f, 0.6f, -0.5f);
			new_.transform.localRotation = Quaternion.identity;
			new_.transform.parent = DisabledContainer;	
			//DisabledContainer.transform.localPosition = new Vector3(-0.005f, 0.005f, 0.006f);
		}
	}*/
	
	public PartManager.Pattern mPattern;

	// Use this for initialization
	void Start () 
	{
	 	Initialize();
	}

	float _LightBlinkTimeStamp;

	// Update is called once per frame
	void Update () 
	{	
		UpdateLightBlinking();
	}

	void UpdateLightBlinking()
	{
		if ( Time.time - _LightBlinkTimeStamp > 0 )
		{
			Color signalColor_ = _SignalLight.GetColor("_TintColor");
			
			signalColor_.a = 1.0f - signalColor_.a;
			
			_SignalLight.SetColor("_TintColor", signalColor_);
			
			signalColor_.a *= 0.5f;
			_SignalGlow.SetColor("_TintColor", signalColor_);

			if ( _Powered )
			{
				_LightBlinkTimeStamp = Time.time + (signalColor_.a > 0.2f ? 4.0f + Random.value * 2.0f : 1.0f);
			}
			else
			{
				_LightBlinkTimeStamp = Time.time + 0.2f;
			}
		}
	}
	
	public Vector3 GetGunPoint()
	{
		return this.transform.position + this.transform.parent.rotation * new Vector3(-0.5f, 1.0f, -0.5f);
	}
	
	void DestroyComponent<T>(Transform parent)
	{
		for ( int i = 0; i < parent.childCount; ++i )
		{
			DestroyComponent<T>(parent.GetChild(i));
		}
		
		GameObject.Destroy( parent.GetComponent(typeof(T)) );
	}
	
	void SetMaterial(Transform parent, Material mat)
	{
		for ( int i = 0; i < parent.childCount; ++i )
		{
			SetMaterial(parent.GetChild(i), mat);
		}
		
		Renderer rend_ = parent.GetComponent<Renderer>();
		
		if ( rend_ != null )
		{
			rend_.material = mat;
		}
	}
	
	GameObject CreateShadow()
	{
		GameObject ret_ = (GameObject)GameObject.Instantiate(gameObject);
		
		GameObject.Destroy(ret_.GetComponent<Part>());
		
		DestroyComponent<BoxCollider>(ret_.transform);
		DestroyComponent<TouchForward>(ret_.transform);
		Material shadowMat_ = (Material)Resources.Load("ShadowMaterial", typeof(Material));
		SetMaterial(ret_.transform, shadowMat_);
		
		return ret_;
	}

	Location _TouchLocation;
	float _TouchLocationTimeStamp;
	Vector3 _TouchLocationPosition;
	
	void SetTouchLocation(Location at, Vector3 fingerPos)
	{
		_TouchLocation = at;
		_TouchLocationTimeStamp = Time.time;
		_TouchLocationPosition = fingerPos;
	}
	
	void ResetTouchLocation()
	{
		_TouchLocation = Location.Invalid;
		_TouchLocationTimeStamp = -1;
	}
	
	void CheckTouchLocation(Vector3 fingerPos)
	{
		if ( _TouchLocationTimeStamp == -1 )
		{
		//	return;
		}

		Vector3 fingerDelta = (fingerPos - _TouchLocationPosition);
		fingerDelta.x = Mathf.Abs(fingerDelta.x);
		fingerDelta.y = Mathf.Abs(fingerDelta.y);
		fingerDelta.z = Mathf.Abs(fingerDelta.z);

		if ( fingerDelta.x > GetDragThreshold() || _TouchLocation == Location.Ship ) 
		{
			DragBegin();
		}
		else if ( fingerDelta.y > GetDragThreshold() )
		{
			ResetTouchLocation();
		}

	}

	float GetDragThreshold()
	{
		float ret_ = 30.0f;
		return ret_;
	}

	void CheckClick(Vector3 position)
	{
		if ( _TouchLocationTimeStamp == -1 )
		{
			return;
		}

		if ( (position - _TouchLocationPosition).magnitude < GetDragThreshold() && Time.time - _TouchLocationTimeStamp < 0.4f )
		{
			Debug.Log ("Click");
			PopupManager.GetInstance().CreatePartPopup(gameObject, mPattern);
			ResetTouchLocation();
		}
	}
	
	bool _Dragging = false;
	
	void DragBegin()
	{
		if ( _Dragging )
		{
			return;
		}
		
		switch ( mLocation )
		{
			case Location.Inventory:
				InventoryManager.Instance.RetrievePart(transform);
			break;
			
			case Location.Ship:
				FleetManager.GetShip().RetrievePart(transform);
			break;
			
			default:
			break;
		}
		
		//DisabledContainer.gameObject.SetActive(false);	
		SetPowered(true);

		mDragBeginPosition = MainManager.Instance.GetWorldPos(mDragFingerID) - transform.localPosition;
		mLocation = Location.Handle;

		if ( mHandleShadow == null )
		{
			mHandleShadow = CreateShadow();
		}
		
		mHandleShadow.transform.parent = null;
		mHandleShadow.SetActive(true);
		
		_Dragging = true;
	}
	
	void DragMove()
	{
		if ( !_Dragging )
		{
			return;
		}
		
		Vector3 localPosition_ = MainManager.Instance.GetWorldPos(mDragFingerID) + Vector3.forward * 1.5f + Vector3.left * 0.5f + Vector3.up * 1.0f - mDragBeginPosition;
		transform.localPosition = localPosition_;
		
		
		Vector3 targetPosition_ = localPosition_ - HangarManager.Instance._HangarContainer.transform.localPosition;
		
		Vector3 placementPostiion = mHandleShadow.transform.position;
		
		if ( !FleetManager.GetShip().IsOccupied(mPattern.mHash, targetPosition_, ref placementPostiion) )
		{
			mHandleShadow.transform.position = placementPostiion + HangarManager.Instance._HangarContainer.transform.localPosition;
		}
	}
	
	void DragEnd()
	{
		if ( !_Dragging )
		{
			return;
		}
		
		ResetTouchLocation();
		
		transform.position = mHandleShadow.transform.position;
		
		mHandleShadow.SetActive(false);
		mHandleShadow.transform.parent = transform;
		UpdateLocation(InventoryManager.Instance._ScrollingPanel.IsWithin(MainManager.Instance.GetWorldPos(mDragFingerID)) ? Location.Inventory : Location.Ship);
		
		_Dragging = false;
	}
	
	void OnTouchDown(int fingerID)
	{
		if ( mDragFingerID != -1 )
		{
			return;
		}

		MainManager.Instance.AttachListner(gameObject);
		
		mDragFingerID = fingerID;
		SetTouchLocation(mLocation, MainManager.Instance.GetScreenPos(fingerID));	
	}
	
	int mBoundaryHeight = 0;
	
	public void SetHeight(int height)
	{
		mBoundaryHeight = height;
	}
	
	public int GetHeight()
	{
		return mBoundaryHeight;
	}
	
	void OnTouchMove(int fingerID)
	{
		if ( mDragFingerID != fingerID )
		{
			return;
		}
		
		if ( _Dragging )
		{
			DragMove ();
		}
		else
		{
			CheckTouchLocation(MainManager.Instance.GetScreenPos(fingerID));
		}
	}
	
	void OnTouchUp(int fingerID)
	{		
		if ( mDragFingerID != fingerID )
		{
			return;
		}
		MainManager.Instance.DetachListener(gameObject);
		
		
		if ( _Dragging )
		{
			DragEnd();
		}
		else
		{
			CheckClick(MainManager.Instance.GetScreenPos(fingerID));
		}
		
		ResetTouchLocation();
		
		mDragFingerID = -1;
	}
	
	public void UpdateLocation(Location newLocation_)
	{
		if ( newLocation_ == mLocation )
		{
			return;
		}

		mLocation = newLocation_;
		
		switch (mLocation)
		{
			case Location.Ship:
				FleetManager.GetShip().InsertPart(transform);
				break;
			case Location.Inventory:
				InventoryManager.Instance.InsertPart(gameObject);
				break;
		}
	}
}
