using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AvatarAI : MonoBehaviour
{
	[SerializeField] float minIdleTime = 2;
	[SerializeField] float maxIdleTime = 15;

	[SerializeField] float offsetDist = 0.5f;

	public Vector3 Velocity
	{
		get
		{
			if (agent.enabled)
				return agent.velocity;

			return velocity;
		}
	}

	NavMeshAgent agent;
	You you;
	AiNode[] nodes;
	Coroutine currentRoutine;
	Vector3 velocity;

	void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
		agent.enabled = false;

		you = GetComponent<You>();
		nodes = FindObjectsByType<AiNode>(FindObjectsInactive.Include, FindObjectsSortMode.None);
	}

	public void Enter()
	{
		StartAiRoutine(EnterLab);
	}

	public void Leave()
	{
		StartAiRoutine(LeaveLab);
	}

	IEnumerator EnterLab()
	{
		yield return FollowPath(AiPathNode.FindByName("Lab Entrance"));

		you.FacingDirection = You.FacingDir.RIGHT;
		yield return new WaitForSeconds(1.5f);

		StartAiRoutine(Wander);
	}

	IEnumerator LeaveLab()
	{
		agent.enabled = true;
		agent.destination = AiPathNode.FindByName("Lab Exit").Position;

		while (agent.remainingDistance > agent.stoppingDistance)
		{
			yield return null;
		}

		velocity = Vector3.zero;
		agent.enabled = false;
		you.FacingDirection = You.FacingDir.RIGHT;

		yield return new WaitForSeconds(1.5f);

		yield return FollowPath(AiPathNode.FindByName("Lab Exit"));

		Destroy(you.gameObject);
	}

	IEnumerator Wander()
	{
		while (true)
		{
			Vector3 offset = new(UnityEngine.Random.Range(0, offsetDist), 0, UnityEngine.Random.Range(0, offsetDist));
			agent.destination = PickRandomNode().Position + offset;

			while (agent.remainingDistance > agent.stoppingDistance)
			{
				yield return null;
			}

			yield return new WaitForSeconds(UnityEngine.Random.Range(minIdleTime, maxIdleTime));
		}
	}

	void StartAiRoutine(Func<IEnumerator> routine)
	{
		if (currentRoutine != null)
			StopCoroutine(currentRoutine);

		currentRoutine = StartCoroutine(routine());
	}

	AiNode PickRandomNode()
	{
		return nodes[UnityEngine.Random.Range(0, nodes.Length)];
	}

	IEnumerator FollowPath(AiPathNode pathStart)
	{
		agent.enabled = false;

		AiPathNode nextNode = pathStart;

		while (nextNode)
		{
			// todo: this code has gotten fucked up over time, use a lerp
			Vector3 end = nextNode.Position;

			while (true)
			{
				Vector3 direction = (end - transform.position).normalized;
				float moveAmt = agent.speed * Time.deltaTime;

				Vector3 move = moveAmt * direction;
				float requiredDist = Vector3.Distance(transform.position, end);

				if (move.magnitude > requiredDist)
				{
					transform.position = end;
					break;
				}

				transform.position += move;
				velocity = move / Time.deltaTime;

				yield return null;
			}
			nextNode = nextNode.Next;
		}

		agent.enabled = true;
	}
}
