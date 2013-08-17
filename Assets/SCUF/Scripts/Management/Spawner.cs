using UnityEngine;
using System.Collections;

/// <summary>
/// Class taken from the example game "Angry Bots"
/// How to use:
/// - Create an object and add this script to it
/// - This object will have an array called "Caches" to hold the cache of objects we want
/// - Create a new cache object, adding a prefab and setting the number of entities spawned
/// - In the code, instead of instantiating a new object, we use 'spawner.Spawn()'. 
/// - Still in the code, insted of calling Destroy(). we use spawner.Destroy(). This will not destroy the game 
/// object, instead it will set it free in the spawner list.
/// </summary>
public class Spawner : MonoBehaviour {
	
	public static Spawner	spawner;	//< Shortcut for this class and instance
	public ObjectCache[]	caches;		//< Shown in the inspector: an array of object we wanted cached
	Hashtable							activeCachedObjects;	//< internal Hashtable

	// Class to keep all the objects in the cache
	[System.Serializable]
  public class ObjectCache {

		public GameObject			prefab;					//< The object prefab
		public int						cacheSize = 10;	//< How many objects will be instantiated when the code starts

		private GameObject[]	objects;				//< Array for all the instantiated objects
		private int						cacheIndex = 0;	//< Index on the cache

		/// <summary>
		/// Create and populate the objects array, instantiating them in the game
		/// </summary>
		public void Initialize() {

			objects = new GameObject[cacheSize];

			// Instantiate the objects in the array and set them to be inactive
			for(int i = 0; i < cacheSize; i++) {

				objects[i] = MonoBehaviour.Instantiate(prefab) as GameObject;
				objects[i].SetActive(false);
				objects[i].name = objects[i].name + i;
			}
		}

		/// <summary>
		/// Search the cache and return the next free object
		/// </summary>
		/// <returns> A game object that is tagged as free in the cache, or the older one when none is free </returns>
		public GameObject GetNextObjectInCache() {

			GameObject obj = null;

			// The cacheIndex start out at the position of the object created the longest time ago, so that one
			// is usually free, but in case not, loop through the cache until we a find a free one
			for(int i = 0; i < cacheSize; i++) {

				obj = objects[cacheIndex];

				// If we found an inactive object in the cache, use that
				if(!obj.activeSelf)
					break;

				// If not, increment index and make it loop around if it exceeds the size of cache
				cacheIndex = (cacheIndex +1) % cacheSize;
			}

			// The object should be inactive. If it's not, log a warning and use the object created the longest ago
			// even though it's still active
			if(obj.activeSelf) {

				// DEBUG
				Debug.LogWarning(
						"Spawn of " + prefab.name + 
						" exceeds cache size of " + cacheSize + 
						"! Reusing already active object.", obj );

				Spawner.Destroy(obj);
			}

			// Increment index and make it loop around if it exceeds the size of the cache
			cacheIndex = (cacheIndex + 1) % cacheSize;

			return obj;
		}
	}
	
	/// <summary>
	///
	/// </summary>
	void Awake() {

		// Set the global variable
		spawner = this;

		// Total number of cached objects
		int amount = 0;

		// loop through the caches
		for(int i = 0; i < caches.Length; i++) {

			// Initialize each cache
			caches[i].Initialize();

			// Count
			amount += caches[i].cacheSize;
		}

		// Create a hashtable with the capacity set to the amount of cached objects specified
		activeCachedObjects = new Hashtable(amount);
	}


	/// <summary>
	///	Used in the place of Instantiate in other classes. The Spawner will find the cache for the object that we
	///	want to create and return the next one free. If theres no cache, it will instantiate the object
	/// </summary>
	/// <param name="prefab"> Object to be instantiate or found in the cache</param>
	/// <param name="position"> Position to instantiate the object </param>
	/// <param name="rotation"> Rotation of the instantiated object </param>
	/// <returns> The object instantiated or found in the cache </returns>
	public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation) {

		ObjectCache cache = null;
		
		// Find the cache for the specified prefab
		if(spawner) {

			for(int i = 0; i < spawner.caches.Length; i++) {

				if(spawner.caches[i].prefab == prefab) {

					cache = spawner.caches[i];
				}
			}
		}

		// If there's no cache for this prefab type, just instantiate normally
		if(cache == null) {

			// DEBUG
			Debug.LogWarning(Time.time + " Instantiating " + prefab);

			return Instantiate(prefab, position, rotation) as GameObject;
		}

		// Find the next object in the cache
		GameObject obj = cache.GetNextObjectInCache();
		
		// Set the position and rotation of the object
		obj.transform.position = position;
		obj.transform.rotation = rotation;

		// Set the object to be active
		obj.SetActive(true);
		spawner.activeCachedObjects[obj] = true;

		return obj;
	}

	/// <summary>
	/// Set the object as inactive and free to be used again if found in the cache, otherwise destroy it from the
	/// hierarchy. Must be used instead of object.Destroy()
	/// </summary>
	/// <param name="objectToDestroy"> Object to be freed or destroyed </param>
	public static void Destroy(GameObject objectToDestroy) {
	
		if(objectToDestroy == null) {

			// DEBUG
			Debug.LogError("Spawner.Destroy: Null object as parameter");
		}


		if(spawner && spawner.activeCachedObjects.ContainsKey(objectToDestroy)) {

			objectToDestroy.SetActive(false);
			spawner.activeCachedObjects[objectToDestroy] = false;
		}
		else {

			Destroy(objectToDestroy);
		}
	}
}
