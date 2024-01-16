using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    // Start is called before the first frame update
    float minY = 10f;
    float maxY = 20f;
    public float speed = 50f;
    public GameObject Caspule;
    public int MSSV = 20127425;
    public float rotationSpeed = 50.0f; // Tốc độ xoay
    private bool isRotatingObjectA = false;
    private bool isRotatingObjectB = false;

    private bool isEnterT = false;
    private int points = 0; // The points value
    List<GameObject> spawnedObjects = new List<GameObject>();
    public float heightChangeAmount = 1; 
    private bool isMoving = false;
    private bool moveUp = true;
    private float moveSpeed;
    float randomY;
    void Start()
    {
        randomY = Random.Range(minY, maxY);
        Vector3 pos = transform.position; 
        pos = new Vector3(0f, randomY, 0f);
        transform.position = pos;

        points = 0; 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) ||  Input.GetKeyDown(KeyCode.A)) 
        {
            transform.position += new Vector3(-1, 0, 0) * speed * Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) 
        {
            transform.position += new Vector3(1, 0, 0) * speed * Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) 
        {
            transform.position += new Vector3(0, 1, 0) * speed * Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) 
        {
            transform.position += new Vector3(0, -1, 0) * speed * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            generateCapsule();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(DeleteObjectCoroutine());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            isRotatingObjectA = !isRotatingObjectA;
        }

        if (isRotatingObjectA)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {

            isRotatingObjectB = !isRotatingObjectB;
            isEnterT = true;
        }

        if(isEnterT) 
        {
            if (isRotatingObjectB)
            {
                ChangeHeightAndGravity(); 
                RotateObjects();
            } 
            else
            {
                RestoreGravity(); 
            }
            isEnterT = false;
            
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            isMoving = !isMoving; 
            {
                int MSSV = 1234567; 
                moveSpeed = 2 + (MSSV % 10);
            }
        }

        if (isMoving)
        {
            MoveObjectVertically();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ChangeColors();
        }

        if (Input.GetMouseButtonDown(0)) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject;

                if (spawnedObjects.Contains(clickedObject))
                {
                    Destroy(clickedObject); 
                    IncreasePoints(); 
                }
            }
        }
    }

    void IncreasePoints()
    {
        points++; 
    }

    IEnumerator DeleteObjectCoroutine()
    {

        yield return new WaitForSeconds(2f);
        while (spawnedObjects.Count > 0)
        {
            int randomIndex = Random.Range(0, spawnedObjects.Count);
            Destroy(spawnedObjects[randomIndex]);
            spawnedObjects.Remove(spawnedObjects[randomIndex]);
        }

    }

    void generateCapsule() 
    {
        float randomY = Random.Range(10, 10 + MSSV % 10);
        float randomX = Random.Range(-5, 5);
        float randomZ = Random.Range(-5, 5);

        Vector3 spawnPosition = new Vector3(randomX, randomY, randomZ);

        GameObject objectB = Instantiate(Caspule, spawnPosition, Quaternion.identity);
        spawnedObjects.Add(objectB); 
        Rigidbody rb = objectB.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(Vector3.forward * 150, ForceMode.Force);
            rb.useGravity = true; 
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }


    void ChangeHeightAndGravity()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            rb.useGravity = false; 
            rb.velocity = Vector3.zero;

            Vector3 newPosition = obj.transform.position;
            if (newPosition.y == 0)
            {
                newPosition.y += 10;
            }
            else
            {
                newPosition.y = 10;
            }
            obj.transform.position = newPosition;
            rb.angularVelocity = newPosition;
        }
    }

    void RotateObjects()
    {

        foreach (GameObject obj in spawnedObjects)
        {
            obj.transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
        }
    }

    // Hàm khôi phục trọng lực cho các đối tượng B
    void RestoreGravity()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(Vector3.down, ForceMode.Force);
                rb.useGravity = true; 
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    void MoveObjectVertically()
    {
        float newYPos = transform.position.y;

        if (moveUp)
        {
            newYPos += moveSpeed * Time.deltaTime;
            if (newYPos >= randomY) // If reaching the upper limit
            {
                newYPos = randomY;
                moveUp = false;
            }
        }
        else
        {
            newYPos -= moveSpeed * Time.deltaTime;
            if (newYPos <= 1) // If reaching the lower limit
            {
                newYPos = 1;
                moveUp = true;
            }
        }

        transform.position = new Vector3(transform.position.x, newYPos, transform.position.z);
    }

    void OnMouseEnter()
    {
        GetComponent<Renderer>().material.color = Color.blue; // Thay đổi màu khi di chuột vào
    }

    void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = Color.white; // Đổi về màu gốc khi di chuột ra
    }

    void ChangeColors()
    {
        // Đổi màu cho tất cả các đối tượng B
        foreach (GameObject obj in spawnedObjects)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = GetRandomColor();
            }
        }
    }

    Color GetRandomColor()
    {
        // Tạo một màu ngẫu nhiên
        return new Color(Random.value, Random.value, Random.value);
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 50;
        style.normal.textColor = Color.white;
        GUI.Label(new Rect(10, 10, 100, 20), "Points: " + points.ToString(), style);
    }
}