
// version 2.0 
// date 2017.08.02
// description: basic communication between Arduino and Unity. Simulation of the handwatch
// considering the rotating speed of the watch. It may lose some package occasionally, 
// since it read the angle only when the rotation currrently is done. 
// And the angle is a decimal with 2 digits. It may cause some error after several rotation.
// by Tyrone 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class playercontroller : MonoBehaviour
{
    public float speed;
    public Text countText;
    public Text winText;
    public Text p;
    public Text v;

    private Rigidbody rb;
    private int count;
    public static double x_cor, y_cor;
    public SerialController serialController;              // read and write
    private Vector2 pre_collision_pos;                      // last 
    private Vector2 pre_velocity;
    private int sendFlag;                              // send it or not
    private int firsty, firsty2;
    private GameObject LineRenderGameObject;             // draw the line
    private LineRenderer lineRenderer;

    private GameObject LineRenderGameObject2;             // draw the spot
    private LineRenderer lineRenderer2;

    public static double angle, pre_angle, delta_angle;
    public static Vector3 cur_speed;
    public static double dda;               // receive the delta_angle from arduino and share it with the watchhand(handRotate)
    private float radiuss;
    public GameObject explosion;
    private int ffl;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        winText.text = "";
        pre_collision_pos = new Vector2(0, 0);
        pre_velocity = new Vector2(3, 5);           //????
        serialController = GameObject.Find("SerialController").GetComponent<SerialController>();
        Debug.Log("Initialized...");
        firsty = 0;
        sendFlag = 0;
        x_cor = 0;
        y_cor = 0;
        pre_angle = 0;
        radiuss = 9.9f;

        ffl = 0;
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        //Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        Vector3 movement = new Vector3(0.0f, 0.0f, moveVertical);
        rb.AddForce(movement * speed);                            // add a force to the ball

        cur_speed = GetComponent<Rigidbody>().velocity;       // get the current speed and position
        Vector3 cur_pos = GetComponent<Rigidbody>().position;

        // calculate the colision position
        if (cur_speed[0] == 0 && cur_speed[2] != 0)
        {
            x_cor = cur_pos[0];
            y_cor = Math.Sqrt(radiuss * radiuss - x_cor * x_cor);
            if (cur_speed[2] < 0)
            {
                y_cor *= -1;
            }
        }
        else if (cur_speed[0] == 0 && cur_speed[2] == 0)
        {
           // sendFlag = 1;
        }
        else
        {
            float k = cur_speed[2] / cur_speed[0];
            float b1 = cur_pos[2] - k * cur_pos[0];
            double delta = Math.Sqrt(radiuss * radiuss - b1 * b1 + radiuss * radiuss * k * k);

            if (cur_speed[0] < 0)
            {
                x_cor = (-k * b1 - delta) / (1 + k * k);
            }

            else
            {
                x_cor = (-k * b1 + delta) / (1 + k * k);
            }
            y_cor = k * x_cor + b1;
        }   
        // end of calculate the colision position

        /*LineRenderGameObject = GameObject.Find("line");           // draw the line before sending
        lineRenderer = (LineRenderer)LineRenderGameObject.GetComponent("LineRenderer");
        lineRenderer.SetPosition(0, new Vector3(cur_pos[0], 0.0f, cur_pos[2]));
        lineRenderer.SetPosition(1, new Vector3((float)x_cor, 0.0f, (float)y_cor));    */
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Debug.Log("RESET");
        } 
        
        string message;
        cur_speed = GetComponent<Rigidbody>().velocity;       // get the current speed and position
        Vector3 cur_pos = GetComponent<Rigidbody>().position;

        if (firsty == 0)
        {
            firsty = 1;
            message = serialController.ReadSerialMessage();
            if (message == null)
            {
                Debug.Log("Got nothing...");
                return;
            }

            // Check if the message is plain data or a connect/disconnect event.
            if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_CONNECTED))
                Debug.Log("Connection established");
            else if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_DISCONNECTED))
                Debug.Log("Connection attempt failed or disconnection detected");
            else
                Debug.Log("Message arrived: " + message);
        }

        else
        {

            
            //receive the message from Arduino
            message = serialController.ReadSerialMessage();
            if (message == null)
            {
                //Debug.Log("Got nothing");
                return;
            }

            // Check if the message is plain data or a connect/disconnect event.
            if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_CONNECTED))
                Debug.Log("Connection established");
            else if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_DISCONNECTED))
                Debug.Log("Connection attempt failed or disconnection detected");
            else
            {
                Debug.Log("Got:"+ message+"|||");
                /*if (double.TryParse(message, out dda))
                {
                    ;
                }
                else
                {
                    //  Debug.Log("error in the string..." + message + "||");      // receive some dirty data
                }*/
            }
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            count += 1;
            SetCountText();
        }
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if (count >= 12)
            winText.text = "You Win";
    }



    void OnCollisionEnter(Collision ctl)
    {                                            //collision point?
        // add an explosion here      
        if (explosion != null && ctl.gameObject.name=="loop")
        {
            Debug.Log("boom");
           // Instantiate(explosion, transform.position,transform.rotation);
        }

        ContactPoint contact = ctl.contacts[0];
        Vector3 pos = contact.point;
        Vector3 a = GetComponent<Rigidbody>().velocity;
        p.text = pos.ToString();
        v.text = a.ToString();
        //Debug.Log(x_cor + "||" + y_cor + "...cur_position:" + GetComponent<Rigidbody>().position);  


        if (ffl == 0)
        {
            ffl = 1;
        }

        else 
        {
            // string xx = (delta_angle).ToString("0.00");              // send the data int..., without end_sign...
            serialController.SendSerialMessage("666");
            // if (delta_angle != 0)
            Debug.Log("Send 666");
        
        }

        
        pre_angle = angle;
        //SceneManager.LoadScene("Mini_Game");
       
       
       /* LineRenderGameObject2 = GameObject.Find("spot");              // draw the line after receive the data from arduino
        lineRenderer2 = (LineRenderer)LineRenderGameObject2.GetComponent("LineRenderer");
        lineRenderer2.SetPosition(0, new Vector3(cur_pos[0], 0.0f, cur_pos[2]));
        lineRenderer2.SetPosition(1, new Vector3((float)x_cor, 0.0f, (float)y_cor));        */    


    }
}