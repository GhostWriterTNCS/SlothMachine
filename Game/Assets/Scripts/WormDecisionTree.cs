using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormDecisionTree : MonoBehaviour
{
    public float reactionTime = 2f;
    public string targetTag = "Player";
    private DecisionTree dt;
    // Start is called before the first frame update
    void Start()
    {
        // Define actions
        DTAction a1 = new DTAction(moveToBoundaries);
        DTAction a2 = new DTAction(attackRandomly);
        DTAction a3 = new DTAction(attackBestPlayer);

        // Define decisions
        DTDecision d1 = new DTDecision(checkLastAttack);
        DTDecision d2 = new DTDecision(checkRobotAround);
        

        // Link action with decisions
        d1.AddLink(false, d2);
        d1.AddLink(true, a1);

        d2.AddLink(true, a2);
        d2.AddLink(false, a3);

        // Setup my DecisionTree at the root node
        dt = new DecisionTree(d1);

      
        StartCoroutine(Hunt());
    }


    // Take decision every interval, run forever
    public IEnumerator Hunt()
    {
        while (true)
        {
            dt.walk();
            yield return new WaitForSeconds(reactionTime);
        }
    }

    //check how many second he attacks a player
    public object checkLastAttack(object o)
    {
        return null;
    }

    //check the nearest player in the range
    public object checkRobotAround(object o)
    {
        return null;
    }




    //attack a enemy random in the range
    public object attackRandomly(object o)
    {
        return null;
    }


    //move randomly for an amount of time near the boundaries of the map
    public object moveToBoundaries(object o)
    {
        return null;
    }


    //attack the player who has the highest score
    public object attackBestPlayer(object o)
    {
        return null;
    }


}
