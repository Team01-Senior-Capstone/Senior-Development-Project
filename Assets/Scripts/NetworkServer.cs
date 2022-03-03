using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class NetworkServer : MonoBehaviourPunCallbacks, IConnectionCallbacks
{
	//Flag
	bool moved;
	public bool ready = false;
	public string _tag1;
	public string _tag2;

	public List<RoomInfo> roomList;

	public string gameVersion = "0.1";

	private byte maxPlayers = 4;

	public bool host;
	//public string roomName;

	private bool connected = false;
	public bool connectedToLobby = false;

	PhotonView pv;
	Tuple<Move, Move> moves;

	public void sendMoves(Tuple<Move, Move> moves)
	{
		Tuple<string, string> ss = Serialize.serialize(moves);
		pv.RPC("acceptMove", RpcTarget.Others, ss.Item1, ss.Item2);
	}

	public void sendTags(string t1, string t2)
	{
		//Send tags
		pv.RPC("acceptTags", RpcTarget.Others, t1, t2);
	}


	public void sendReady(bool r)
	{
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
		StartCoroutine(joinR(roomName));
	}

	public void hostRoom(string roomName)
	{
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


		Debug.Log("Created room!");
		PhotonNetwork.CreateRoom(roomName, roomOptions);
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		Debug.Log("Player Disconnected " + otherPlayer.IsInactive);
		//throw new DisconnetException("Other Player Disconnected");
		UnityEngine.GameObject.Find("GameManager").GetComponent<GameManager>().playerDisconnected();
	}

	//Stolen from pun tutorial
	public override void OnDisconnected(DisconnectCause cause)
	{
		//Start Coroutine
	}

	//This will probably change to IEnumarator
	private bool CanRecoverFromDisconnect(DisconnectCause cause)
	{
		switch (cause)
		{
			// Figure out if back online
			case DisconnectCause.Exception:
			case DisconnectCause.ServerTimeout:
			case DisconnectCause.ClientTimeout:
			case DisconnectCause.DisconnectByServerLogic:
			case DisconnectCause.DisconnectByServerReasonUnknown:
				return true;
		}
		return false;
	}

	private void Recover()
	{
		if (!PhotonNetwork.ReconnectAndRejoin())
		{
			Debug.LogError("ReconnectAndRejoin failed, trying Reconnect");
			if (!PhotonNetwork.Reconnect())
			{
				Debug.LogError("Reconnect failed, trying ConnectUsingSettings");
				if (!PhotonNetwork.ConnectUsingSettings())
				{
					Debug.LogError("ConnectUsingSettings failed");
				}
			}

		}
	}
	//Use RoomOptions.PlayerTtl != 0 and call PhotonNetwork.ReconnectAndRejoin() or PhotonNetwork.RejoinRoom(roomName);.

	//  private void OnApplicationQuit()
	//{
	//    PhotonNetwork.LeaveRoom();
	//    PhotonNetwork.SendOutgoingCommands();
	//}


	//Event subscriber that sets the flag
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
		//Other stuff...
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
}
