using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ship : MonoBehaviour 
{
	
	
	
//	private static Ship mInstance = null;
	private bool [,] mOccupied = new bool[HangarManager.HANGAR_SIZE, HangarManager.HANGAR_SIZE];
//	private bool [,] mOccupiedNormalized = new bool[HANGAR_SIZE, HANGAR_SIZE];
	private GameObject mStats = null;
	
	private float mEnergyConsumption = 0;
	private float mCreditCost = 0;
	private float mShieldCapacity = 0;
	private float mShieldRecharge = 0;
	private float mWeaponDamage = 0;
	private float mMass = 0;
	private float mEnginePower = 0;
	private bool mValidShip = false;
	private Vector3 mShipCenter = Vector3.zero;
	
	public struct Weapon
	{
		public Weapon(Part which)
		{
			mObject = which;
		}
		
		public Part mObject;
	};
	
	public List<Weapon> mWeaponList {get; private set;}
	
/*	public static Ship GetInstance()
	{
		if ( mInstance == null )
		{
			GameObject instanceObject_ = GameObject.Find("#Ship");
			if ( instanceObject_ != null )
				mInstance = instanceObject_.transform.GetComponent<Ship>();
			else
				mInstance =  new GameObject("#Ship").AddComponent<Ship>();
		}
		return mInstance;
	}*/
	
	public string mShipName {get; private set;}
	bool mInitialized = false;
	
	
	
	void ClearHangar()
	{
		for ( int i = 0; i < HangarManager.HANGAR_SIZE; ++i )
		{
			for ( int j = 0; j < HangarManager.HANGAR_SIZE; ++j )
			{
				mOccupied[i,j] = false;
			}
		}
	}
	
	/*public int GetRange(int direction)
	{
		float left, right, top, bottom;
		
		GetBoundary(out top, out bottom, out left, out right, true);
		
		if ( (direction & 0) != 0 ) //top down
		{
			return (int)(left - right);
		}
		else //left right
		{
			return (int)(top - bottom);
		}
	}*/
	
	public void Initialize(FleetManager.ShipScan template)
	{
		if ( mInitialized )
		{
			return;
		}
		
		mWeaponList = new List<Weapon>();
		
		ClearHangar();
		
		LoadShip(template);
		mInitialized = true;
	}
	
	Transform _ShipContainer;
		
	public void Initialize(string name)
	{
		if ( mInitialized )
		{
			return;
		}
		
		_ShipContainer = new GameObject("ShipContainer").transform;
		_ShipContainer.transform.localPosition = Vector3.zero;
		_ShipContainer.transform.localRotation = Quaternion.identity;
		
		transform.parent = _ShipContainer;
			
		mWeaponList = new List<Weapon>();
		
		mShipName = name;
		
		ClearHangar();
				
		RestoreShip();
		mInitialized = true;
	}
		
	
	public void RetrievePart(Transform which)
	{
		
		Occupy(which.GetComponent<Part>(), which.transform.localPosition, false);
		which.parent = null;
		BackupShip();
	}
	
	public void InsertPart(Transform which)
	{
		which.parent = transform;
		Occupy(which.GetComponent<Part>(), which.transform.localPosition, true);
		BackupShip();
	}
	
	public void LoadShip(FleetManager.ShipScan template)
	{
		mShipName = template.mName;
		
		if (PlayerPrefs.GetInt(template.mName+"_ShipItemCount") > 0 )
		{
			RestoreShip();
		}
		else
		{
			int itemCount_ = template.mPartList.Count;
			
			for ( int i = 0; i < itemCount_; ++i )
			{
				GameObject part_ = PartManager.GetInstance().GetPattern(template.mPartList[i].mPattern.mID);
				part_.transform.parent = transform;
				
				part_.transform.localPosition = template.mPartList[i].mPosition;
				part_.GetComponent<Part>().mLocation = Part.Location.Ship;
				Occupy(part_.GetComponent<Part>(), part_.transform.position, true);
				
			}
		}
	}
	
	public void RestoreShip()
	{
//		Debug.Log ("Restoring Ship" + mShipName);
		int itemCount_ = PlayerPrefs.GetInt(mShipName+"_ShipItemCount");
		
		for ( int i = 0; i < itemCount_; ++i )
		{
			GameObject part_ = PartManager.GetInstance().GetPattern(PlayerPrefs.GetString(mShipName+"_ShipPartID_"+i));
			
			InventoryManager.GetInstance().AddCaption(part_.GetComponent<Part>());
			part_.GetComponent<Part>().SetCaptionVisibility(false);
			
			part_.transform.parent = transform;
			
			float x_ = PlayerPrefs.GetFloat(mShipName+"_ShipPartPosX_"+i);
			float y_ = PlayerPrefs.GetFloat(mShipName+"_ShipPartPosY_"+i);
			float z_ = PlayerPrefs.GetFloat(mShipName+"_ShipPartPosZ_"+i);
			
			
			part_.transform.localPosition = new Vector3(x_,y_,z_);
			
			part_.GetComponent<Part>().mLocation = Part.Location.Ship;
			Occupy(part_.GetComponent<Part>(), part_.transform.localPosition, true);
			
		}
		
		CalculateShipCenter();
	}
	
	void BackupShip()
	{
		PlayerPrefs.SetInt(mShipName+"_ShipItemCount", transform.childCount);
		for ( int i = 0; i<transform.childCount; ++i )
		{
			PlayerPrefs.SetString(mShipName+"_ShipPartID_"+i,transform.GetChild(i).GetComponent<Part>().mPattern.mID);
			PlayerPrefs.SetFloat(mShipName+"_ShipPartPosX_"+i,transform.GetChild(i).localPosition.x);
			PlayerPrefs.SetFloat(mShipName+"_ShipPartPosY_"+i,transform.GetChild(i).localPosition.y);
			PlayerPrefs.SetFloat(mShipName+"_ShipPartPosZ_"+i,transform.GetChild(i).localPosition.z);
		}
	}
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	void GetHangarPlace(Vector3 position, out int x, out int y)
	{
		x = (int)(((int)(position.x + 1000.5f))-1000 + ((HangarManager.HANGAR_SIZE) * 0.5f -1));
		y = (int)(((int)(position.z + 1000.5f))-1000 + ((HangarManager.HANGAR_SIZE) * 0.5f -1)); 
	}
	
	Vector3 GetPosition(int x, int y)
	{
		Vector3 ret_ = new Vector3(x - ((HangarManager.HANGAR_SIZE) * 0.5f -1),0, y - ((HangarManager.HANGAR_SIZE) * 0.5f - 1));
		
		return ret_;
	}
	
	public bool IsOccupied(int hash, Vector3 position, ref Vector3 shadowPosition)
	{
		int sx_, sy_;
		GetHangarPlace(shadowPosition, out sx_, out sy_);
		
		int x_, y_;
		GetHangarPlace(position, out x_, out y_);
		
		if ( !IsOccupiedLocalRange(hash, x_, y_, ref sx_, ref sy_) )
		{
			shadowPosition = GetPosition(sx_,sy_);
			return false;
		}
		
		return true;
		
	}
	
	bool IsOccupiedLocalRange(int hash, int px, int py, ref int sx, ref int sy)
	{
		int incX_ = px < sx ? 1 : -1;
		int incY_ = py < sy ? 1 : -1;
		
		bool free_ = false;
		
		for ( int x_ = px; (incX_ > 0 ? x_ <= sx : x_ >= sx); x_ += incX_ )
		{
			for ( int y_ = py; (incY_ > 0 ? y_ <= sy : y_ >= sy); y_ += incY_ )
			{
				if ( !IsOccupied(hash, x_, y_) )
				{
					sx = x_;
					sy = y_;
					return false;
				}
			}
		}
		
		return true;
	}
	
	
		
	float GetEvade()
	{
		if ( mMass <= 0 || mEnginePower <= 0 )
		{
			return -1;
		}
		
		return mEnginePower / (mMass+mEnginePower);
	}
	
	public void EraseShip()
	{
		while ( transform.childCount > 0 )
		{
			Transform destroy_ = transform.GetChild(0);
			RetrievePart(destroy_);
			GameObject.Destroy(destroy_.gameObject);
		}
	}
	
	void UpdateStats()
	{
		if ( mStats == null )
		{
			mStats = TextManager.GetInstance().GetText("", 0.6f);
			mStats.GetComponent<TextMesh>().alignment = TextAlignment.Right;
			mStats.GetComponent<TextMesh>().anchor = TextAnchor.UpperRight;
			mStats.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1,1,0)) + Vector3.down * 4.0f;
			mStats.transform.rotation = Camera.main.transform.rotation;
		}
		
		
		mStats.GetComponent<TextMesh>().text = 
			"PRICE: " + mCreditCost + 
			"\nENERGY: " + mEnergyConsumption +
			"\n MONKEY DAMAGE: " + mWeaponDamage + 
			"\n SHIELD CAPACITY: " + mShieldCapacity + 
			"\n SHIELD RECHARGE: " + mShieldRecharge +
			"\n MASS: " + mMass +
			"\n ENGINE POWER: " + mEnginePower +
			"\n EVADE: " + (GetEvade() == -1 ? "N/A" : (int)(GetEvade() * 100) + "%") + 
			"\n IT'S " + (mValidShip ? "VALID SHIP" : "PIECE OF CRAP");
	}
	
	void SetStats(Part part, bool addition)
	{
		mCreditCost += addition ? part.mPattern.mPrice : -part.mPattern.mPrice;
		mEnergyConsumption += addition ? part.mPattern.mPower : -part.mPattern.mPower;
		
		PartManager.Ability new_ = part.mPattern.GetAbility(PartManager.AbilityType.Beam);
		if ( new_ != null )
		{
			float damage_ = (addition ? 1 : -1 ) * new_.mValue;
			new_ = part.mPattern.GetAbility(PartManager.AbilityType.BeamRepeater);
			if ( new_ != null )
			{
				damage_ *= new_.mValue;
			}
			mWeaponDamage += damage_;
			mWeaponList.Add(new Weapon(part));
		}
		new_ = part.mPattern.GetAbility(PartManager.AbilityType.Phaser);
		if ( new_ != null )
		{
			mWeaponDamage += (addition ? 1 : -1 ) * new_.mValue;
			mWeaponList.Add(new Weapon(part));
		}
		new_ = part.mPattern.GetAbility(PartManager.AbilityType.Tracer);
		if ( new_ != null )
		{
			mWeaponDamage += (addition ? 1 : -1 ) * new_.mValue;
			mWeaponList.Add(new Weapon(part));
		}
		
		new_ = part.mPattern.GetAbility(PartManager.AbilityType.ShieldCapacity);
		if ( new_ != null )
			mShieldCapacity += (addition ? 1 : -1 ) * new_.mValue;
		
		new_ = part.mPattern.GetAbility(PartManager.AbilityType.ShieldRecharge);
		if ( new_ != null )
			mShieldRecharge += (addition ? 1 : -1 ) * new_.mValue;
		
		new_ = part.mPattern.GetAbility(PartManager.AbilityType.EnginePower);
		if ( new_ != null )
			mEnginePower += (addition ? 1 : -1 ) * new_.mValue;
		
		mMass += (addition ? 1 : -1 ) * part.mPattern.mWeight;
		
		bool lastValidity_ = mValidShip;
		mValidShip = (mEnginePower > 0 && mEnergyConsumption >= 0);
		
		if ( mValidShip != lastValidity_ )
		{
			HangarManager.GetInstance().InformShipValidity(mValidShip);
		}
		
		UpdateStats();
	}
	
	bool DebugRotate_ = false;
	
	public void SetHangarEntry(bool state)
	{
		if ( mStats != null )
		{
			mStats.SetActive(state);
		}
		
		if ( !state )
		{
			_ShipContainer.parent = null;
			CalculateShipCenter();
			transform.localPosition = mShipCenter;
			_ShipContainer.localPosition = Vector3.zero;
			FleetManager.GetInstance().ScanShip(gameObject);
			DebugRotate_ = true;
		}
		else
		{	
			DebugRotate_ = false;
			_ShipContainer.parent = HangarManager.GetInstance()._HangarContainer.transform;
			transform.localPosition = Vector3.zero;
			_ShipContainer.localPosition = Vector3.zero;		
			_ShipContainer.localRotation = Quaternion.identity;
		}
	}
	
	
	void GetBoundary(out float top, out float bottom, out float left, out float right)
	{
		top=-999;
		bottom=999;
		left=999; 
		right=-999;
		
		for ( int x = 0; x < HangarManager.HANGAR_SIZE; ++x )
		{
			for ( int y = 0; y < HangarManager.HANGAR_SIZE; ++y )
			{
				if ( mOccupied[x,y] )
				{
					if ( x > right )
						right = x;
					if ( x < left )
						left = x;
					if ( y < bottom )
						bottom = y;
					if ( y > top )
						top = y;
				}
			}
		}
	}	
	
	public void CalculateShipCenter()
	{		
		float top_, bottom_,left_,right_;
		
		GetBoundary(out top_, out bottom_, out left_, out right_);
		
		mShipCenter = new Vector3((int)( (right_ - left_) * 0.5f), 0, (int)( ( top_ - bottom_) * 0.5f));
		Debug.Log ("Ship Center is " + mShipCenter + " cuz T:" + top_ + ", B:" + bottom_ + ", L:" + left_ + ", R:" + right_ );
	}
	
	bool IsOccupied(int hash, int x, int y)
	{		
		
		for ( int i = 0; i < 6; ++i )
		{
			for ( int j = 0; j < 4; ++j)
			{
				int compareHash_ = 1 << ((j * 6) + i);
				
				if ( (compareHash_ & hash) != 0 )
				{	
					
					if (x-i < 0 || x-i >= HangarManager.HANGAR_SIZE || y-j < 0 || y-j >= HangarManager.HANGAR_SIZE )
					{
						return true;
					}
					
					if ( mOccupied[x-i,y-j] )
					{
						return true;
					}
				}
			}
		}
		
		return false;
	}
		
	void Occupy(Part part, Vector3 position, bool place)
	{
		int x, y;
	
		GetHangarPlace(position, out x, out y);
		int hash = part.mPattern.mHash;
		
		//Setting hangar layer camera
		if ( place )
		{
			Utils.SetLayer(part.transform, MainManager.GetInstance()._HangarCamera.GetRealLayer() );
		}
		else
		{
			Utils.SetLayer(part.transform, 0 );
		}
			
		SetStats(part, place);
				
		for ( int i = 0; i < 6; ++i )
		{
			for ( int j = 0; j < 4; ++j)
			{
				int compareHash_ = 1 << ((j * 6) + i);
				
				if ( (compareHash_ & hash) != 0 )
				{		
					if (x-i < 0 || x-i >= HangarManager.HANGAR_SIZE || y-j < 0 || y-j >= HangarManager.HANGAR_SIZE )
					{
						Debug.Log ("Kurva UZ!");
						continue;
					}
					
					if ( mOccupied[x-i, y-j] == place)
					{
						Debug.Log ("Kurva UZ! 2");
						continue;
					}
					
					mOccupied[x-i,y-j] = place;
				}
			}
		}
		
		//NormalizeOccupied();
	}
	
/*	void NormalizeOccupied()
	{
		int bottom_ = 999;
		int left_ = 999;
		
		for ( int i = 0; i < HangarManager.HANGAR_SIZE; ++i )
		{
			for ( int j = 0; j < HangarManager.HANGAR_SIZE; ++j )
			{
				if (mOccupied[i,j])
				{
					if ( i < left_ )
					{
						left_ = i;
					}
					if ( j < bottom_ )
					{
						bottom_ = j;
					}
				}
			}
		}
		
		for ( int i = left_; i < HANGAR_SIZE; ++i )
		{
			for ( int j = bottom_; j < HANGAR_SIZE; ++j )
			{
				mOccupiedNormalized[i-left_, j-bottom_] =  mOccupied[i,j];
			}
		}
	}*/
	
	public void Shoot(Weapon weapon, int direction, int offset)
	{
//		Debug.Log(mShipName + ": Shooting with" + weapon.mObject.name  + " from direction " + direction + ", offset " + offset);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if ( DebugRotate_ )
		{
			_ShipContainer.Rotate(Vector3.up, Time.deltaTime * 10.0f);
		}
	}
}
