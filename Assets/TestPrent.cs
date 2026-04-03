using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPrent : MonoBehaviour
{
    [SerializeField] protected InputActionAsset actionMap;
    [SerializeField] protected int protectedInt;
    [SerializeField] private int privateInt;
    private Renderer render;
    private Material material;
    public bool test = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}