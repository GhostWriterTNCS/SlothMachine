using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WormDecisionTree : MonoBehaviour
{

    [Range(0f, 20f)] public float range = 5f;
    [Range(0f, 10f)] public float moveSpeed = 1f;
    public float reactionTime = 2f;
    public float waitDuration = 10;
    public ParticleSystem particle;
    public GameObject wormEvolution;
    private DecisionTree dt;
    private Robot playerTarget;
    private Rigidbody myRigidbody;
    private float timer;
    private float timerArena;

    // Start is called before the first frame update
    void Start()
    {
        // Define actions
        DTAction a1 = new DTAction(moveToBoundaries);
        DTAction a2 = new DTAction(attackRandomly);

        // Define decisions
        DTDecision d1 = new DTDecision(checkLastAttack);
        DTDecision d2 = new DTDecision(checkRobotAround);
        

        // Link action with decisions
        d1.AddLink(false, d2);
        d1.AddLink(true, a1);

        d2.AddLink(true, a2);

        // Setup my DecisionTree at the root node
        dt = new DecisionTree(d1);
        myRigidbody = GetComponent<Rigidbody>();
        particle = GetComponent<ParticleSystem>();
        StartCoroutine(Hunt());

       
    }


    // Take decision every interval, run forever
    public IEnumerator Hunt()
    {
        while (true)
        {
            Debug.Log("Worm");
            dt.walk();
            yield return new WaitForSeconds(reactionTime);
        }
    }

    //DECISIONS
    //check how many second he attacks a player
    public object checkLastAttack(object o)
    {
        timer -= reactionTime;
        if(timer <= 0)
        {
            return false;
        }
        return true;
    }

    //check the nearest player in the range
    public object checkRobotAround(object o)
    {
        if(playerTarget)
        {
            return true;
        }
        Debug.Log("checkRobotAround");
        GetComponent<Collider>().enabled = true;
        particle.Play();
        System.Random rnd = new System.Random();
        Robot[] arr = FindObjectsOfType<Robot>().OrderBy(x => rnd.Next()).ToArray();
        foreach (Robot go in arr)
        {
            if ((go.transform.position - transform.position).magnitude <= range)
            {
                playerTarget = go;
                return true;
            }    
        }

        //attack the player who has the highest score
        int scoreMax = -1;
        foreach (Robot go in arr)
        {
            if (go.player.score > scoreMax)
            {
                scoreMax = go.player.score;
                playerTarget = go;
            }
        }
        return true;
    }


    //ACTIONS
    //attack a enemy random in the range
    public object attackRandomly(object o)
    {
        if (playerTarget)
        {
            Debug.Log("attackRandomly");
            myRigidbody.position = Vector3.MoveTowards(myRigidbody.transform.position, playerTarget.transform.position, moveSpeed);

        }
        return null;
    }


    //move randomly for an amount of time near the boundaries of the map
    public object moveToBoundaries(object o)
    {
        GetComponent<Collider>().enabled = false;
        particle.Stop();
        return null;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Robot>())
        {
            timerArena = FindObjectOfType<ArenaManager>().countdown.GetComponentInChildren<Countdown>().seconds;
            Debug.Log(timerArena);
            if (timerArena <= 90)
            {
                Debug.Log("timerArena < 1.30");
                Instantiate(wormEvolution, new Vector3(playerTarget.transform.position.x, playerTarget.transform.position.y-10, playerTarget.transform.position.z), Quaternion.identity);
                float specificCoordinate = wormEvolution.GetComponent<Rigidbody>().position.y + 100;

                Vector3 wormPosition = new Vector3(wormEvolution.GetComponent<Rigidbody>().position.x, wormEvolution.GetComponent<Rigidbody>().position.y, wormEvolution.GetComponent<Rigidbody>().position.z);

                wormEvolution.GetComponent<Rigidbody>().AddForce(0, 2500, 0);
                

            }
            Debug.Log("colpito");
            playerTarget = null;
            timer = waitDuration;
        }
    }

}
