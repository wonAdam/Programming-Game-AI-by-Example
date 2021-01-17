using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterposeMgr : MonoBehaviour
{
    [SerializeField] AIMover agentA;
    [SerializeField] AIMover agentB;
    [SerializeField] AIMover player;
    [SerializeField] public Transform target;

    public bool agentAArrived;
    public bool agentBArrived;
    public bool playerArrived;

    public bool coroutineInProcess;
    // Start is called before the first frame update
    void Start()
    {
        SetAgentPlayerInRandomPos();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(target.position, player.transform.position) < 1f) playerArrived = true;
        if (Vector2.Distance(target.position, agentA.transform.position) < 1f) agentAArrived = true;
        if (Vector2.Distance(target.position, agentB.transform.position) < 1f) agentBArrived = true;


        if (!coroutineInProcess && agentAArrived && agentBArrived && playerArrived)
        {
            StartCoroutine(SetNewSceneDelay());
        }
    }

    IEnumerator SetNewSceneDelay()
    {
        coroutineInProcess = true;
        yield return new WaitForSeconds(3f);

        SetAgentPlayerInRandomPos();
        coroutineInProcess = false;
    }

    private void SetAgentPlayerInRandomPos()
    {
        agentA.transform.position = new Vector3(
            Random.Range(-8f, 8f),
            Random.Range(-4.5f, 4.5f),
            0f
            ) ;

        agentB.transform.position = new Vector3(
            Random.Range(-8f, 8f),
            Random.Range(-4.5f, 4.5f),
            0f
            );

        player.transform.position = new Vector3(
            Random.Range(-8f, 8f),
            Random.Range(-4.5f, 4.5f),
            0f
            );

        Vector2 midPointBtwAgents = (agentA.transform.position + agentB.transform.position) / 2.0f;
        Vector2 newTargetPos = midPointBtwAgents + new Vector2(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f));
        target.position = newTargetPos;


        agentAArrived = false; agentBArrived = false; playerArrived = false;
    }
}
