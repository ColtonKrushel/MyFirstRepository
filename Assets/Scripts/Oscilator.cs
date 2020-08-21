using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscilator : MonoBehaviour
{
    [SerializeField] Vector3 movement;
    [SerializeField] float period = 2f;

    [Range(0,1)][SerializeField] float movementFactor;

    Vector3 startingPosition;

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (period <= Mathf.Epsilon)
        {
            return;
        }
        Oscilate();
    }

    private void Oscilate()
    {
        float cycles = Time.time / period;

        const float tau = Mathf.PI * 2;
        float rawSinWave = Mathf.Sin(cycles * tau);

        movementFactor = rawSinWave / 2f + 0.5f;

        Vector3 offset = movementFactor * movement;
        transform.position = startingPosition + offset;
    }
}
