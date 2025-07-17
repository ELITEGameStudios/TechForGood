using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AvatarAI : MonoBehaviour
{
	[SerializeField] float minIdleTime = 2;
	[SerializeField] float maxIdleTime = 15;

	[SerializeField] float offsetDist = 0.5f;

	public NavMeshAgent Agent { get; private set; }

	AiNode[] nodes;
	Coroutine currentRoutine;

	void Awake()
	{
		Agent = GetComponent<NavMeshAgent>();

		nodes = FindObjectsByType<AiNode>(FindObjectsInactive.Include, FindObjectsSortMode.None);

		StartAiRoutine(Wander);
	}

	IEnumerator Wander()
	{
		while (true)
		{
			// Wait for seconds
			yield return new WaitForSeconds(UnityEngine.Random.Range(minIdleTime, maxIdleTime));

			Vector3 offset = new(UnityEngine.Random.Range(0, offsetDist), 0, UnityEngine.Random.Range(0, offsetDist));
			Agent.destination = PickRandomNode().Position + offset; ;

			while (Agent.pathStatus == NavMeshPathStatus.PathPartial)
			{
				yield return null;
			}
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
}
