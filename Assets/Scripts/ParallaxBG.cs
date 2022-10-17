using UnityEngine;
using UnityEngine.UI;
public class ParallaxBG : MonoBehaviour
{
    Vector2 _startPos;
    [SerializeField] int _moveSpeed;
    public Transform canvas;
    private void Awake()
    {
        if (canvas == null)
        {
            canvas = GetComponentInParent<Transform>();
        }
    }
    void Start()
    {
        _startPos = transform.localPosition;
        GameManager.instance.ResolutionChanged += gm_ResolutionChangeMenuBG;
    }

    void Update()
    {
        Vector2 pz = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        float posX = Mathf.Lerp(transform.localPosition.x, _startPos.x + (pz.x * _moveSpeed), 1.5f * Time.deltaTime);
        float posY = Mathf.Lerp(transform.localPosition.y, _startPos.y + (pz.y * _moveSpeed), 1.5f * Time.deltaTime);

        transform.localPosition = new Vector3(posX, posY, 0);
    }
    public void gm_ResolutionChangeMenuBG(object sender, ResolutionEventArgs e)
    {
        transform.localPosition = _startPos;
        // Debug.Log($"canvas position: {canvas.position} \nbackground position: {_startPos}");
    }
}
