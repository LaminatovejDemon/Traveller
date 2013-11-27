using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour 
{
	private static NetworkManager mInstance = null;
	public static NetworkManager GetInstance()
	{
		if ( mInstance == null )
		{
			GameObject instanceObject_ = GameObject.Find("#NetworkManager");
			if ( instanceObject_ != null )
				mInstance = instanceObject_.transform.GetComponent<NetworkManager>();
			else
				mInstance =  new GameObject("#NetworkManager").AddComponent<NetworkManager>();
		}
		return mInstance;
	}
	
	int mLastHostTriedIndex = 0;
	
	// Use this for initialization
	void Start () 
	{
		RequestHostList();
	}
	
	void RequestHostList()
	{
		MasterServer.RequestHostList("Obulus_Traveler");
	}
	
	void CheckHostList()
	{
		HostData [] hostData_ = MasterServer.PollHostList();
		
		Debug.Log ("HostList has " + hostData_.Length + " hosts.");
		
		if ( hostData_.Length == 0 )
		{
			
			CreateHost();
			return;
		}
		
		if ( mLastHostTriedIndex >= hostData_.Length )
		{
			mLastHostTriedIndex = 0;
		}
		
		ConnectToHost(hostData_[mLastHostTriedIndex++]);
	}
	
	void ConnectToHost(HostData server)
	{
		Debug.Log ("Connecting to host..." + server.gameName);
		Network.Connect(server);
	}
	
	void CreateHost()
	{
		Debug.Log ("Creating host");
		Network.InitializeServer(32, 73234, Network.HavePublicAddress());
		MasterServer.RegisterHost("Obulus_Traveler",  "debug");
	}
	
	void OnConnectedToServer()
	{
		Debug.Log ("Connected to server");
	}
	
	void OnFailedToConnect()
	{
		Debug.Log ("Failed to connect");
		RequestHostList();
	}
	
	void OnServerInitialized()
	{
		Debug.Log ("Server is initialzied");
	}
	
	void OnPlayerDisconnected()
	{
		Debug.Log ("Player is disconnected");
	}
	
	void OnPlayerConnected()
	{
		Debug.Log ("Player is connected");
	}
	
	void OnMasterServerEvent(MasterServerEvent evt)
	{
		switch (evt)
		{
		case MasterServerEvent.RegistrationSucceeded:
			Debug.Log ("Registration succeedded ");
			break;
		case MasterServerEvent.HostListReceived:
			Debug.Log ("HostListReceived");
			CheckHostList();
			break;
		case MasterServerEvent.RegistrationFailedGameName:
			Debug.Log ("RegistrationFailedGameName");
			break;
		case MasterServerEvent.RegistrationFailedGameType:
			Debug.Log ("RegistrationFailedGameType");
			break;
		case MasterServerEvent.RegistrationFailedNoServer:
			Debug.Log ("RegistrationFailedNoServer");
			break;
		}
	}
}
