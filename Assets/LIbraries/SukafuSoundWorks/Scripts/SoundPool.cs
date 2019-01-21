using UnityEngine;
using System.Collections.Generic;

public static class SoundPool {
	/// <summary>
	/// The Pool class represents the pool for a particular prefab.
	/// </summary>
	public class Pool {
		// We append an id to the name of anything we instantiate.
		// This is purely cosmetic.
		private int _nextId=1;

		// The structure containing our inactive objects.
		// Why a Stack and not a List? Because we'll never need to
		// pluck an object from the start or middle of the array.
		// We'll always just grab the last one, which eliminates
		// any need to shuffle the objects around in memory.
		Stack<GameObject> inactive;

		// The prefab that we are pooling
		GameObject prefab;

		// Constructor
		public Pool(GameObject prefab, int initialQty) {
			this.prefab = prefab;

			// If Stack uses a linked list internally, then this
			// whole initialQty thing is a placebo that we could
			// strip out for more minimal code. But it can't *hurt*.
			inactive = new Stack<GameObject>(initialQty);
		}

		// Spawn an object from our pool
		public GameObject Spawn(Vector3 pos, Quaternion rot, Transform parent) {
			GameObject obj;
			if(inactive.Count==0) {
				// We don't have an object in our pool, so we
				// instantiate a whole new object.
				obj = GameObject.Instantiate(prefab, pos, rot, parent);
				obj.name = prefab.name + " ("+(_nextId++)+")";

				// Add a PoolMember component so we know what pool
				// we belong to.
				obj.AddComponent<PoolMember>().MyPool = this;
			}
			else {
				// Grab the last object in the inactive array
				obj = inactive.Pop();

				if(obj == null) {
					// The inactive object we expected to find no longer exists.
					// The most likely causes are:
					//   - Someone calling Destroy() on our object
					//   - A scene change (which will destroy all our objects).
					//     NOTE: This could be prevented with a DontDestroyOnLoad
					//	   if you really don't want this.
					// No worries -- we'll just try the next one in our sequence.

					return Spawn(pos, rot, parent);
				}
			}

			obj.transform.position = pos;
			obj.transform.rotation = rot;
			obj.SetActive(true);
			return obj;

		}

		// Return an object to the inactive pool.
		public void Despawn(GameObject obj) {
			obj.SetActive(false);

			// Since Stack doesn't have a Capacity member, we can't control
			// the growth factor if it does have to expand an internal array.
			// On the other hand, it might simply be using a linked list 
			// internally.  But then, why does it allow us to specify a size
			// in the constructor? Maybe it's a placebo? Stack is weird.
			inactive.Push(obj);
		}
	}


	/// <summary>
	/// Added to freshly instantiated objects, so we can link back
	/// to the correct pool on despawn.
	/// </summary>
	public class PoolMember : MonoBehaviour {
		public Pool MyPool;
	}
}