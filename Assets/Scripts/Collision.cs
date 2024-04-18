using System;
using UnityEngine;
using Vectors;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

[ExecuteAlways] // Run in Edit Mode 
[RequireComponent(typeof(VectorRenderer))]
public class Collision : MonoBehaviour
{
    // User Input Variables
    public Vector3 ballTravelDirection = new Vector3(1, -1, 0);
    public Vector3 ballPosition;
    public float ballRadius = 1;

    [Tooltip("Plane rotation in Euler degrees")]
    public Vector3 planeRotation;
    [Tooltip("Distance from plane normal to world origin")]
    public float planeDistance;

    // Internal Variables
    [NonSerialized]
    private VectorRenderer vectorRenderer;

    [SerializeField] private Transform planeTransform;
    [SerializeField] private Transform ballTransform;
    [SerializeField] private Transform ballImpactTransform;
    [SerializeField] private Transform ballFinalTransform;

    private Vector3 newBallPosition;
    private Vector3 impact;
    private Vector3 VectorA;
    private Vector3 VectorB;
    private Vector3 normalizedBallTravelDirection;

    private void Start()
    {
    }

    private void OnEnable()
    {
        vectorRenderer = GetComponent<VectorRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateUserInput();
        CalculateBounce();
        VisualizeModel();
    }

    // Update user input values and positions
    private void UpdateUserInput()
    {
        planeTransform.rotation = Quaternion.Euler(planeRotation);
        ballTransform.position = ballPosition;
        planeTransform.position = planeTransform.up * planeDistance;
        normalizedBallTravelDirection = CustomNormalize(ballTravelDirection);

        Vector3 ballScale = new Vector3(ballRadius * 2f, ballRadius * 2f, ballRadius * 2f);
        ballTransform.localScale = ballScale;
        ballImpactTransform.localScale = ballScale;
        ballFinalTransform.localScale = ballScale;
    }

    // Render vectors to visualize the model
    private void VisualizeModel()
    {
        ballImpactTransform.position = impact;
        ballFinalTransform.position = newBallPosition;

        using (vectorRenderer.Begin())
        {
            // Draw vectors using the vector renderer
            vectorRenderer.Draw(ballPosition, ballPosition + ballTravelDirection, Color.red);
            vectorRenderer.Draw(planeTransform.position, planeTransform.position + planeTransform.up, Color.green);
            vectorRenderer.Draw(ballPosition, impact, Color.blue);
            vectorRenderer.Draw(impact, impact - VectorA, Color.green);
            vectorRenderer.Draw(impact - VectorA, impact - VectorA - VectorA, Color.green);
            vectorRenderer.Draw(impact - VectorA - VectorA, impact - VectorA - VectorA + VectorB, Color.yellow);
            vectorRenderer.Draw(impact, newBallPosition, Color.blue);
            vectorRenderer.Draw(ballPosition, ballPosition + VectorA, Color.black);
            vectorRenderer.Draw(ballPosition, ballPosition + VectorA.normalized * ballRadius, Color.white);
        }
    }


    // Calculate the bounce behavior
    private void CalculateBounce()
    {
        // Calculate the distance between the ball position and the plane
        float aLength = CustomDot(ballPosition, planeTransform.up) - planeDistance - ballRadius;

        // Calculate the direction of the reflection vector 'VectorA'
        Vector3 DirectionA = -planeTransform.up;

        // Calculate the reflection vector 'VectorA' by multiplying the direction by the length
        VectorA = DirectionA * aLength;

        // Calculate the direction of the incident vector 'DirectionB' (same as the ball's travel direction)
        Vector3 DirectionB = normalizedBallTravelDirection;

        // Calculate the length of the reflection vector 'VectorB' using the dot product
        // and the length of the incident vector 'VectorA'
        float dotProductResult = CustomDot(VectorA, ballTravelDirection);
        float bLength = aLength * aLength / dotProductResult;

        // Calculate the reflection vector 'VectorB' by multiplying the direction by the length
        VectorB = DirectionB * bLength;

        // Calculate the impact point by adding the reflection vector 'VectorB' to the ball position
        impact = ballPosition + VectorB;

        // Calculate the new ball position after the bounce
        // by subtracting two times the reflection vector 'VectorA' and adding the reflection vector 'VectorB'
        newBallPosition = impact - VectorA - VectorA + VectorB;
    }

    float CustomDot(Vector3 a, Vector3 b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }

    // Anpassad normaliseringsfunktion
    Vector3 CustomNormalize(Vector3 vector)
    {
        float magnitude = Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        if (magnitude > 0)
        {
            return new Vector3(vector.x / magnitude, vector.y / magnitude, vector.z / magnitude);
        }
        else
        {
            return Vector3.zero; // Returnera nollvektor om inget att normalisera
        }
    }
}
