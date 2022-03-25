using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;

public class Network : Opponent
{
	NetworkServer ns;
	bool host;
	string roomName;
	GameObject server;
	const int MAXPLAYERS = 2;

	

	public Network()
	{
		server = new GameObject("Server");
		GameObject.DontDestroyOnLoad(server.gameObject);
		server.AddComponent<NetworkServer>();
		ns = server.GetComponent<NetworkServer>();

		

		//ns.host = this.host;
		//ns.roomName = this.roomName;
	}

	public void HostRoom(string roomN)
	{
		ns.host = true;
		ns.hostRoom(roomN);

	}

	public void JoinRoom(string roomName)
	{
		ns.host = false;
		ns.joinRoom(roomName);
	}

	public void OnJoinedRoom(){
		if(PhotonNetwork.PlayerList.Length < 2){
			ns.joinRoom(roomName);
		}
		else{
			Debug.LogError("Error: Already 2 people in room! Disconnecting from Room...");
			PhotonNetwork.LeaveRoom();
			//May need some sort of Change scene after this
		}
	}

	public void disconnect()
	{
		ns.disconnect();
	}

	public List<RoomInfo> rooms()
	{
		return ns.roomList;
	}

	public override bool HasMove()
	{

		return ns.getMoves() != null;
	}

	public override Tuple<Move, Move> GetMove(Gamecore.GameController gc) {
		ns.get();
		return ns.consumeMoves();
	}

	public override Tuple<Move, Move> GetWorkerPlacements(Gamecore.GameController gc)
	{
		ns.get();
		return ns.consumeMoves();
	}

	public override Tuple<string, string> GetWorkerTags()
	{
		return new Tuple<string, string>(ns._tag1, ns._tag2);
	}
	public override bool GetReady()
	{
		return ns.ready;
	}

	public override void SendReady(bool r)
	{
		ns.sendReady(r);
	}
	public override void SendWorkerPlacements(Tuple<Move, Move> m)
	{
		ns.sendMoves(m);
	}
	public override void SendWorkerTags(string s1, string s2)
	{
		ns.sendTags(s1, s2);
	}

	public override void SendMoves(Tuple<Move, Move> moves)
	{
		ns.sendMoves(moves);
	}


	public int getNumPlayers()
	{
		return ns.getNumPlayers();

	}

	public bool connected()
	{
		return ns.connected;
	}

}
