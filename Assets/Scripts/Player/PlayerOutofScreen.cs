using UnityEngine;

public class PlayerOutofScreen
{
    private Camera m_camera;
    private Transform m_playerTransform;

    private Rect m_cameraRect;

    private Vector2 m_pureScreenSize = Vector2.zero;
    private Vector2 m_WorldScreenSize = Vector2.zero;

    private float m_MinX = 0f;
    private float m_MinY = 0f;
    private float m_DetectSpacing = 5f;

    public PlayerOutofScreen(Camera targetCam, in Transform playerTransform)
    {
        m_camera = targetCam;
        m_playerTransform = playerTransform;

        CalculateScreenSize();

        m_MinX = GetMinX();
        m_MinY = GetMinY();
    }

    public void CalculateScreenSize()
    {
        m_cameraRect = m_camera.pixelRect;
        m_pureScreenSize = new Vector2(m_cameraRect.width, m_cameraRect.height);
        m_WorldScreenSize = m_camera.ScreenToWorldPoint(m_pureScreenSize);
    }

    public float GetMinX()
    {
        CalculateScreenSize();
        return m_camera.transform.position.x - Mathf.Abs((m_camera.transform.position.x - m_WorldScreenSize.x) * 0.5f);
    }
    public float GetMinY() => m_camera.transform.position.y - (m_WorldScreenSize.y * 0.5f);

    public Vector2 GetCameraMinPosition() => new Vector2(GetMinX(), GetMinY());

    public bool IsScreenOut()
    {
        if (m_playerTransform.position.y < m_MinY - m_DetectSpacing)
        {
            return true;
        }
        return false;
    } 
}
