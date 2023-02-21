using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class random_location : MonoBehaviour
{
    float x;
    float y;
    float z;
    Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        x = Random.Range(-4, 4);
        y = 0.2F;
        z = Random.Range(-4, 4);
        pos = new Vector3(x, y, z);
        transform.position = pos;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            Destroy(other.gameObject);
        }
    }

    public float speed = 2;
    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(x, 0, z);
        movement = Vector3.ClampMagnitude(movement, 1);
        transform.Translate(movement * speed * Time.deltaTime);
    }
}
