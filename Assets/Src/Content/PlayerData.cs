using UnityEngine;
using System.Collections;

public class PlayerData : MonoBehaviour 
{
	int _AchievedRarity = 1;

	public void Backup()
	{
		PlayerPrefs.SetInt("Player_AchievedRarity", _AchievedRarity);
		InventoryManager.GetInstance().BackupInventory();
	}

	public void Restore()
	{
		Debug.Log("REstoring player Data");
		_AchievedRarity = PlayerPrefs.GetInt("Player_AchievedRarity");
		//_AchievedRarity = 0;

		if ( _AchievedRarity < 1 )
		{
			_AchievedRarity = 1;
			InventoryManager.GetInstance().FillInventory(_AchievedRarity);
		}
		else
		{
			InventoryManager.GetInstance().RestoreInventory();
		}
	}
}
