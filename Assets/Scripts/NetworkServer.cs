using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class NetworkServer : MonoBehaviourPunCallbacks, IConnectionCallbacks
{
	//Flag
	bool moved;
	public bool ready = false;
	public string _tag1;
	public string _tag2;
	public int joinedRoom = 0;

	GameManager gm;
	//MainMenu mm;
	public List<RoomInfo> roomList;

	private string roomName;
	public string gameVersion = "0.1";

	private byte maxPlayers = 2;

	public bool host;
	//public string roomName;

	//private LoadBalancingClient loadBalance;

	public bool connected = false;// PhotonNetwork.IsConnectedAndReady;
	public bool connectedToLobby = false;// PhotonNetwork.InLobby;

	PhotonView pv;
	Tuple<Move, Move> moves;

	public void sendMoves(Tuple<Move, Move> _moves) { StartCoroutine(_sendMoves(_moves)); }
	public IEnumerator _sendMoves(Tuple<Move, Move> moves)
	{

		connected = false;
		StartCoroutine(checkInternetConnection((isConnected) => {
			if (!isConnected)
			{
				connected = false;
			}
			else
			{
				connected = true;
			}
		}));

		yield return new WaitUntil(() => connected);
		getPinged();
		while (!getPinged())
		{
			ping();
			yield return new WaitForSeconds(.1f);
		}
		//Debug.Log(moves);
		Tuple<string, string> ss = Serialize.serialize(moves);
		pv.RPC("acceptMove", RpcTarget.Others, ss.Item1, ss.Item2);
	}

	public void sendTags(string t1, string t2) { StartCoroutine(_sendTags(t1, t2)); }
	public IEnumerator _sendTags(string t1, string t2)
	{
		connected = false;
		StartCoroutine(checkInternetConnection((isConnected) => {
			if (!isConnected)
			{
				connected = false;
			}
			else
			{
				connected = true;
			}
		}));

		yield return new WaitUntil(() => connected);

        while (!getPinged())
        {
            ping();
            yield return new WaitForSeconds(.1f);
        }
		//Send tags
		pv.RPC("acceptTags", RpcTarget.Others, t1, t2);
	}

	public void sendReady(bool r) { StartCoroutine(_sendReady(r)); }
	public IEnumerator _sendReady(bool r)
	{
		connected = false;
		StartCoroutine(checkInternetConnection((isConnected) => {
			if (!isConnected)
			{
				connected = false;
			}
			else
			{
				connected = true;
			}
		}));

		yield return new WaitUntil(() => connected);
		while (!getPinged())
		{
			ping();
			yield return new WaitForSeconds(.1f);
		}
		//Send ready Status
		pv.RPC("acceptReady", RpcTarget.Others, r);
	}

	bool isConnected()
	{
		return connected && PhotonNetwork.InLobby;
	}
	public void Start()
	{
		//Debug.Log("NS Start");
		PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = gameVersion;
		if (!PhotonNetwork.IsConnectedAndReady )
		{
			if(PhotonNetwork.IsConnected)
			{
				PhotonNetwork.Disconnect();
			}
			//Debug.Log("Connecting!");
			StartCoroutine(connectUsingWaitSettings());
		}
			StartCoroutine(checkInternetConnection((isConnected) => {
				if (!isConnected)
				{
					StartCoroutine(connectUsingWaitSettings());
				}
			}));


		roomList = new List<RoomInfo>();
		

		//Debug.Log(PhotonNetwork.InLobby);
		//Debug.Log(PhotonNetwork.InRoom);
		
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

		if (PhotonNetwork.InRoom)
		{
			sendDiscOnPurpose();
			PhotonNetwork.LeaveRoom();
		}

		connected =  PhotonNetwork.IsConnectedAndReady;
	    connectedToLobby =  PhotonNetwork.InLobby;

	}

	public static IEnumerator checkInternetConnection(Action<bool> syncResult)
	{
		const string echoServer = "https://www.harding.edu/";

		bool result;
		using (var request = UnityWebRequest.Head(echoServer))
		{
			request.timeout = 5;
			yield return request.SendWebRequest();
			result = !request.isNetworkError && !request.isHttpError && request.responseCode == 200;
		}
		syncResult(result);
	}

	IEnumerator connectUsingWaitSettings()
	{
		
		while (!PhotonNetwork.IsConnected)
		{
			StartCoroutine(checkInternetConnection((isConnected) => {
				if (isConnected)
				{
					PhotonNetwork.ConnectUsingSettings();
				}
			}));
			

			yield return new WaitForSeconds(.1f);
		}
		connected = true;
	}

	IEnumerator reconnectAfterGame()
	{
		yield return new WaitUntil(fullyExited);
		PhotonNetwork.ConnectUsingSettings();
	}

	public int getNumPlayers()
	{
		return PhotonNetwork.PlayerList.Length;
	}

	public override void OnJoinedLobby()
	{
		connectedToLobby = true;
		//Debug.Log("Joined Lobby");
	}

	public override void OnConnected()
	{
		//DEBUG: Test if connection was successful
		//Debug.Log("Well we connected");
	}

	public override void OnRoomListUpdate(List<RoomInfo> rooms)
	{
		//Debug.Log("Updated room list!");
		roomList = rooms;
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		StartCoroutine(hostR(roomName));
		Debug.Log("Overriden onCreateRoomFailed");
	}

	public void disconnect() { StartCoroutine(_disconnect()); }
	public IEnumerator _disconnect()
	{
		//Debug.Log("Disconnecting");
		//discOnPurpose = true;
		sendDiscOnPurpose();

		while (PhotonNetwork.CountOfPlayersInRooms > 1 && !getPinged())
		{
			ping();
			yield return new WaitForSeconds(.1f);
		}

		PhotonNetwork.LeaveLobby();
		PhotonNetwork.Disconnect();
	}

	public override void OnConnectedToMaster()
	{
		//string roomName = GameObject.Find("Opponent").GetComponent<OpponentManager>().roomName;
		connected = true;
		//Debug.Log(PhotonNetwork.CloudRegion);
		if (!PhotonNetwork.InLobby)
		{
			//Debug.Log(PhotonNetwork.InLobby);
			try
			{

				PhotonNetwork.JoinLobby(TypedLobby.Default);
			}
			catch(Exception e)
			{
				//Debug.Log(e.Message);
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
		Debug.Log("Inside joinR");
		Debug.Log(connected);
		Debug.Log(PhotonNetwork.InLobby);
		yield return new WaitUntil(isConnected);
		Debug.Log("after waitFor in joinR");

		PhotonNetwork.JoinRoom(roomName);
		OnJoinedRoom();
	}

	public override void OnJoinedRoom(){
		joinedRoom = 1;
		
		if(PhotonNetwork.PlayerList.Length > 2){
			disconnect();
			
		}
		//else if (PhotonNetwork.PlayerList.Length == 0){

		//}
		else if(PhotonNetwork.PlayerList.Length == 2) {
			PhotonNetwork.CurrentRoom.IsVisible = false;
		}
	}

	IEnumerator hostR(string roomName)
	{
		yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady);

		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = maxPlayers;
		roomOptions.PlayerTtl = 60000;
		roomOptions.EmptyRoomTtl = 60000;
		PhotonNetwork.CreateRoom(roomName, roomOptions);
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		//if (getOppDiscOnPurpose()) return;
		connected = false;
		GameObject go = UnityEngine.GameObject.Find("GameManager");
		if (go != null)
		{
			if (getOppDiscOnPurpose())
			{
				go.GetComponent<GameManager>().otherPlayerQuit();
			}
			else
			{
				go.GetComponent<GameManager>().playerDisconnected();
			}
		}
	}

	public override void OnPlayerEnteredRoom(Player pl)
	{
		GameObject disc = GameObject.Find("OppDisconnect");
		if (disc != null)
		{
			connected = true;
			Destroy(disc);
		}
	}

	bool fullyExited()
	{
		return PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState == ExitGames.Client.Photon.PeerStateValue.Disconnected;
	}

	bool detectedDisconnect = false;
	public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.Log("My onDisconnect");
		if(!PhotonNetwork.IsConnectedAndReady)
		{
			StartCoroutine(tryConnect());
		}
		else if(getDiscOnPurpose())
		{
			//Handle quitting game
		}
		else if (PhotonNetwork.IsConnected || detectedDisconnect || exited) return;
		
		else {
			Debug.Log("Here");
			detectedDisconnect = true;
			connected = false;
			StartCoroutine(tryConnect());
		}

	}

	public bool isInRoom() { 
		return PhotonNetwork.InRoom;
	}
	bool connectedToInternet() { return Application.internetReachability != NetworkReachability.NotReachable; }

	IEnumerator tryConnect()
	{
		Debug.Log("Before wait for");
		yield return new WaitUntil(fullyExited);
		Debug.Log("After wait for");
		//yield return new WaitUntil(connectedToInternet);
		//PhotonNetwork.ReconnectAndRejoin();
		//yield return new WaitUntil(isInRoom);
		if (joinedRoom == 1)
		{
			while (!isInRoom())
			{

				PhotonNetwork.ReconnectAndRejoin();
				yield return new WaitForSeconds(.2f);
			}
		} 
		else
		{
			while(!PhotonNetwork.InLobby)
			{
				PhotonNetwork.Reconnect();
				yield return new WaitForSeconds(.2f);
			}
			if(roomName != "" && !isInRoom())
			{
				if(host)
				{
					StartCoroutine(hostR(roomName));
				}
			}
		}
		Debug.Log("After while loop");
		//PhotonNetwork.RejoinRoom(this.roomName);
		//StartCoroutine(joinR(this.roomName));
		GameObject go = GameObject.Find("MeDisconnect");
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

	public int getJoinedRoom()
	{
		if(PhotonNetwork.InRoom)
		{
			joinedRoom = 1;
		}
		return joinedRoom;
	}
	public override void OnJoinRoomFailed(short ret, string mess){
		Debug.Log("Failed to Join Room");
		joinedRoom = -1;
	}

	//Use RoomOptions.PlayerTtl != 0 and call PhotonNetwork.ReconnectAndRejoin() or PhotonNetwork.RejoinRoom(roomName);.
	//Detect if Left Room
	bool exited = false;
	  private void OnApplicationQuit(){
			exited = true;
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
		moves = Serialize.deserialize(m1, m2);
	}

	[PunRPC]
	public void acceptTags(string tag1, string tag2)
	{
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

		pinged = true;
	}
	[PunRPC]
	public void acceptPing() { 
		pv.RPC("returnPing", RpcTarget.Others);
	}
	public void ping()
	{
		pv.RPC("acceptPing", RpcTarget.Others);
	}

	/***********************************************
	Chat functionality

	***********************************************/
	string chatMessage = "";
	[PunRPC]
	public void acceptChatMessage(string chatM)
	{
		UnityEngine.Debug.Log("Accept chat called");
		chatMessage = chatM;
	}

	public void sendChatMessage(string chatM)
	{
		StartCoroutine(_sendChatMessage(chatM));
		
	}
	public IEnumerator _sendChatMessage(string chatM)
	{
		yield return new WaitUntil(() => connected);

		while (!getPinged())
		{
			ping();
			yield return new WaitForSeconds(.1f);
		}
		pv.RPC("acceptChatMessage", RpcTarget.Others, chatM);
	}

	public string getChatMessage() { return chatMessage; }
	public void clearChatMessage() { chatMessage = ""; }


	/***********************************************
	Purposeful disconnect

	***********************************************/
	//bool discOnPurpose = connected;
	bool oppDiscOnPurpose = false;

	[PunRPC] 
	public void acceptDiscOnPurpose()
	{
		UnityEngine.Debug.Log("Recieved a disc");
		oppDiscOnPurpose = true;
	}

	public void sendDiscOnPurpose()
	{
		pv.RPC("acceptDiscOnPurpose", RpcTarget.Others);
	}

	bool getDiscOnPurpose()
	{
		return SceneManager.GetActiveScene().name == "Main Menu";
	}
	bool getOppDiscOnPurpose()
	{
		if(oppDiscOnPurpose)
		{
			oppDiscOnPurpose = false;
			return true;
		}
		return false;
	}
}
