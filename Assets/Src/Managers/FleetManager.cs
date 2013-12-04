using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FleetManager : MonoBehaviour 
{
	public string mPlayerShipName = "";
	
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
	
	public class ShipScan
	{
		public struct PatternPlan
		{
			public PatternPlan(string patternID, Vector3 position)
			{
				mPatternID = patternID;
				mPosition = position;
			}
			
			//public PartManager.Pattern mPattern {get; private set;}
			public string mPatternID;
			public Vector3 mPosition {get; private set;}
		};
		
		public string mName;
		public Vector3 mCenter;
		public int mBoundaryH;
		public int mBoundaryV;
		public int mOffsetH;
		public int mOffsetV;
		
		
		public List <PatternPlan> mPartList {get; private set;}
		
		public ShipScan()
		{
			mPartList = new List<PatternPlan>();
		}
		
		//public void AddPart(Part part)
		public void AddPart(string patternID, Vector3 localPosition)
		{
			//if ( part == null )
			//{
			//	return;
			//}
			//mPartList.Add(new PatternPlan(part.mPattern, part.transform.localPosition));
			//mPartList.Add(new PatternPlan(PartManager.GetInstance().GetPattern(patternID).GetComponent<PatternPlan>(), localPosition));
			mPartList.Add(new PatternPlan(patternID, localPosition));
		}
		
		List<PatternPlan> mList;
	};
	
	public void DestroyShipInstance(Ship instance)
	{
//		mUsedShipList.Remove(instance);
		instance.EraseShip();
	}
	
	public ShipScan GetRandomScan()
	{
		int scanCount_ = PlayerPrefs.GetInt("ScanShipCount");
		
		int order_ = Random.Range(1, scanCount_);
		Debug.Log ("Getting some scan ScannedShip_" + order_);
		return mShipScanList["ScannedShip_"+order_];
	}
	
	public ShipScan GetPlayerScan()
	{
		return mShipScanList[mPlayerShipName];
	}
	
	Dictionary<string, ShipScan> mShipScanList = new Dictionary<string, ShipScan>();
	
	public ShipScan GetDuplicate(Ship ship)
	{
		int scanCount_ = PlayerPrefs.GetInt("ScanShipCount");
		for ( int i = 1; i < scanCount_+1; ++ i )
		{
			if ( IsMatching(ship, mShipScanList["ScannedShip_" + i]) )
			{
				return mShipScanList["ScannedShip_" + i];
			}
		}
		
		return null;
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
		
		ShipScan new_ = new ShipScan();
		new_.mName = name_;
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
		
		mShipScanList.Add(new_.mName, new_);
		
		mPlayerShipName = new_.mName;
	}
	
	void RestoreScan(string name)
	{
		ShipScan new_ = new ShipScan();
		
		Vector3 center_;
		center_.x = PlayerPrefs.GetFloat(name+"_centerX");
		center_.y = PlayerPrefs.GetFloat(name+"_centerY");
		center_.z = PlayerPrefs.GetFloat(name+"_centerZ");
		new_.mCenter = center_;
		
		new_.mBoundaryH = PlayerPrefs.GetInt(name+"_boundaryH");
		new_.mBoundaryV = PlayerPrefs.GetInt(name+"_boundaryV");
		new_.mOffsetH = PlayerPrefs.GetInt(name+"_offsetH");
		new_.mOffsetV = PlayerPrefs.GetInt(name+"_offsetV");
		
		int count_ = PlayerPrefs.GetInt(name+"_partCount");
		for ( int i = 0; i < count_; ++i )
		{
			string id_ = PlayerPrefs.GetString(name+"_part_"+i+"_id");
			Vector3 pos_;
			pos_.x = PlayerPrefs.GetFloat(name+"_part_"+i+"_posX");
			pos_.y = PlayerPrefs.GetFloat(name+"_part_"+i+"_posY");
			pos_.z = PlayerPrefs.GetFloat(name+"_part_"+i+"_posZ");
			
			new_.AddPart(id_, pos_);
		}
		new_.mName = name;
		
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
		PlayerPrefs.SetInt("ScanShipCount", lastIndex);
		
		PlayerPrefs.SetFloat(scan.mName+"_centerX", scan.mCenter.x);
		PlayerPrefs.SetFloat(scan.mName+"_centerY", scan.mCenter.y);
		PlayerPrefs.SetFloat(scan.mName+"_centerZ", scan.mCenter.z);
		
		PlayerPrefs.SetInt(scan.mName+"_boundaryH", scan.mBoundaryH);
		PlayerPrefs.SetInt(scan.mName+"_boundaryV", scan.mBoundaryV);
		PlayerPrefs.SetInt(scan.mName+"_offsetH", scan.mOffsetH);
		PlayerPrefs.SetInt(scan.mName+"_offsetV", scan.mOffsetV);
		
		PlayerPrefs.SetInt(scan.mName+"_partCount", scan.mPartList.Count);
		for ( int i = 0; i < scan.mPartList.Count; ++i )
		{
			PlayerPrefs.SetString(scan.mName+"_part_"+i+"_id", scan.mPartList[i].mPatternID);
			PlayerPrefs.SetFloat(scan.mName+"_part_"+i+"_posX", scan.mPartList[i].mPosition.x);
			PlayerPrefs.SetFloat(scan.mName+"_part_"+i+"_posY", scan.mPartList[i].mPosition.y);
			PlayerPrefs.SetFloat(scan.mName+"_part_"+i+"_posZ", scan.mPartList[i].mPosition.z);
		}
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
	
//	List<Ship> mUsedShipList = new List<Ship>();
	
	public void RegisterShip(Ship which)
	{
//		mUsedShipList.Add(which);
	}
	
	public void SetHangarEntry(bool state)
	{
		
		FleetManager.GetShip().SetHangarEntry(state);
//		for ( int i = 0; i < mUsedShipList.Count; ++i )
		{
//			mUsedShipList[i].gameObject.SetActive(!state);
		}
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
		
		Debug.Log ("Restored " + mShipScanList.Count + " ships");
		GetShip();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
