using UnityEngine;

public class CameraFollow : MonoBehaviour 
{
    public Transform playerView;
    public Transform mapView;
    private bool view = false;
    

    public Vector3 mapOffset;
    public Vector3 playerOffset;

    private void Update() 
    {
        //if (Input.GetMouseButtonDown(1))
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (view) 
            {
                view = false;
            }
            else 
            {
                view = true;
            }
        }

        if (view)
        {
            transform.position = playerView.position + playerOffset;
            transform.LookAt(playerView);
        }
        else
        {
            transform.position = mapView.position + mapOffset;
            transform.LookAt(mapView);
        }
        
    }

    public void setTutorialView()
    {
        view = false;
    }

    public bool getView() { return view; }
}