using UnityEngine;

public abstract class Constraint : MonoBehaviour
{
    public delegate void IsDone(bool state, int index);

    public event IsDone Notify;

    protected int index;

    public Constraint(int _index)
    {
        index = _index;
    }
}
