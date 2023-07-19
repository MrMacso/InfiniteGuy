using System;
using UnityEngine;

public class InfiniteGuy : MonoBehaviour
{
    public event Action<float> OnHungerChanged;
    public event Action<float> OnThirstChanged;
    public event Action<float> OnCircleChanged;

    private StateMachine _stateMachine;

    [SerializeField, Range(0, 100)] public float _hunger;
    [SerializeField, Range(0, 100)] public float _hydration;

    public bool _isHydrating = false;
    public float _time;

    private CharacterController _controller;
    public Transform _cam;

    public float _speed = 5f;

    public float _turnSmoothTime = 0.1f;
    float _turnSmoothVelocity;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _hunger = 30f;
        _hydration = 30f;
        //assign objects and components to variables
        var animator = GetComponent<Animator>();
        _stateMachine = new StateMachine();
        //states
        var idle = new Idle(this);


        //transitions
        //At(wander, searchForFood, Hungry());


        //_stateMachine.AddAnyTransition(flee, () => objectDetector.EnemyInRange);
        //At(flee, searchForFood, () => objectDetector.EnemyInRange == false);

        _stateMachine.SetState(idle);
        //At: add transition
        void At(CState to, CState from, Func<bool> condition)
        {
            _stateMachine.AddTransition(to, from, condition);
        }
        //conditions
        //Func<bool> GotEaten() => () => (Target == null || Target.IsDepleted) && !BellyFull().Invoke();
        Func<bool> Hungry() => () => _hunger <= 40 && _hydration! < 20;
        Func<bool> Thirsty() => () => _hydration < 20;
        //Func<bool> NightTime() => () => _time < 8 && !_stateMachine.Equals(searchForLake);
        Func<bool> DayTime() => () => 8 < _time;
    }


    // Update is called once per frame
    void Update()
    {
        _stateMachine.Tick();

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targerAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targerAngle, ref _turnSmoothVelocity, _turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targerAngle, 0f) * Vector3.forward;
            _controller.Move(moveDir.normalized * _speed * Time.deltaTime);
        }
    }
    public bool Take()
    {
        if (!_isHydrating) _isHydrating = true;
        if (_hunger <= 0)
            return false;

        _hunger--;
        OnHungerChanged?.Invoke(_hunger);
        return true;
    }
    public void Hydrate(float amount)
    {
        if (!_isHydrating) _isHydrating = true;

        _hydration += amount * Time.deltaTime;
        OnThirstChanged?.Invoke(_hydration);
        if (_hydration >= 99) _isHydrating = false;
    }
    public void Digestion(float water, float food)
    {
        _hydration -= water * Time.deltaTime;
        _hunger -= food * Time.deltaTime;
        OnThirstChanged?.Invoke(_hydration);
        OnHungerChanged?.Invoke(_hunger);
    }
    public void SetIsHydrating(bool state)
    {
        _isHydrating = state;
    }
    public void SetHydrationVaule(float state)
    {
        _hydration = state;
    }
    public void SetHungerValue(float state)
    {
        _hunger = state;
    }
}
