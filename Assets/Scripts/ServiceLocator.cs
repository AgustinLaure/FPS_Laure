using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : Singleton<ServiceLocator>
{
    private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

    public void AddService<T>(T service) where T : class
    {
        var type = typeof(T);

        if (!_services.TryAdd(type, service))
        {
            Debug.LogWarning("Ya esta suscripto: " + type);
        }
    }

    public void RemoveService<T>(T service) where T : class
    {
        var type = typeof(T);

        if (_services.ContainsKey(type))
        {
            _services.Remove(type);
        }
        else
        {
            Debug.LogWarning("No existe el servicio: " + type);
        }
    }

    public T GetService<T>() where T : class
    {
        var type = typeof(T);

        if (_services.TryGetValue(type, out var service))
        {
            return (T)service;
        }

        Debug.LogWarning("No esta suscripto: " + type);
        return null;
    }

    public void ClearServices() => _services.Clear();
}

