using UnityEngine;

public class DirectionalLightTowardsTarget : MonoBehaviour
{
    [SerializeField]
    private Transform _toRotate;

    [SerializeField]
    private Transform _target;

    private float distanceToTarget;

    protected virtual void Start()
    {
            
    }

    public void UpdateLighting()
    {
        Vector3 dirToTarget = (_target.position - _toRotate.position).normalized;

        distanceToTarget = Vector3.Distance(transform.position, _target.position);
        //transform.GetComponent<Light>().intensity = distanceToTarget / 100;
        _toRotate.LookAt(_toRotate.position - dirToTarget, Vector3.up);
    }
}
