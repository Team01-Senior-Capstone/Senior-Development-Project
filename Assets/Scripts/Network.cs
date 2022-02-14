using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;


public class Network : Opponent
{
	string roomName;
	bool host;
	NetworkServer ns;


	void Start(){
		PhotonNetwork.ConnectUsingSettings();
		
	}
	public Network(string roomName, bool h)
	{

		if(host)
		{
			//Create Room
			PhotonNetwork.CreateRoom(roomName);
			
		}
		else
		{
			//join the room
			PhotonNetwork.JoinRoom(roomName);
		
		}
	}

	public override Tuple<Move, Move> GetMove(Gamecore.GameController gc) {
		ns.get();
		return ns.getMoves();
	}

	public override Tuple<Move, Move> GetWorkerPlacements(Gamecore.GameController gc)
	{
		ns.get();
		return ns.getMoves();
	}

	public override Tuple<string, string> GetWorkerTags()
	{
		return new Tuple<string, string>(ns._tag1, ns._tag2);
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
	

}
