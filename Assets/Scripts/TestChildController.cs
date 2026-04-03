using UnityEngine;

public class TestChildController : Controller
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }
    /*
    public override void KeyboardManager()
    {
        if (Input.GetKey(kUpDirectionInput))
        {
            print("is moving forward");
        }
        if (Input.GetKey(kUpDirectionInput))
        {
            print("is moving back");
        }
        if (Input.GetKey(kRightDirectionInput))
        {
            print("is moving right");
        }
        if (Input.GetKey(kLeftDirectionInput))
        {
            print("is moving left");
        }
    }
    */
}
