using System.Linq;
using UnityEngine;

public class AiPathNode : MonoBehaviour
{
	[SerializeField] AiPathNode next;
	[SerializeField] string nodeName;

	public string Name => nodeName;
	public Vector3 Position => transform.position;
	public AiPathNode Next => next;

	public static AiPathNode FindByName(string name)
	{
		return FindObjectsByType<AiPathNode>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
			.FirstOrDefault(p => p.Name == name);
	}
}
