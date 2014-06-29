using UnityEngine;
using System.Collections;

public class PlayerData : MonoBehaviour 
{
	public void DeleteAll()
	{
		FleetManager.GetShip().EraseShip();
		PlayerPrefs.DeleteAll();
		FleetManager.Instance.DeleteAllScans();
		FleetManager.GetShip().GetTierData().DeleteAll();	
		Restore();
	}
	
	public void Backup()
	{
		FleetManager.GetShip().GetTierData().Backup();
		InventoryManager.Instance.BackupInventory();
	}

	public void Restore()
	{
		TierData tierData_ = FleetManager.GetShip().GetTierData();

		tierData_.Restore();

		if ( tierData_._TotalFightCount < 1 )
		{
			InventoryManager.Instance.FillInventory(1);
		}
		else
		{
			InventoryManager.Instance.RestoreInventory();

			if ( InventoryManager.Instance.GetCount() == 0 && !FleetManager.GetShip().IsAlive() )
			{
				InventoryManager.Instance.FillInventory(1);
			}
		}
	}
}
