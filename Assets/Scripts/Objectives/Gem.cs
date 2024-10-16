using UnityEngine;

public class Gem : MonoBehaviour
{
    [SerializeField] private string GleamAnimationName = "Gleam";
    [SerializeField] private float minTimeBetweenGleams = 1.0f;
    [SerializeField] private float maxTimeBetweenGleams = 3.0f;

    private Animator _animator;

    private float _timeBetweenGleams;
    private float _timer;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) return;
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            _animator.Play(GleamAnimationName, -1, 0f);
            _timer = Random.Range(minTimeBetweenGleams, maxTimeBetweenGleams);
        }
    }

    // TODO: Fly to player
}
