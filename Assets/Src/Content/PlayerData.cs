using UnityEngine;
using System.Collections;

public class PlayerData : MonoBehaviour 
{
	public void DeleteAll()
	{
		FleetManager.GetShip().EraseShip();
		PlayerPrefs.DeleteAll();
		FleetManager.GetInstance().DeleteAllScans();
		FleetManager.GetShip().GetTierData().DeleteAll();
		
		FleetManager.GetInstance().AddTutorialShip();
		Restore();
	
	}
	
	public void Backup()
	{
		FleetManager.GetShip().GetTierData().Backup();
		InventoryManager.GetInstance().BackupInventory();
	}

	public void Restore()
	{
		TierData tierData_ = FleetManager.GetShip().GetTierData();

		tierData_.Restore();

		if ( tierData_._TotalFightCount < 1 )
		{
			InventoryManager.GetInstance().FillInventory(1);
		}
		else
		{
			InventoryManager.GetInstance().RestoreInventory();
		}
	}
}
