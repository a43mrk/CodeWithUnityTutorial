using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public MeshRenderer Renderer;
    [SerializeField] Vector3 location = new Vector3(3, 4, 1);
    [SerializeField] float _scale = 1.3f;
    [SerializeField] float _angle = 10.0f;
    [SerializeField] float _rotationSpeed = 1f;
    [SerializeField] float _alpha = 1.0f;
    [SerializeField] Color _color;

    void Start()
    {
        // change randomly each time the scene is played.
        _color = new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            _alpha);

        transform.position = location;
        transform.localScale = Vector3.one * _scale;
        
        Material material = Renderer.material;
        _color.a = _alpha;
        material.color = _color;
    }
    
    void Update()
    {
        transform.localScale = Vector3.one * _scale;

        Material material = Renderer.material;
        _color.a = _alpha;
        material.color = _color;

        transform.Rotate(
           ( rotateX ?1 :0) *_rotationSpeed *_angle * Time.deltaTime,
           ( rotateY ?1 :0) *_rotationSpeed *_angle * Time.deltaTime,
           ( rotateZ ?1 :0) *_rotationSpeed *_angle * Time.deltaTime
        );
    }


    public float Red { get => _color.r; set => _color.r = value; }
    public float Blue { get => _color.b; set => _color.b = value; }
    public float Green { get => _color.g; set => _color.g = value; }
    public float Alpha { get => _alpha; set => _alpha = value; }
    public float Speed { get => _rotationSpeed; set => _rotationSpeed = value; }
    public float Scale { get => _scale; set => _scale = value; }
    public float Angle { get => _angle; set => _angle = value; }
    public bool rotateX = false;
    public bool rotateY = false;
    public bool rotateZ = false;

    public void ToggleRotateX()
    {
        rotateX = !rotateX;
    }

    public void ToggleRotateY()
    {
        rotateY = !rotateY;
    }

    public void ToggleRotateZ()
    {
        rotateZ = !rotateZ;
    }
}
