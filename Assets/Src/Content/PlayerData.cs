using UnityEngine;
using System.Collections;

public class PlayerData : MonoBehaviour 
{
	public void DeleteAll()
	{
		FleetManager.GetShip().EraseShip();
		FleetManager.GetInstance().DeleteAllScans();
		PlayerPrefs.DeleteAll();
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

		if ( tierData_._AchievedRarity < 1 )
		{
			tierData_._AchievedRarity = 1;
			InventoryManager.GetInstance().FillInventory(tierData_._AchievedRarity);
		}
		else
		{
			InventoryManager.GetInstance().RestoreInventory();
		}
	}
}
