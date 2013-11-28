using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FleetManager : MonoBehaviour 
{
	
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
			public PatternPlan(PartManager.Pattern pattern, Vector3 position)
			{
				mPattern = pattern;
				mPosition = position;
			}
			
			public PartManager.Pattern mPattern {get; private set;}
			public Vector3 mPosition {get; private set;}
		};
		
		public string mName;
		public Vector3 mCenter;
		
		public List <PatternPlan> mPartList {get; private set;}
		
		public ShipScan()
		{
			mPartList = new List<PatternPlan>();
		}
		
		public void AddPart(Part part)
		{
			if ( part == null )
			{
				return;
			}
			mPartList.Add(new PatternPlan(part.mPattern, part.transform.localPosition));
		}
		
		List<PatternPlan> mList;
	};
	
	public void DestroyShipInstance(Ship instance)
	{
		mUsedShipList.Remove(instance);
		instance.EraseShip();
	}
	
	public ShipScan GetScan(string name)
	{
		return mShipScanList[name];
	}
	
	Dictionary<string, ShipScan> mShipScanList = new Dictionary<string, ShipScan>();
	
	public void ScanShip(GameObject ship)
	{
		string name_ = "TestShip";
		
		if ( mShipScanList.ContainsKey(name_) )
		{
			int i = 0;
			while ( mShipScanList.ContainsKey(name_ +"_"+ (++i)) ){}
			name_+= "_"+i;
		}
		
//		Debug.Log ("Scanning ship" + ship.GetComponent<Ship>().mShipName + " as " + name_);
		
		ShipScan new_ = new ShipScan();
		new_.mName = name_;
		new_.mCenter = ship.GetComponent<Ship>().mShipCenter;
			
		for ( int i = 0; i < ship.transform.childCount; ++i )
		{
			new_.AddPart(ship.transform.GetChild(i).GetComponent<Part>());			
		}
		
		mShipScanList.Add(new_.mName, new_);
	}
	
	List<Ship> mUsedShipList = new List<Ship>();
	
	public void RegisterShip(Ship which)
	{
		mUsedShipList.Add(which);
	}
	
	public void SetHangarEntry(bool state)
	{
		
		FleetManager.GetShip().SetHangarEntry(state);
		for ( int i = 0; i < mUsedShipList.Count; ++i )
		{
			mUsedShipList[i].gameObject.SetActive(!state);
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
		GetShip();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
