using UnityEngine;
using UnityEngine.AI;

public class AvatarAI : MonoBehaviour
{
    [SerializeField]
    NavMeshAgent agent;
    public NavMeshAgent GetAgent() {return agent;}
    [SerializeField] LayerMask ground_layer;

    //Patrolling Variables
    Vector3 destination_point;
    bool walking_point_set;
    [SerializeField] float range;
    float roaming_count = 0.0f;
    float not_roaming_count = 0.0f;
    float roam_timer = 0.0f;
    public float min_roam_time = 5.0f;
    public float max_roam_time = 15.0f;
    public float min_stand_time = 5.0f;
    public float max_stand_time = 15.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        roaming_count = Random.Range(min_roam_time, max_roam_time);
        not_roaming_count = Random.Range(min_stand_time, max_stand_time);
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        Patrol();
        roam_timer += Time.deltaTime;
        // Debug.Log(roam_timer);
    }

    void Patrol(){
        
        if (roam_timer < roaming_count){
            if (!walking_point_set){
            SearchForPoint();
            }

            if (walking_point_set){
                agent.SetDestination(destination_point);
            }

            if (Vector3.Distance(transform.position, destination_point) < 10) walking_point_set = false;

        }

        else{
            if (roam_timer > (roaming_count + not_roaming_count)){
                roam_timer = 0.0f;
                roaming_count = Random.Range(min_roam_time, max_roam_time);
                not_roaming_count = Random.Range(min_stand_time, max_stand_time);
            }
        }
        
    }

    void SearchForPoint(){
        float z_pos = Random.Range(-range, range);
        float x_pos = Random.Range(-range, range);

        destination_point = new Vector3(transform.position.x + x_pos, transform.position.y, transform.position.z + z_pos);

        if(Physics.Raycast(destination_point, Vector3.down, ground_layer)){
            walking_point_set = true;
        }
    }
}
