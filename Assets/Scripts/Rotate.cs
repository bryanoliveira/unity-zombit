using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {
    public Vector3 rotateVector;
    public float smooth = 5;

    void Start() {
        rotateVector.Normalize();
    }

    void Update() {
        transform.Rotate(rotateVector * Player.time * smooth);
    }
}
