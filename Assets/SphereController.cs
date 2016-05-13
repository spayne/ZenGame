using UnityEngine;
using System.Collections;
using System;

public class SphereController : MonoBehaviour {

    public LevelData level;
    public float max_sphere_speed;

    void Start()
    {
       
    }

	// Update is called once per frame
	void FixedUpdate () {
        // look at the current sphere_path
        // if I've reached the head of the queue, then pop off the next element and head that way
        if (level.current_sphere_path != null && level.current_sphere_path.Count > 0)
        {
            CellCoord head = level.current_sphere_path.Peek();
            Vector3 target = level.CellCoordToVector3(head);
            Vector3 delta =  target - transform.position;
            bool prevent_overshoot = true;

            if (delta.magnitude < 0.1f)
            {
                
            }
            else
            {
                // go on a reasonable speed towards the next waypoint

                // if it's the last waypoint converge
                Vector3 new_position;
                if (Math.Abs(delta.x) > 0.1)
                {
                    new_position = new Vector3(transform.position.x + Time.deltaTime * Math.Sign(delta.x) * max_sphere_speed,
                                                   target.y,
                                                   0.0f);
                    if (prevent_overshoot)
                    {
                        Vector3 new_delta = target - new_position;
                        if (Math.Sign(new_delta.x) != Math.Sign(delta.x))
                        {
                            new_position.x = target.x;
                            level.current_sphere_path.Dequeue();
                        }
                        
                    }
                }
                else
                {
                    new_position = new Vector3(target.x,
                                               transform.position.y + Time.deltaTime * Math.Sign(delta.y) * max_sphere_speed,
                                               0.0f);
                    if (prevent_overshoot)
                    {
                        Vector3 new_delta = target - new_position;
                        if (Math.Sign(new_delta.y) != Math.Sign(delta.y))
                        {
                            new_position.y = target.y;
                            level.current_sphere_path.Dequeue();
                        }
                        
                    }
                }

                transform.position = new_position;
            }           
        }
	}
}
