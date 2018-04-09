
using UnityEngine;


namespace Float
{

	public class Singleton<T> where T : new()
	{
		private static T _instance;
		static object _lock = new object();

		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_lock)
					{
						if (_instance == null)
						{
							_instance = new T();
						}
					}
				}
				return _instance;

			}
		}
	}


	public class SingletonMono<T> : MonoBehaviour
		where T : Component
	{
		private static T _instance;

		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType(typeof(T)) as T;
					if (_instance == null)
					{
						var go = new GameObject();
						go.name = typeof(T).Name;
						//go.hideFlags = HideFlags.HideAndDontSave;
						_instance = go.AddComponent<T>();
					}
				}
				return _instance;
			}
		}

		void Awake()
		{
			DontDestroyOnLoad(gameObject);
			if (_instance == null)
			{
				_instance = this as T;
			}
			else
			{
				Destroy(gameObject);
			}
		}

	}

}