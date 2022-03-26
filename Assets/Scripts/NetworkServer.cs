using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;

public class NetworkServer : MonoBehaviourPunCallbacks, IConnectionCallbacks
{
	//Flag
	bool moved;
	public bool ready = false;
	public string _tag1;
	public string _tag2;


	GameManager gm;
	public List<RoomInfo> roomList;

	private string roomName;
	public string gameVersion = "0.1";

	private byte maxPlayers = 4;

	public bool host;
	//public string roomName;

	//private LoadBalancingClient loadBalance;

	public bool connected = false;
	public bool connectedToLobby = false;

	PhotonView pv;
	Tuple<Move, Move> moves;

	public void sendMoves(Tuple<Move, Move> _moves) { StartCoroutine(_sendMoves(_moves)); }
	public IEnumerator _sendMoves(Tuple<Move, Move> moves)
	{

		Debug.Log("Got to sendMoves and we are connected? " + connected);
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			connected = false;
		}

		yield return new WaitUntil(() => connected);
		ping();
		yield return new WaitUntil(getPinged);
		Debug.Log("Got past ping");
		Debug.Log(moves);
		Tuple<string, string> ss = Serialize.serialize(moves);
		pv.RPC("acceptMove", RpcTarget.Others, ss.Item1, ss.Item2);
	}

	public void sendTags(string t1, string t2) { StartCoroutine(_sendTags(t1, t2)); }
	public IEnumerator _sendTags(string t1, string t2)
	{
		Debug.Log("Got to line 55 and we are connected? " + connected);
		Debug.Log("Tags: " + t1 + ", " + t2);
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			connected = false;
		}

		yield return new WaitUntil(() => connected);

        while (!getPinged())
        {
            ping();
            yield return new WaitForSeconds(.1f);
        }
        Debug.Log("Sending tags");
		//Send tags
		pv.RPC("acceptTags", RpcTarget.Others, t1, t2);
	}

	public void sendReady(bool r) { StartCoroutine(_sendReady(r)); }
	public IEnumerator _sendReady(bool r)
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			connected = false;
		}

		yield return new WaitUntil(() => connected);
		ping();
		yield return new WaitUntil(getPinged);
		//Send ready Status
		pv.RPC("acceptReady", RpcTarget.Others, r);
	}

	bool isConnected()
	{
		return connected && connectedToLobby;
	}
	public void Start()
	{
		PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = gameVersion;
		if (!PhotonNetwork.IsConnectedAndReady)
		{
			PhotonNetwork.ConnectUsingSettings();
		}

		
		moves = null;

		//roomList = new List<RoomInfo>();

		if (gameObject.GetComponent<PhotonView>() == null)
		{
			pv = gameObject.AddComponent<PhotonView>();
			pv.ViewID = OpponentManager.getViewID();
		}
		else
		{
			pv = gameObject.GetComponent<PhotonView>();
		}



	}

	public int getNumPlayers()
	{
		return PhotonNetwork.PlayerList.Length;
	}

	public override void OnJoinedLobby()
	{
		connectedToLobby = true;
		Debug.Log("Joined Lobby");
	}

	public override void OnConnected()
	{
		//DEBUG: Test if connection was successful
		Debug.Log("Well we connected");
	}

	public override void OnRoomListUpdate(List<RoomInfo> rooms)
	{
		Debug.Log("Created room!");
		roomList = rooms;
	}
	
	public void disconnect()
	{

		PhotonNetwork.LeaveLobby();
		PhotonNetwork.Disconnect();
	}

	public override void OnConnectedToMaster()
	{
		//string roomName = GameObject.Find("Opponent").GetComponent<OpponentManager>().roomName;
		connected = true;
		Debug.Log(PhotonNetwork.CloudRegion);
		if (!PhotonNetwork.InLobby)
		{
			Debug.Log(PhotonNetwork.InLobby);
			try
			{

				PhotonNetwork.JoinLobby(TypedLobby.Default);
			}
			catch(Exception e)
			{
				Debug.Log(e.Message);
			}
		}
		//if (host)
		//{
		//	//Create Room
		//	PhotonNetwork.CreateRoom(roomName);

		//	//DEBUG: Test if room was created
		//	Debug.Log("Created a room!");
		//}
		//else
		//{
		//	//join the room
		//	PhotonNetwork.JoinRoom(roomName);

		//	//DEBUG: Test if room was joined
		//	Debug.Log("Joined a room!");
		//}
	}

	public void joinRoom(string roomName)
	{
		this.roomName = roomName;
		StartCoroutine(joinR(roomName));
	}

	public void hostRoom(string roomName)
	{
		this.roomName = roomName;
		StartCoroutine(hostR(roomName));
	}

	IEnumerator joinR(string roomName)
	{
		yield return new WaitUntil(isConnected);

		PhotonNetwork.JoinRoom(roomName);
	}

	IEnumerator hostR(string roomName)
	{
		yield return new WaitUntil(isConnected);

		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = maxPlayers;
		roomOptions.PlayerTtl = 60000;

		Debug.Log("Created room!");
		PhotonNetwork.CreateRoom(roomName, roomOptions);
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		Debug.Log("Player Disconnected " + otherPlayer.IsInactive);
		//throw new DisconnetException("Other Player Disconnected");
		connected = false;
		UnityEngine.GameObject.Find("GameManager").GetComponent<GameManager>().playerDisconnected();
	}

	public override void OnPlayerEnteredRoom(Player pl)
	{
		GameObject disc = GameObject.Find("OppDisconnect");
		if (disc != null)
		{
			connected = true;
			Destroy(disc);
		}
		Debug.Log("Entered Room");
	}

	/*******************************************************
	Disconnect Recovery
	-OnDisconnect : Called if Disconnect is Detected
	-CanRecoverFromDisconnect : Finds if Reconnection is Possible
	-Recover : Attempts to Reconnect
	-OnJoinRoomFail : Detects if failure to connect to Room
	-OnApplicationQuit : Detects if Application was quit
	*******************************************************/
	//Stolen from pun tutorial (edited since)

	bool fullyExited()
	{
		return PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState == ExitGames.Client.Photon.PeerStateValue.Disconnected;
	}

	bool detectedDisconnect = false;
	public override void OnDisconnected(DisconnectCause cause)
	{
		if (PhotonNetwork.IsConnected || detectedDisconnect || exited) return;
		Debug.Log("Disconnect Detected");
		detectedDisconnect = true;
		//GameObject manager = UnityEngine.GameObject.Find("GameManager");
		//if (manager != null) {
		//	manager.GetComponent<GameManager>().meDisconnected();
		//}
		//Attempt to reconnect
		//gm.playerDisconnected();
		//meObject go = (GameObject)Instantiate(Resources.Load("Prefabs/PlayerDisconnect"));
		//go.name = "Disconnect";
		connected = false;
		//if(this.CanRecoverFromDisconnect(cause)){
			Debug.Log("Can Recover: Attempting to Recover: ");
			StartCoroutine(tryConnect());
			//StartCoroutine(abortIn60());

			//if(this.Recover()){
			//	Debug.Log("Recover Successful");
			//}
			//else{
			//	Debug.LogError("Recover Failure: Failed to reconnect");
			//}
		//}
		//else
		//{
			//Debug.LogError("Can't reconnect: CanRecoverFromDisconnect returned False");
		//}

	}

	public bool isInRoom() { 
		return PhotonNetwork.InRoom;
	}
	bool connectedToInternet() { return Application.internetReachability != NetworkReachability.NotReachable; }
	IEnumerator tryConnect()
	{
        Debug.Log("line 249");
		yield return new WaitUntil(fullyExited);
        Debug.Log("line 251");
		//yield return new WaitUntil(connectedToInternet);
		//PhotonNetwork.ReconnectAndRejoin();
		//yield return new WaitUntil(isInRoom);
		while (!isInRoom())
		{
			PhotonNetwork.ReconnectAndRejoin();
			yield return new WaitForSeconds(.2f);
		}
		//PhotonNetwork.RejoinRoom(this.roomName);
		//StartCoroutine(joinR(this.roomName));
		Debug.Log("Made it inside reconnect");
		Debug.Log(PhotonNetwork.InRoom);
		GameObject go = GameObject.Find("MeDisconnect");
		Debug.Log("Go: " + go.name);
		if(go != null)
		{
			Destroy(go);
		}
		connected = true;
		detectedDisconnect = false;
		//gm.playerReconnected();
		
	}

	IEnumerator abortIn60()
	{
		yield return new WaitForSeconds(60);
		StopCoroutine(tryConnect());
		SceneManager.LoadScene("Main Menu");
	}

	//This will probably change to IEnumarator
	private bool CanRecoverFromDisconnect(DisconnectCause cause)
	{
		//This must be true in order to reconnect, & game must exist
		//if(PlayerTTL != 0 && !GameDoesNotExist){
			switch (cause)
			{
				// Possible to reconnect if one of these methods:
				case DisconnectCause.Exception:
				case DisconnectCause.ServerTimeout:
				case DisconnectCause.ClientTimeout:
				case DisconnectCause.DisconnectByServerLogic:
				case DisconnectCause.DisconnectByServerReasonUnknown:

				return true;
			}

		//}
		return false;
	}

	//For catching harder errors
	//May not be needed?
	private void OnJoinRoomFailed(){
		Debug.Log("Failed to Join Room");
	}

	private bool Recover()
	{
		//Attempt to recover
		if (!PhotonNetwork.ReconnectAndRejoin()){
			Debug.LogError("ReconnectAndRejoin failed, trying Reconnect");

			if (!PhotonNetwork.Reconnect()){
				Debug.LogError("MasterConnect failed, trying Reconnect to Master");
					if (!PhotonNetwork.ConnectUsingSettings()){
						Debug.LogError("ConnectUsingSettings failed");
						//Reconnect Failed
						return false;
					}
			}
		}
		else{
			Debug.LogError("CanRecoverFromDisconnect: Returned False");
			return false;
		}

		//Successfully Reconnected
		Debug.Log("Reconnect Successful");
		return true;
		
	}
	//Use RoomOptions.PlayerTtl != 0 and call PhotonNetwork.ReconnectAndRejoin() or PhotonNetwork.RejoinRoom(roomName);.
	//Detect if Left Room
	bool exited = false;
	  private void OnApplicationQuit(){
			exited = true;
		  Debug.Log("Application Quit Detected");
	    //PhotonNetwork.LeaveRoom();
	  //  PhotonNetwork.SendOutgoingCommands();
	}

	//Event subscriber that sets the flag

/***********************************************
Sending Network Packages

***********************************************/
	[PunRPC]
	public void acceptMove(string m1, string m2)
	{
		//Recieve a move from opponent
		moved = true;

		//DEBUG: Prompt if moves are recieved
		if(moves != null){
			Debug.Log("Recieved moves: "  + moves.Item1 + ", " + moves.Item2 +" succesfully.");
		}
		moves = Serialize.deserialize(m1, m2);
	}

	[PunRPC]
	public void acceptTags(string tag1, string tag2)
	{
		Debug.Log("Tags: " + tag1 + ", " + tag2);
		//Recieve Tags from the opponent
		_tag1 = tag1;
		_tag2 = tag2;
	}

	[PunRPC]
	public void acceptReady(bool r)
	{
		//Recieve ready status from the opponent
		ready = r;
	}


	bool checkHappened()
	{
		//Return true if move was successful
		return moved;
	}
	//Coroutine that waits until the flag is set
	IEnumerator WaitForEvent()
	{
		//Wait until moves are recieved, then reset the moved bool
		yield return new WaitUntil(checkHappened);
		moved = false;
	}

	//Main coroutine, that stops and waits somewhere within it's execution
	public IEnumerator get()
	{
		yield return StartCoroutine(WaitForEvent());
		//Oher stuff...
	}

	public Tuple<Move, Move> getMoves()
	{
		return moves;
	}

	public Tuple<Move, Move> consumeMoves()
	{
		Tuple<Move, Move> m = moves;
		moves = null;
		return m;
	}

	/***********************************************
	Pinging other player

	***********************************************/
	bool pinged = false;
	public bool getPinged() { 
		if(pinged) {
			pinged = false;
			return true;
		}
		return false;
	}
	[PunRPC] 
	public void returnPing()
	{

		UnityEngine.Debug.Log("Got return ping!");
		pinged = true;
	}
	[PunRPC]
	public void acceptPing()
	{
		UnityEngine.Debug.Log("Calling return ping!");
		pv.RPC("returnPing", RpcTarget.Others);
	}
	public void ping()
	{
		UnityEngine.Debug.Log("Calling acceptPing!");
		pv.RPC("acceptPing", RpcTarget.Others);
	}
}
