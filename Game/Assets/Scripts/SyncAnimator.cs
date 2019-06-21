using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel = 4)]
public class SyncAnimator : NetworkBehaviour {
	Robot robot;
	Animator animator;
	Dictionary<string, int> triggers = new Dictionary<string, int>();

	void Start() {
		robot = GetComponent<Robot>();
		animator = GetComponent<Animator>();
	}

	public void SetTrigger(string id) {
		if (animator && !animator.GetBool(id)) {
			CmdSetTrigger(id);
		}
	}
	[Command]
	void CmdSetTrigger(string id) {
		RpcSetTrigger(id);
	}
	[ClientRpc]
	void RpcSetTrigger(string id) {
		if (robot) {
			if (id != "B") {
				robot.playerMove.isAttacking = true;
				robot.CmdGuardOff();
			}
			if (animator)
				animator.SetTrigger(id);
			if (!triggers.ContainsKey(id)) {
				triggers.Add(id, 1);
			} else {
				triggers[id] += 1;
			}
			string triggerID = id;
			StartCoroutine(DelayCall(() => ResetTrigger(triggerID), robot.comboDelay));
		}
	}

	public IEnumerator DelayCall(Action action, float delayTime) {
		yield return new WaitForSeconds(delayTime);
		action();
	}

	void ResetTrigger(string trigger) {
		triggers[trigger] -= 1;
		if (triggers[trigger] == 0) {
			if (animator)
				animator.ResetTrigger(trigger);
		}
	}

	public void SetFloat(string id, float value) {
		if (animator && animator.GetFloat(id) != value) {
			CmdSetFloat(id, value);
		}
	}
	[Command]
	void CmdSetFloat(string id, float value) {
		RpcSetFloat(id, value);
	}
	[ClientRpc]
	void RpcSetFloat(string id, float value) {
		if (animator)
			animator.SetFloat(id, value);
	}

	public void SetBool(string id, bool value) {
		if (animator && animator.GetBool(id) != value) {
			CmdSetBool(id, value);
		}
	}
	[Command]
	void CmdSetBool(string id, bool value) {
		RpcSetBool(id, value);
	}
	[ClientRpc]
	void RpcSetBool(string id, bool value) {
		if (animator)
			animator.SetBool(id, value);
	}
}
