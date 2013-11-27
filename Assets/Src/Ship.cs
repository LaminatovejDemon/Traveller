using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ship : MonoBehaviour 
{
	
	const int HANGAR_SIZE = 30;
	
//	private static Ship mInstance = null;
	private bool [,] mOccupied = new bool[HANGAR_SIZE, HANGAR_SIZE];
	private bool [,] mOccupiedNormalized = new bool[HANGAR_SIZE, HANGAR_SIZE];
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
		for ( int i = 0; i < HANGAR_SIZE; ++i )
		{
			for ( int j = 0; j < HANGAR_SIZE; ++j )
			{
				mOccupied[i,j] = false;
			}
		}
	}
	
	public int GetRange(int direction)
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
	}
	
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
	
	public void Initialize(string name)
	{
		if ( mInitialized )
		{
			return;
		}
		
		mWeaponList = new List<Weapon>();
		
		mShipName = name;
		
		ClearHangar();
				
		RestoreShip();
		mInitialized = true;
	}
		
	
	public void RetrievePart(Transform which)
	{
		which.parent = null;
		Occupy(which.GetComponent<Part>(), which.transform.position, false);
		BackupShip();
	}
	
	public void InsertPart(Transform which)
	{
		which.parent = transform;
		Occupy(which.GetComponent<Part>(), which.transform.position, true);
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
			Occupy(part_.GetComponent<Part>(), part_.transform.position, true);
			
		}
		
		CalculateShipCenter();
	}
	
	void BackupShip()
	{
		Debug.Log ("Backing up ship " + mShipName);
		PlayerPrefs.SetInt(mShipName+"_ShipItemCount", transform.childCount);
		for ( int i = 0; i<transform.childCount; ++i )
		{
			//Debug.Log("Backuping transform of " + transform.GetChild(i).GetComponent<Part>().mPattern.mID + " to " + transform.getch + ", " + y_ + ", " + z_);
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
		x = ((int)(position.x + 1000.5f))-1000 + HANGAR_SIZE / 2 - 13;
		y = ((int)(position.z + 1000.5f))-1000 + HANGAR_SIZE / 2; 
	}
	
	public bool IsOccupied(int hash, Vector3 position)
	{
		int x, y;
		GetHangarPlace(position, out x, out y);
		
		for ( int i = 0; i < 6; ++i )
		{
			for ( int j = 0; j < 4; ++j)
			{
				int compareHash_ = 1 << ((j * 6) + i);
				if ( (compareHash_ & hash) != 0 )
				{
					if ( x-i < 0 || x-i >= HANGAR_SIZE || y-j < 0 || y-j >= HANGAR_SIZE )
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
		
		return mOccupied[x,y];
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
	
	public void SetHangarEntry(bool state)
	{
		if ( mStats != null )
		{
			mStats.SetActive(state);
		}
		
		if ( !state )
		{
			FleetManager.GetInstance().ScanShip(gameObject);
			
			Ship newShip_ = FleetManager.GetShip( FleetManager.GetInstance().GetScan("SomeShip") );
			FleetManager.GetInstance().RegisterShip(newShip_);
			
			BattleManager.GetInstance().StartBattle(this , newShip_);
		}
		else
		{
			CalculateShipCenter();
		}
	}
	
	
	
	void GetBoundary(out float top, out float bottom, out float left, out float right, bool normalized)
	{
		top=-999;
		bottom=999;
		left=999; 
		right=-999;
		
		for ( int x = 0; x < HANGAR_SIZE; ++x )
		{
			for ( int y = 0; y < HANGAR_SIZE; ++y )
			{
				if ( (normalized ? mOccupiedNormalized[x,y] : mOccupied[x,y]) )
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
		
		GetBoundary(out top_, out bottom_, out left_, out right_, false);
		
		mShipCenter = new Vector3((int)(left_ + (right_ - left_) * 0.5f), 0, (int)(bottom_ + ( top_ - bottom_) * 0.5f));
//		Debug.Log ("Ship Center is " + mShipCenter + " cuz T:" + top_ + ", B:" + bottom_ + ", L:" + left_ + ", R:" + right_ );
		transform.position -= mShipCenter;
	}
		
	void Occupy(Part part, Vector3 position, bool place)
	{
		int x, y;
		GetHangarPlace(position, out x, out y);
		int hash = part.mPattern.mHash;
		
		SetStats(part, place);
				
		for ( int i = 0; i < 6; ++i )
		{
			for ( int j = 0; j < 4; ++j)
			{
				int compareHash_ = 1 << ((j * 6) + i);
				
				if ( x-i < 0 || x-i >= HANGAR_SIZE || y-j < 0 || y-j >= HANGAR_SIZE )
				{
					Debug.Log("KURVA UZ");
					break;
				}
				
				if ( (compareHash_ & hash) != 0 )
				{
					mOccupied[x-i,y-j] = place;
					
				}
			}
		}
		
		NormalizeOccupied();
	}
	
	void NormalizeOccupied()
	{
		int bottom_ = 999;
		int left_ = 999;
		
		for ( int i = 0; i < HANGAR_SIZE; ++i )
		{
			for ( int j = 0; j < HANGAR_SIZE; ++j )
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
	}
	
	public void Shoot(Weapon weapon, int direction, int offset)
	{
//		Debug.Log(mShipName + ": Shooting with" + weapon.mObject.name  + " from direction " + direction + ", offset " + offset);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
