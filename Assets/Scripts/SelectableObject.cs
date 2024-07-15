/*
This file is part of a Unity-based space simulation framework.
Copyright (c) 2024 Tejaswi Gorti
Licensed under the MIT License. See the LICENSE file in the project root for more information.
*/

using UnityEngine;

/// <summary>
/// Author: Tejaswi Gorti
/// Description: SelectableObject is a script that can be attached to any GameObject
///             in the scene. On the update loop it will look for mousedown events
///             and perform a raycast to determine which object in the universe is
///             selected by the player.
/// </summary>
/// 
public class SelectableObject : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // This will only fire the first frame after the button was pressed
        if (Input.GetMouseButtonDown(0))
        {
            // Fire a ray from the camera into the world
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            var select = GameObject.FindWithTag("Selectable").transform;
            // Test it out to 1000 units. If it hits something, continue.
            if (Physics.Raycast(ray, out hit))
            {
                // If it hit THIS object, do something.
                if (hit.collider.gameObject.Equals(this.gameObject))
                {
                    Debug.Log("You tapped " + gameObject.name);
                }
            }
        }
    }
}
