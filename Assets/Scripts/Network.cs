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

	public void SendMove(Tuple<Move, Move> moves)
	{
		ns.sendMoves(moves);
	}
	

}
