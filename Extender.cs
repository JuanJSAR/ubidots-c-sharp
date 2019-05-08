using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class Extender : MonoBehaviour
{
    static Extender _instance;

    public static Extender Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("Extender", typeof(Extender)).GetComponent<Extender>();
                _instance.gameObject.hideFlags = HideFlags.HideAndDontSave;
            }
            return _instance;
        }
    }
}

/// <summary>
/// Una co-rutina que puede ser suspendider, reanudár y abortadar.
/// </summary>
public class ExtCoroutine : IEnumerable
{
    bool done = false;
    bool abort = false;
    bool suspend = false;
    IEnumerator coroutine;
    float time = 0.0f;

    /// <summary>
    /// Crear una co-rutina de funciones interrupción, suspensión y reanudación.
    /// </summary>
    /// <param name="task">
    /// Un <see cref="IEnumerator"/> La tarea que se puede ejecutarse como una co-rutina.
    /// </param>
    public ExtCoroutine(IEnumerator task)
    {
        coroutine = task;

    }

    /// <summary>
    /// Comprueba si ya termino la co-rutina.
    /// </summary>
    public bool isDone
    {
        get
        {
            return done;
        }
    }

    /// <summary>
    /// Devuelve el tiempo que lleva la co-rutina.
    /// </summary>
    public float TimeLapse
    {
        get
        {
            return time;
        }
    }

    /// <summary>
    /// Devuelve la co-rutina.
    /// </summary>
    public IEnumerator Coroutine
    {
        get
        {
            return coroutine;
        }
    }

    /// <summary>
    /// Comienza la ejecución de la co-rutina.
    /// </summary>
    public void Start()
    {
        abort = false;
        done = false;
        Extender.Instance.StartCoroutine(GetEnumerator());
    }

    /// <summary>
    /// Aborta la ejecución de la co-rutina.
    /// </summary>
    public void Abort()
    {
        abort = true;
    }

    /// <summary>
    /// Suspende temporalmente la ejecución de la co-rutina.
    /// </summary>
    public void Suspend()
    {
        suspend = true;
    }

    /// <summary>
    /// Reanudar una co-rutina previamente suspendida.
    /// </summary>
    public void Resume()
    {
        suspend = false;
    }

    public IEnumerator GetEnumerator()
    {
        while (!abort)
        {
            if (suspend)
                yield return null;
            else
            {
                if (coroutine.MoveNext())
                    yield return coroutine.Current;
                else
                    break;
            }
            time += UnityEngine.Time.deltaTime;
        }
        done = true;
    }
}