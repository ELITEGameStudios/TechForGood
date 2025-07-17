using UnityEngine;

public class AiNode : MonoBehaviour
{
	public enum NodeType
	{
		// AI can idle here
		Normal,

		// AI does a special action
		Special
	}

	public Vector3 Position => transform.position;
}
