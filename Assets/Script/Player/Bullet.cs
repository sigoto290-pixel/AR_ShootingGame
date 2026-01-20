using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] MeshRenderer _meshRenderer;
    [SerializeField] BoxCollider _boxCollider;
    [SerializeField] TrailRenderer _trailRenderer;

    [SerializeField] float _speed = 5000;
    [SerializeField] float _lifeTime = 1;
    [SerializeField] float _maxWidth = 0.5f;
    [SerializeField] float _minWidth = 0.3f;
    [SerializeField] float _maxAlpha = 0.5f;
    [SerializeField] float _minAlpha = 0.3f;

    Rigidbody _rb;
    ParticleSystem _particle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _particle = GetComponent<ParticleSystem>();
        _trailRenderer.widthMultiplier = Random.Range(_minWidth,_maxWidth);
        _trailRenderer.colorGradient.alphaKeys[0].alpha = Random.Range(_minAlpha,_maxAlpha);
    }
    void FixedUpdate()
    {
        _rb.linearVelocity = transform.forward * _speed;
    }
    void Update()
    {
        _lifeTime -= Time.deltaTime;
        if(_lifeTime <= 0)
        {
            Destroy(this.gameObject); 
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        Explosion();
    }
    void Explosion()
    {
        StartCoroutine(OneShot());
        IEnumerator OneShot()
        {
            _particle.Play();
            _meshRenderer.enabled = false;
            _rb.isKinematic = true;
            yield return new WaitWhile(() => _particle.isPlaying == true);
            yield return new WaitWhile(() => _lifeTime <= 0);
            Destroy(this.gameObject);
        }

    }
}
