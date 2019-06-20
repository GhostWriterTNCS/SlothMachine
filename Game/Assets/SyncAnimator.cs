using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

 [NetworkSettings(channel=4)]
public class SyncAnimator : NetworkBehaviour {
    Robot robot;
    Dictionary<string, int> triggers = new Dictionary<string, int>();

    void Start()
    {
        robot = GetComponent<Robot>();
    }

    [Command]
    public void CmdSetTrigger(string trigger)
    {
        RpcSetTrigger(trigger);
    }
    [ClientRpc]
    public void RpcSetTrigger(string trigger)
    {
        if (robot)
        {
            if (trigger != "B")
            {
                robot.playerMove.isAttacking = true;
                robot.CmdGuardOff();
            }
            robot.animator.SetTrigger(trigger);
            string triggerID = trigger;
            if (!triggers.ContainsKey(trigger))
            {
                triggers.Add(trigger, 1);
            }
            else
            {
                triggers[trigger] += 1;
            }
            StartCoroutine(robot.DelayCall(() => ResetTrigger(triggerID), robot.comboDelay));
        }
    }
    void ResetTrigger(string trigger)
    {
        triggers[trigger] -= 1;
        if (triggers[trigger] == 0)
        {
            if (robot)
                robot.animator.ResetTrigger(trigger);
        }
    }

    [Command]
    public void CmdSetFloat(string id, float value)
    {
        RpcSetFloat(id, value);
    }
    [ClientRpc]
    public void RpcSetFloat(string id, float value)
    {
        if (robot)
            robot.animator.SetFloat(id, value);
    }

    [Command]
    public void CmdSetBool(string id, bool value)
    {
        RpcSetBool(id, value);
    }
    [ClientRpc]
    public void RpcSetBool(string id, bool value)
    {
        if(robot)
            robot.animator.SetBool(id, value);
    }
}
