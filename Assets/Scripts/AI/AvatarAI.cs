using UnityEngine;
using UnityEngine.AI;

public class AvatarAI : MonoBehaviour
{
    GameObject player;
    NavMeshAgent agent;
    [SerializeField] LayerMask ground_layer, player_layer;

    //Patrolling Variables
    Vector3 destination_point;
    bool walking_point_set;
    [SerializeField] float range;
    bool roam = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        Patrol();
    }

    void Patrol(){
        
            if (!walking_point_set){
            SearchForPoint();
            }

            if (walking_point_set){
                agent.SetDestination(destination_point);
            }

            if (Vector3.Distance(transform.position, destination_point) < 10) walking_point_set = false;
        
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
