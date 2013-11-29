using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ship : MonoBehaviour 
{

	private Part [,] mOccupied = new Part[HangarManager.HANGAR_SIZE, HangarManager.HANGAR_SIZE];

	public GameObject mStats {get; private set;}
	
	private float mEnergyOverall = 0;
	public float mEnergyProduction { get; private set;}
	private float mCreditCost = 0;
	private float mShieldCapacity = 0;
	private float mShieldRecharge = 0;
	private float mWeaponDamage = 0;
	private float mMass = 0;
	private float mEnginePower = 0;
	private bool mValidShip = false;
	
	public int _OffsetHorizontal {get; private set;}
	public int _OffsetVertical {get; private set;}
	public int _BoundaryHorizontal {get; private set;}
	public int _BoundaryVertical {get; private set;}
	public Vector3 mShipCenter { get; private set;}
	
	BattleComputer _BattleComputer;
	
	public bool IsAlive()
	{
		if ( mWeaponDamage > 0 && mEnergyOverall > 0)
		{
			return true;
		}
		
		return false;
	}
	
	public Part GetPartAt(int x, int y)
	{
		return mOccupied[x,y];
	}
	
	public string mShipName {get; private set;}
	bool mInitialized = false;
	
	void ClearHangar()
	{
		for ( int i = 0; i < HangarManager.HANGAR_SIZE; ++i )
		{
			for ( int j = 0; j < HangarManager.HANGAR_SIZE; ++j )
			{
				mOccupied[i,j] = null;
			}
		}
	}
	
	void CreateContainer(string name)
	{
		_ShipContainer = new GameObject(name + "_Container").transform;
		_ShipContainer.transform.localPosition = Vector3.zero;
		_ShipContainer.transform.localRotation = Quaternion.identity;
		
		transform.parent = _ShipContainer;
	}
	
	public void Initialize(FleetManager.ShipScan template)
	{
		if ( mInitialized )
		{
			return;
		}
		
		CreateContainer(template.mName);
		_BattleComputer = gameObject.AddComponent<BattleComputer>();
		
		ClearHangar();
		
		LoadShip(template);
		
		transform.localPosition = mShipCenter = template.mCenter;
		_BoundaryHorizontal = template.mBoundaryH;
		_BoundaryVertical = template.mBoundaryV;
		_OffsetVertical = template.mOffsetV;
		_OffsetHorizontal = template.mOffsetH;
		
		mInitialized = true;
	}
	
	Transform _ShipContainer;
		
	public void Initialize(string name)
	{
		if ( mInitialized )
		{
			return;
		}
		
		CreateContainer(name);
		_BattleComputer = gameObject.AddComponent<BattleComputer>();	
		
		mShipName = name;
		
		ClearHangar();
				
		RestoreShip();
		
		transform.localPosition = mShipCenter;
		
		
		
		mInitialized = true;
	}
	
	public void RemoveDestroyedParts()
	{
		Part actual_;
		for ( int x_ = 0; x_ < mOccupied.GetLength(0); ++x_ )
		{
			for ( int y_ = 0; y_ < mOccupied.GetLength(1); ++y_ )
			{
				actual_ = mOccupied[x_, y_];
				if ( actual_ != null && actual_.mHP <= 0 )
				{
					Debug.Log (name + ": Removing " + actual_);
					Occupy(actual_, actual_.transform.localPosition, false);
					GetComponent<BattleComputer>().CheckPart(actual_, false);
					GameObject.Destroy(actual_.gameObject);
				}
			}
		}
		
		GetBoundary();
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
		
		float x_, y_, z_;
		
		for ( int i = 0; i < itemCount_; ++i )
		{
			GameObject part_ = PartManager.GetInstance().GetPattern(PlayerPrefs.GetString(mShipName+"_ShipPartID_"+i));
			
			InventoryManager.GetInstance().AddCaption(part_.GetComponent<Part>());
			part_.GetComponent<Part>().SetCaptionVisibility(false);
			
			part_.transform.parent = transform;
			
			x_ = PlayerPrefs.GetFloat(mShipName+"_ShipPartPosX_"+i);
			y_ = PlayerPrefs.GetFloat(mShipName+"_ShipPartPosY_"+i);
			z_ = PlayerPrefs.GetFloat(mShipName+"_ShipPartPosZ_"+i);
			
			
			part_.transform.localPosition = new Vector3(x_,y_,z_);
			
			part_.GetComponent<Part>().mLocation = Part.Location.Ship;
			Occupy(part_.GetComponent<Part>(), part_.transform.localPosition, true);
			
		}
		
		x_ = PlayerPrefs.GetFloat(mShipName+"_ShipCenterX");
		y_ = PlayerPrefs.GetFloat(mShipName+"_ShipCenterY");
		z_ = PlayerPrefs.GetFloat(mShipName+"_ShipCenterZ");
		mShipCenter = new Vector3(x_, y_, z_);
		
		_BoundaryHorizontal = PlayerPrefs.GetInt(mShipName+"_ShipBoundaryH");
		_BoundaryVertical = PlayerPrefs.GetInt(mShipName+"_ShipBoundaryV");
		_OffsetHorizontal = PlayerPrefs.GetInt(mShipName+"_ShipOffsetH");
		_OffsetVertical = PlayerPrefs.GetInt(mShipName+"_ShipOffsetV");
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
		PlayerPrefs.SetFloat(mShipName+"_ShipCenterX",mShipCenter.x);
		PlayerPrefs.SetFloat(mShipName+"_ShipCenterY",mShipCenter.y);
		PlayerPrefs.SetFloat(mShipName+"_ShipCenterZ",mShipCenter.z);
		
		PlayerPrefs.SetInt(mShipName+"_ShipBoundaryH",_BoundaryHorizontal);
		PlayerPrefs.SetInt(mShipName+"_ShipBoundaryV",_OffsetVertical);
		PlayerPrefs.SetInt(mShipName+"_ShipOffsetH",_OffsetHorizontal);
		PlayerPrefs.SetInt(mShipName+"_ShipOffsetV",_OffsetVertical);
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
		GameObject.Destroy(mStats);
		GameObject.Destroy(transform.parent.gameObject);
	}
	
	void UpdateStats()
	{
		if ( mStats == null )
		{
			mStats = TextManager.GetInstance().GetText("", 0.6f);
			mStats.GetComponent<TextMesh>().alignment = TextAlignment.Right;
			mStats.GetComponent<TextMesh>().anchor = TextAnchor.UpperRight;
		}
		
		mStats.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1,1,0)) + Vector3.down * 8.0f;
		mStats.transform.rotation = Camera.main.transform.rotation;
		
		
		mStats.GetComponent<TextMesh>().text = 
			"PRICE: " + mCreditCost + 
			"\nENERGY: " + mEnergyOverall +
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
		mEnergyOverall += addition ? part.mPattern.mPower : -part.mPattern.mPower;
		mEnergyProduction += addition ? part.mPattern.mPower : 0;
		
		_BattleComputer.CheckPart(part, addition);
		
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
		}
		new_ = part.mPattern.GetAbility(PartManager.AbilityType.Phaser);
		if ( new_ != null )
		{
			mWeaponDamage += (addition ? 1 : -1 ) * new_.mValue;
		}
		new_ = part.mPattern.GetAbility(PartManager.AbilityType.Tracer);
		if ( new_ != null )
		{
			mWeaponDamage += (addition ? 1 : -1 ) * new_.mValue;
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
		mValidShip = (mEnginePower > 0 && mEnergyOverall >= 0);
		
		if ( mValidShip != lastValidity_ )
		{
			HangarManager.GetInstance().InformShipValidity(mValidShip);
		}
		
		UpdateStats();
	}
	
	public bool DebugRotate_ = false;
	
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
			UpdateStats();
			DebugRotate_ = false;
			_ShipContainer.parent = HangarManager.GetInstance()._HangarContainer.transform;
			transform.localPosition = Vector3.zero;
			_ShipContainer.localPosition = Vector3.zero;		
			_ShipContainer.localRotation = Quaternion.identity;
		}
	}
	
	
	void GetBoundary(/*out float top, out float bottom, out float left, out float right*/)
	{
		int top=-999;
		int bottom=999;
		int left=999; 
		int right=-999;
		
		for ( int x = 0; x < HangarManager.HANGAR_SIZE; ++x )
		{
			for ( int y = 0; y < HangarManager.HANGAR_SIZE; ++y )
			{
				if ( mOccupied[x,y] != null )
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
		
		_BoundaryHorizontal = (int)(right - left);
		_BoundaryVertical = (int)(top - bottom);
		_OffsetHorizontal = (int)(left);
		_OffsetVertical = (int)(bottom);
	}	
	
	public void CalculateShipCenter()
	{		
	/*	float top_, bottom_,left_,right_;
		
		GetBoundary(out top_, out bottom_, out left_, out right_);
		
		mShipCenter = new Vector3( 
			(int)((HangarManager.HANGAR_SIZE) * 0.5f) - ((right_ - left_  ) * 0.5f + left_   + 0.5f) , 0, 
			(int)((HangarManager.HANGAR_SIZE) * 0.5f) - ((top_   - bottom_) * 0.5f + bottom_ + 0.5f) );*/
		
		GetBoundary();
	
		mShipCenter = new Vector3( 
			(int)((HangarManager.HANGAR_SIZE) * 0.5f) - (_BoundaryHorizontal * 0.5f + _OffsetHorizontal + 0.5f) , 0, 
			(int)((HangarManager.HANGAR_SIZE) * 0.5f) - (_BoundaryVertical * 0.5f + _OffsetVertical + 0.5f) );
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
					
					if ( mOccupied[x-i,y-j] != null )
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
					
					if ( (place && mOccupied[x-i, y-j] != null) || (!place && mOccupied[x-i, y-j] == null) )
					{
						Debug.Log ("Kurva UZ! 2");
						continue;
					}
					
					mOccupied[x-i,y-j] = place ? part : null;
				}
			}
		}	
		
		
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
