﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine.XR;

namespace IMRE.HandWaver.Networking
{

	public class playerHead : MonoBehaviourPunCallbacks
	{

		#region Constant Values

		const int MAXPLAYERCOUNT = 4;

		#endregion



		[Tooltip("Player Hands")]
		public playerHand lHand;
		public playerHand rHand;

		[Space]

		[Tooltip("Player Information")]
		public string playerName = "DEFAULTNAME";

		[Range(0, MAXPLAYERCOUNT)]
		public int playerPort;

		public TMPro.TextMeshPro text;

		#region Mono Funcs
		private void Start()
		{
			if (photonView.IsMine)
			{

				GetComponent<MeshRenderer>().enabled = false;
				UnityEngine.XR.InputTracking.trackingAcquired += notifyTracked;
				UnityEngine.XR.InputTracking.trackingLost += notifyNotTracked;
			}
		}

		private void notifyNotTracked(XRNodeState obj)
		{
			photonView.RPC("showHand", RpcTarget.OthersBuffered, false);
		}

		private void notifyTracked(XRNodeState obj)
		{
			photonView.RPC("showHand", RpcTarget.OthersBuffered, true);
		}

		private void Update()
		{
			if (photonView.IsMine)
			{
				transform.position = Camera.main.transform.position;
				transform.rotation = Camera.main.transform.rotation;
			}
		}
		#endregion

		public void setupPlayer(int newPlayerNumber, Color newPlayerColor)
		{
			this.playerPort = newPlayerNumber;
			setColor(newPlayerColor);
			this.name = "Player " + PhotonNetwork.NickName;
			lHand.name = "Player Hand (" + newPlayerNumber+"L)";
			rHand.name = "Player Hand (" + newPlayerNumber+"R)";
			text.text = PhotonNetwork.NickName;
		}

		[PunRPC]
		private void showHead(bool show)
		{
			GetComponent<MeshRenderer>().enabled = show;
		}



		private void setColor(Color newColor)
		{
			GetComponent<MeshRenderer>().materials[0].color = newColor;
			lHand.GetComponent<MeshRenderer>().materials[0].color = newColor;
			rHand.GetComponent<MeshRenderer>().materials[0].color = newColor;
		}

	}
}