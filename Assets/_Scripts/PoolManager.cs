using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
	#region Variables
	[SerializeField] private List<GameObject> prefabs;

	private Transform poolParent;
	private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();
	#endregion


	void Awake()
	{
		poolParent = transform;

		Setup();
	}


	private void Setup()
	{
		foreach (GameObject g in prefabs)
			AddTier(g, 10);
	}


	public void AddTier(GameObject _prefab, int _size)
	{
		if (poolDictionary.ContainsKey(_prefab.name))
			return;

		Queue<GameObject> queue = new Queue<GameObject>();

		for (int i = 0; i < _size; i++)
		{
			GameObject newObject = Instantiate(_prefab) as GameObject;
			newObject.SetActive(false);
			if (poolParent != null)
				newObject.transform.SetParent(poolParent);
			queue.Enqueue(newObject);
		}

		poolDictionary.Add(_prefab.name, queue);
	}

	public GameObject Instantiate(string _name, Vector3 _position, Vector3 _eulerAngle, Transform _parent)
	{
		if (!poolDictionary.ContainsKey(_name))
		{
			Debug.LogError("Pools dictionary does not contain a pool of this prefab and so can not use an instance of it.");
			return null;
		}

		GameObject instance = poolDictionary[_name].Dequeue();
		poolDictionary[_name].Enqueue(instance);
		instance.SetActive(false);
		instance.SetActive(true);
		instance.transform.position = _position;
		instance.transform.eulerAngles = _eulerAngle;
		
		if (_parent != null)
			instance.transform.SetParent(_parent);

		return instance;
	}
}
