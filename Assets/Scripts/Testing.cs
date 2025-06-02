using UnityEngine;
using UnityUtils;

namespace DefaultNamespace
{
	public class Testing : MonoBehaviour
	{
		private float angle = 30f;
		Vector3 plane = new Vector3(0, 0, 1);
		void Update()
		{

			var newPos = transform.position.RotateAroundPoint(Vector3.zero, angle * Time.deltaTime, 10f,plane);
			transform.position = newPos;
			//
		}
	}
}