using UnityEngine;
using System.Collections;

public class HangarManager : MonoBehaviour 
{
	
	private static HangarManager mInstance = null;
	public static HangarManager GetInstance()
	{
		if ( mInstance == null )
		{
			GameObject instanceObject_ = GameObject.Find("#HangarManager");
			if ( instanceObject_ != null )
				mInstance = instanceObject_.transform.GetComponent<HangarManager>();
			else
				mInstance =  new GameObject("#HangarManager").AddComponent<HangarManager>();
		}
		return mInstance;
	}
	
	bool mInitlialzed = false;
	GameObject mDoneButton = null;
	GameObject mHangarButton = null;
	GameObject mClearButton = null;
	
	public void InformShipValidity(bool state)
	{
		mDoneButton.SetActive(state);
	}
	
	void Initialize()
	{
		if ( mInitlialzed )
		{
			return;
		}
		mHangarButton = Button.Create("OPEN HANGAR", "OnHangarOpenButton", transform);
		mHangarButton.transform.parent = Camera.main.transform;
		mHangarButton.transform.localRotation = Quaternion.identity;
		mHangarButton.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1,0,0));
		mHangarButton.transform.localPosition += Vector3.left * mHangarButton.transform.GetComponent<Button>().mRenderer.bounds.extents.x
			+ Vector3.up * mHangarButton.transform.GetComponent<Button>().mRenderer.bounds.extents.y;
		mHangarButton.SetActive(false);
		
		mDoneButton = Button.Create("DONE", "OnDoneButton", transform);
		mDoneButton.transform.parent = Camera.main.transform;
		mDoneButton.transform.localRotation = Quaternion.identity;
		mDoneButton.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1,0,0));
		mDoneButton.transform.localPosition += Vector3.left * mDoneButton.transform.GetComponent<Button>().mRenderer.bounds.extents.x
			+ Vector3.up * mDoneButton.transform.GetComponent<Button>().mRenderer.bounds.extents.y;
		mDoneButton.SetActive(false);
		
		mClearButton = Button.Create("ERASE SHIP", "OnClearButton", transform);
		mClearButton.transform.parent = Camera.main.transform;
		mClearButton.transform.localRotation = Quaternion.identity;
		mClearButton.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1,1,0)); 
		mClearButton.transform.localPosition += Vector3.left * mDoneButton.transform.GetComponent<Button>().mRenderer.bounds.extents.x
			+ Vector3.down * mClearButton.transform.GetComponent<Button>().mRenderer.bounds.extents.y;
			
		mInitlialzed = true;
	}
	
	void OnHangarOpenButton()
	{	
		SetHangarVisibility(true);
	}
	
	void OnDoneButton()
	{	
		SetHangarVisibility(false);	
	}
	
	void SetHangarVisibility(bool state)
	{
		InventoryManager.GetInstance().gameObject.SetActive(state);
		mClearButton.SetActive(state);
		mDoneButton.SetActive(state);
		mHangarButton.SetActive(!state);
		
		FleetManager.GetInstance().SetHangarEntry(state);
	}
	
	void OnClearButton()
	{
		FleetManager.GetShip().EraseShip();
	}
	
	void Start () 
	{
		Initialize();
	}
	
	void Update () {
	
	}
}
