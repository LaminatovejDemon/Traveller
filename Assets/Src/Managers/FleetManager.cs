using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FleetManager : MonoBehaviour 
{
	public string mPlayerShipName = "";
	GameObject _ShipScanContainer;
	
	private static FleetManager mInstance = null;
	public static FleetManager GetInstance()
	{
		if ( mInstance == null )
		{
			GameObject instanceObject_ = GameObject.Find("#FleetManager");
			if ( instanceObject_ != null )
				mInstance = instanceObject_.transform.GetComponent<FleetManager>();
			else
				mInstance =  new GameObject("#FleetManager").AddComponent<FleetManager>();
		}
		return mInstance;
	}
	
	public void DestroyShipInstance(Ship instance)
	{
		instance.EraseShip();
	}

	public ShipScan GetAverageScan(float winRatio)
	{
		ShipScan best_ = GetRandomScan();

		if ( best_ == null )
		{
			return null;
		}

		ShipScan alter_ = best_;

		for ( int i = 0; i < 10; ++i )
		{
			alter_ = GetRandomScan();
			if ( Mathf.Abs(alter_.GetTierData().GetVictoryCoef() - winRatio) < Mathf.Abs(best_.GetTierData().GetVictoryCoef() - winRatio) )
			{
				best_ = alter_;
			}
		}

		return best_;
	}
	
	public ShipScan GetRandomScan()
	{
		if ( mShipScanList.Count < 2 )
		{
			return null;
		}

		int scanCount_ = PlayerPrefs.GetInt("ScanShipCount");
		
		int order_ = Random.Range(1, scanCount_);


		return mShipScanList["ScannedShip_"+order_];
	}
	
	public ShipScan GetPlayerScan()
	{
		if ( !mShipScanList.ContainsKey(mPlayerShipName) )
		{
			return null;
		}

		return mShipScanList[mPlayerShipName];
	}
	
	Dictionary<string, ShipScan> mShipScanList = new Dictionary<string, ShipScan>();
	
	public ShipScan GetDuplicate(Ship ship)
	{

		int scanCount_ = PlayerPrefs.GetInt("ScanShipCount");
		Debug.Log("ScanShipCount is " + scanCount_);

		for ( int i = 1; i < scanCount_+1; ++ i )
		{
			if ( mShipScanList.ContainsKey("ScannedShip_" + i) && IsMatching(ship, mShipScanList["ScannedShip_" + i]) )
			{
				return mShipScanList["ScannedShip_" + i];
			}
		}
		
		return null;
	}

	public void DeleteAllScans()
	{
		GameObject.Destroy(_ShipScanContainer);
		mShipScanList.Clear();
		PlayerPrefs.SetInt("ScanShipCount", 0);
	}

	public void DeleteAllTierData()
	{
		for ( int i = 1; i < mShipScanList.Count; ++i )
		{
			mShipScanList["ScannedShip_" + i].GetTierData().DeleteAll();
		}
	}

	public void AddTutorialShip()
	{
	/*	if ( _ShipScanContainer == null )
		{
			_ShipScanContainer = new GameObject("#ShipScanContainer");
		}

		ShipScan tutorialScan_ = ((GameObject)GameObject.Instantiate((GameObject)Resources.Load("Gameplay/TutorialShip", typeof(GameObject)))).GetComponent<ShipScan>();
		tutorialScan_.transform.parent = _ShipScanContainer.transform;
		mShipScanList.Add(tutorialScan_.mName, tutorialScan_);
		PlayerPrefs.SetInt("ScanShipCount", 1);*/
	}
	
	public bool IsMatching(Ship ship, ShipScan scan)
	{
		if ( ship.transform.childCount != scan.mPartList.Count )
		{
			return false;
		}
		
		bool match_;
		
		for ( int i = 0; i < ship.transform.childCount; ++i )
		{
			match_ = false;
			for ( int j = 0; j < scan.mPartList.Count; ++j )
			{
				if ( ship.transform.GetChild(i).GetComponent<Part>().mPattern.mID == scan.mPartList[i].mPatternID &&
					ship.transform.GetChild(i).transform.localPosition == scan.mPartList[i].mPosition )
				{
					match_ = true;
					break;
				}
			}
			
			if ( match_ == false )
			{
				return false;
			}
		}
		
		return true;
	}
	
	public void ScanShip(GameObject ship)
	{
		ShipScan duplicate_ = GetDuplicate(ship.GetComponent<Ship>());
		
		if ( duplicate_ != null)
		{
			Debug.Log ("We've got duplicate!");
			mPlayerShipName =  duplicate_.mName;
			return;
		}
		
		int scanCount_ = PlayerPrefs.GetInt("ScanShipCount");
		string name_ = "ScannedShip_" + (scanCount_+1);
		
		if ( mShipScanList.ContainsKey(name_) )
		{
			RemoveBackup(name_);
		}
		
		ShipScan new_ = new GameObject("_ShipScan_" + name).AddComponent<ShipScan>();
		if ( _ShipScanContainer == null )
		{
			_ShipScanContainer = new GameObject("#ShipScanContainer");
		}
		new_.transform.parent = _ShipScanContainer.transform;

		new_.name = new_.mName = name_;
		new_.mCenter = ship.GetComponent<Ship>().mShipCenter;
		new_.mBoundaryH = ship.GetComponent<Ship>()._BoundaryHorizontal;
		new_.mBoundaryV = ship.GetComponent<Ship>()._BoundaryVertical;
		new_.mOffsetH = ship.GetComponent<Ship>()._OffsetHorizontal;
		new_.mOffsetV = ship.GetComponent<Ship>()._OffsetVertical;
		
		for ( int i = 0; i < ship.transform.childCount; ++i )
		{
			new_.AddPart(ship.transform.GetChild(i).GetComponent<Part>().mPattern.mID, ship.transform.GetChild(i).transform.localPosition);			
		}
		
		BackupScan(new_, scanCount_+1);

		Debug.Log("Adding ship into list: " + new_.mName);

		mShipScanList.Add(new_.mName, new_);

		BattleManager.GetInstance().SimulateBattle(new_, 50);

		mPlayerShipName = new_.mName;
	}
	
	void RestoreScan(string name)
	{
//		Debug.Log ("Restoring scan " + name);

		ShipScan new_ = new GameObject("_ShipScan_" + name).AddComponent<ShipScan>();
		if ( _ShipScanContainer == null )
		{
			_ShipScanContainer = new GameObject("#ShipScanContainer");
		}
		new_.transform.parent = _ShipScanContainer.transform;

		new_.name = new_.mName = name;
		new_.Restore();

		mShipScanList.Add(new_.mName, new_);
	}
	
	void RestoreScans()
	{
		int scanCount_ = PlayerPrefs.GetInt("ScanShipCount");
		for ( int i = 1; i < scanCount_+1; ++i )
		{
			RestoreScan("ScannedShip_" + i);
		}
	}
	
	void BackupScan(ShipScan scan, int lastIndex)
	{
		Debug.Log("Setting ship count" + lastIndex);
		PlayerPrefs.SetInt("ScanShipCount", lastIndex);

		scan.Backup();
	}
	
	void RemoveBackup(string name)
	{
		PlayerPrefs.DeleteKey(name+"_centerX");
		PlayerPrefs.DeleteKey(name+"_centerY");
		PlayerPrefs.DeleteKey(name+"_centerZ");
		
		PlayerPrefs.DeleteKey(name+"_boundaryH");
		PlayerPrefs.DeleteKey(name+"_boundaryV");
		PlayerPrefs.DeleteKey(name+"_offsetH");
		PlayerPrefs.DeleteKey(name+"_offsetV");
		
		int count_ = PlayerPrefs.GetInt(name+"_partCount");
		for ( int i = 0; i < count_; ++i )
		{
			PlayerPrefs.DeleteKey(name+"_part_"+i+"_id");
			PlayerPrefs.DeleteKey(name+"_part_"+i+"_posX");
			PlayerPrefs.DeleteKey(name+"_part_"+i+"_posY");
			PlayerPrefs.DeleteKey(name+"_part_"+i+"_posZ");
		}
		
		PlayerPrefs.DeleteKey(name+"_partCount");
	}
		
	public void SetHangarEntry(bool state)
	{		
		FleetManager.GetShip().SetHangarEntry(state);
	}
	
	private static Ship mPlayerShip = null;
	
	public static Ship GetShip(ShipScan template = null)
	{
		if ( template != null )
		{
			Ship someShip_ = new GameObject(template.mName).AddComponent<Ship>();
			someShip_.Initialize(template);
			
			return someShip_;
		}
		 
		if ( mPlayerShip == null )
		{
			
			mPlayerShip = new GameObject("Ship").AddComponent<Ship>();
			
			mPlayerShip.Initialize("PlayerShip");
			
		}
		return mPlayerShip;
	}
	
	void Start () 
	{
		//REset
		//PlayerPrefs.SetInt("ScanShipCount", 0);
		RestoreScans();
		
		GetShip();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
