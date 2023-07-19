using UnityEngine;

public class Run : CState
{
    private readonly InfiniteGuy _guy;

    private float _initialSpeed;
    private const float WANDER_SPEED = 2F;
    private const float WANDER_DISTANCE = 10F;

    public Run(InfiniteGuy guy)
    {
        _guy = guy;
    }

    public void OnEnter()
    {
        Debug.Log("entered wander state");
    }

    public void Tick()
    {

    }

    private Vector3 GetRandomPoint()
    {
        Vector3 randomVector = Random.insideUnitSphere.normalized;
        var direction = _guy.transform.position.normalized - randomVector;
        //direction.Normalize();

        var endPoint = _guy.transform.position + (direction * WANDER_DISTANCE);
        Debug.Log("endpoint: " + endPoint);

        return _guy.transform.position;
    }

    public void OnExit()
    {
        Debug.Log("exited wander state");
    }
}
