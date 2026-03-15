using UnityEngine;

/// <summary>
/// Unity泛型单例基类
/// 提供线程安全的懒汉式单例实现，继承自MonoBehaviour
/// </summary>
/// <typeparam name="T">单例类型</typeparam>
public abstract class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
{
    private static T _instance;
    private static readonly object _lock = new object();
    private static bool _applicationIsQuitting = false;
    private static bool _isInitialized = false;
    
    /// <summary>
    /// 获取单例实例
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed. Returning null.");
                return null;
            }
            
            lock (_lock)
            {
                if (_instance == null)
                {
                    // 在场景中查找现有实例
                    _instance = FindFirstObjectByType<T>();
                    
                    if (_instance == null)
                    {
                        // 创建新的GameObject并添加单例组件
                        GameObject singletonObject = new GameObject($"[Singleton] {typeof(T).Name}");
                        _instance = singletonObject.AddComponent<T>();
                        DontDestroyOnLoad(singletonObject);
                        
                        // 手动初始化，因为Awake可能不会立即执行
                        if (!_isInitialized)
                        {
                            _instance.InitializeSingleton();
                            _isInitialized = true;
                        }
                    }
                    else if (!_isInitialized)
                    {
                        _instance.InitializeSingleton();
                        _isInitialized = true;
                    }
                }
                
                return _instance;
            }
        }
    }
    
    /// <summary>
    /// 检查单例是否存在
    /// </summary>
    public static bool HasInstance => _instance != null && !_applicationIsQuitting;
    
    /// <summary>
    /// 单例初始化方法（替代Awake使用）
    /// </summary>
    protected virtual void InitializeSingleton() { }
    
    protected virtual void Awake()
    {
        lock (_lock)
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
                
                if (!_isInitialized)
                {
                    InitializeSingleton();
                    _isInitialized = true;
                }
            }
            else if (_instance != this)
            {
                Debug.LogWarning($"[Singleton] Multiple instances of '{typeof(T)}' found. Destroying the duplicate.");
                Destroy(gameObject);
            }
        }
    }
    
    /// <summary>
    /// 手动销毁单例
    /// </summary>
    public static void DestroyInstance()
    {
        lock (_lock)
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);
                _instance = null;
                _isInitialized = false;
            }
        }
    }
    
    protected virtual void OnDestroy()
    {
        lock (_lock)
        {
            if (_instance == this)
            {
                _instance = null;
                _isInitialized = false;
            }
        }
    }
    
    protected virtual void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }
}