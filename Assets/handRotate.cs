/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class handRotate : MonoBehaviour 
{

    private double delta_angle;
    private double pre_delta;
    private int firsty;

    void Start()
    {
        firsty = 0;
    }

	
	void Update () // Update is called once per frame
    {   
        delta_angle = playercontroller.dda;
        
        if(firsty==0)
        {
            firsty++;
            return;
        }

        //  if ((cur_speed[0] != 0 || cur_speed[2] != 0) && (pre_delta != delta_angle))
        if ( pre_delta != delta_angle )
        {
            transform.RotateAround(new Vector3(30, 0.5f, 0), new Vector3(0, -1,0), (float)delta_angle);
        //    Debug.Log("Get the angle:" + delta_angle);
        }
        pre_delta = delta_angle;
	}
}
*/



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handRotate : MonoBehaviour
{
    private float delta_angle;
    private float pre_delta;
    private int firsty;
    private float angle_per_rot;           // fixedupdate(), 50 frames/s, set it the speed of the hand. 
    private float counter;
    private int getrdy;
    private double []getangles;
    private int cnt;
    private int savepoint;

    void Start()                  // Use this for initialization
    {
        // pre_delta = 0;
        firsty = 0;
        angle_per_rot = 50000000.0f;             // Standard 50 degrees in a second for 1.0f
        counter = 0;
        getrdy = 0;
        getangles = new double[100];          // set 0 automatically
        cnt = 0;
     
    }


    void FixedUpdate()                  // Update is called once per frame
    {
        /*
        double adelta_angle = (float)playercontroller.dda;
        if (Mathf.Abs((float)adelta_angle) < 1 && firsty!=0)
        {
            transform.RotateAround(new Vector3(30, 0.5f, 0), new Vector3(0, -1, 0), (float)adelta_angle);
            return;
        }
        
        float dagl = (float)playercontroller.dda;
        if(Mathf.Abs(dagl)<=10)
        {
            transform.RotateAround(new Vector3(30, 0.5f, 0), new Vector3(0, -1, 0), dagl);
            return;
        }

        else*/
        {
            if (getrdy == 0)
            {
                delta_angle = (float)playercontroller.dda;
                //if(delta_angle!=0 && firsty!=0)
                // getangles[cnt++] = delta_angle;
            }

            if (firsty == 0)
            {
                firsty++;
                return;
            }

            else if (pre_delta != delta_angle && (int)delta_angle != 0)
            {
                getrdy = 1;
                savepoint = cnt;
                if (delta_angle != 0 && firsty != 0)
                {
                    //Debug.Log("|||"+ counter+"||"+delta_angle+"||"+angle_per_rot);              
                    if (delta_angle >= 0)        // left
                    {
                        if (counter <= delta_angle - angle_per_rot)
                        {
                            transform.RotateAround(new Vector3(30, 0.5f, 0), new Vector3(0, -1, 0), angle_per_rot);
                            counter += angle_per_rot;
                        }

                        else if (delta_angle - counter >= 0 && delta_angle - counter < angle_per_rot)
                        {
                            transform.RotateAround(new Vector3(30, 0.5f, 0), new Vector3(0, -1, 0), delta_angle - counter);
                            // Debug.Log("this is now&sum:" + counter*(-1) + "||" + (180-delta_angle));
                            counter = 0;
                            firsty++;
                            pre_delta = delta_angle;
                            getrdy = 0;
                        }

                    }


                    else if (delta_angle < 0)    //right
                    {
                        if (counter <= delta_angle * (-1) - angle_per_rot)
                        {
                            transform.RotateAround(new Vector3(30, 0.5f, 0), new Vector3(0, -1, 0), (-1) * angle_per_rot);
                            counter += angle_per_rot;
                        }

                        else if (delta_angle * (-1) - counter >= 0 && delta_angle * (-1) - counter < angle_per_rot)
                        {
                            transform.RotateAround(new Vector3(30, 0.5f, 0), new Vector3(0, -1, 0), (delta_angle + counter));
                            //  Debug.Log("this is now&sum:" + counter + "||" + (delta_angle + 180));
                            counter = 0;
                            firsty++;
                            pre_delta = delta_angle;
                            getrdy = 0;
                        }

                    }
                }
            }

        }



        


       
    }
}