using UnityEngine;
using UnityUtils;
using UnityUtils.Attributes;

namespace DefaultNamespace
{
	public class Testing : MonoBehaviour
	{
		[Required("Prefab is required")]
		public GameObject wowow;
		[Required("Prefab is required")]
		public GameObject qwe;
		[Required("Prefab is qwe")]
		public GameObject asd;
		
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