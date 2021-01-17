using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HidingMgr : MonoBehaviour
{
    [SerializeField] Collider2D[] obstacles;
    [SerializeField] Transform player;
    [SerializeField] Transform enemy;
    [SerializeField] Transform enemyPatrolTarget;
    [SerializeField] public float resetSec;
    Coroutine resetCoroutine;
    private void OnEnable()
    {
        obstacles = FindObjectsOfType<Collider2D>().Where((c) => (1 << c.gameObject.layer & LayerMask.GetMask("Obstacle")) > 0).ToArray();
        resetCoroutine = StartCoroutine(HidingReset());
    }

    private void OnDisable()
    {
        resetSec = 0f;
        StopCoroutine(resetCoroutine);
    }


    IEnumerator HidingReset()
    {
        while(true)
        {
            yield return null;
            resetSec += Time.deltaTime;

            if (resetSec >= 7f)
            {
                resetSec = 0f;
                ResetPoses();
            }
        }
    }


    private void ResetPoses()
    {
        Vector2 newPlayerPos;
        do
        {
            newPlayerPos = new Vector2(Random.Range(-8f, 8f), Random.Range(-4.5f, 4.5f));
        } while (!IsPosOverlapByObstacles(newPlayerPos));


        Vector2 newEnemyPos;
        do
        {
            newEnemyPos = new Vector2(Random.Range(-8f, 8f), Random.Range(-4.5f, 4.5f));
        } while (!IsPosOverlapByObstacles(newEnemyPos));

        Vector2 newPatrolTargetPos;
        do
        {
            newPatrolTargetPos = new Vector2(Random.Range(-8f, 8f), Random.Range(-4.5f, 4.5f));
        } while (!IsPosOverlapByObstacles(newPatrolTargetPos));

        player.position = newPlayerPos;
        enemy.position = newEnemyPos;
        enemyPatrolTarget.position = newPatrolTargetPos;
        player.GetComponent<SteeringBehaviors>().hasHidingSpot = false;
    }

    private bool IsPosOverlapByObstacles(Vector2 pos)
    {
        foreach(var ob  in obstacles)
        {
            if (ob.OverlapPoint(pos)) return true;
        }
        return false;
    }
}
