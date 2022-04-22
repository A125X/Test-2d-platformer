using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AttackEnemyAgent : Agent
{
    [SerializeField] private Transform enemyTransform;
    [SerializeField] private EnemyMovement enemyMovement;
    private void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
    }

    public override void OnEpisodeBegin()
    {
        //base.OnEpisodeBegin();
        gameObject.SetActive(true);
        transform.position = new Vector2(
            Random.Range(-16f, 16f), 
            Random.Range(-8f, 8f));
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        //base.CollectObservations(sensor);
        sensor.AddObservation(transform.position);
        sensor.AddObservation(enemyTransform.position);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        //base.OnActionReceived(actions);
        //Debug.Log(actions.ContinuousActions[0]);
        enemyMovement.inputValues = new float[] { 
            (actions.ContinuousActions[0] + 1.0f) / 2.0f, 
            (actions.ContinuousActions[0] + 1.0f) / 2.0f, 
            0.0f};
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            SetReward(1f);
            EndEpisode();
        }
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy)
            EndEpisode();
    }
}
