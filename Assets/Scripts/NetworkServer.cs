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

	public string gameVersion = "0.1";

	public bool host;
	public string roomName;

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


	public void Start()
	{
		PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = gameVersion;
		PhotonNetwork.ConnectUsingSettings();

		moves = null;

		if (gameObject.GetComponent<PhotonView>() == null)
		{
			pv = gameObject.AddComponent<PhotonView>();
			pv.ViewID = 1;
		}
		else
		{
			pv = gameObject.GetComponent<PhotonView>();
		}

	}

	public override void OnConnected()
	{
		//DEBUG: Test if connection was successful
		Debug.Log("Well we connected");
	}


	public override void OnConnectedToMaster()
	{
		//string roomName = GameObject.Find("Opponent").GetComponent<OpponentManager>().roomName;
		if (host)
		{
			//Create Room
			PhotonNetwork.CreateRoom(roomName);

			//DEBUG: Test if room was created
			Debug.Log("Created a room!");
		}
		else
		{
			//join the room
			PhotonNetwork.JoinRoom(roomName);

			//DEBUG: Test if room was joined
			Debug.Log("Joined a room!");
		}
	}

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
