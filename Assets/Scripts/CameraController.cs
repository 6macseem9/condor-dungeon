using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _movementLerp;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _rotationLerp;
    [SerializeField] private float _zoomSpeed;
    [SerializeField] private float _zoomLerp;
    [Space(5)]
    [SerializeField] private Transform _background;

    private Vector3 _newPos;
    private Quaternion _newRotation;
    private Vector3 _newZoom;
    private Vector3 _dragStart;
    private Vector3 _dragCurrent;
    private Vector3 _rotateStart;
    private Vector3 _rotateCurrent;

    private Transform _cameraTransform;
    private PixelPerfectCamera _pixelPerfect;

    private void Start()
    {
        _cameraTransform = Camera.main.transform;
        _pixelPerfect = _cameraTransform.GetComponent<PixelPerfectCamera>();

        _newPos = transform.position;
        _newRotation = transform.rotation;
        _newZoom = _cameraTransform.localPosition;

        Camera.main.eventMask = LayerMask.GetMask("RoomObject");
    }

    private void LateUpdate()
    {
        HandleMovementInput();
        HandleRotationInput();
        HandleZoomInput();

        HandleMousePan();
        HandleMouseRotation();

        transform.position = Vector3.Lerp(transform.position, _newPos, _movementLerp * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, _newRotation, _rotationLerp * Time.deltaTime);
    }

    private void HandleMovementInput()
    {
        var horInput = Input.GetAxis("Horizontal");
        var verInput = Input.GetAxis("Vertical");

        _newPos += transform.forward * _movementSpeed *Time.deltaTime * verInput;
        _newPos += transform.right * _movementSpeed * Time.deltaTime * horInput;
        _newPos = _newPos.Clamp(-24, 24);
    }
    private void HandleRotationInput()
    {
        if(Input.GetKey(KeyCode.E))
        {
            _newRotation *= Quaternion.Euler(Vector3.up * _rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            _newRotation *= Quaternion.Euler(Vector3.up * -_rotationSpeed * Time.deltaTime);
        }    
    }
    private void HandleZoomInput()
    {
        if (Input.mouseScrollDelta.y!=0)
        {
            _newZoom += -Input.mouseScrollDelta.y * new Vector3(0,_zoomSpeed,-_zoomSpeed) * Time.deltaTime;
        }

        _newZoom = new Vector3(0, Mathf.Clamp(_newZoom.y, 8, 300), Mathf.Clamp(_newZoom.z, -300, -8));
        _cameraTransform.localPosition = Vector3.Lerp(_cameraTransform.localPosition, _newZoom, _zoomLerp * Time.deltaTime);
    }
    private void HandleMousePan()
    {
        if (Input.GetKey(KeyCode.LeftShift)) return;

        if (Input.GetMouseButtonDown(2))
        {
            Plane plane = new Plane(Vector3.up,Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;
            if(plane.Raycast(ray,out entry))
            {
                _dragStart = ray.GetPoint(entry);
            }
        }
        if (Input.GetMouseButton(2))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;
            if (plane.Raycast(ray, out entry))
            {
                _dragCurrent = ray.GetPoint(entry);

                _newPos = transform.position + _dragStart - _dragCurrent;
            }
        }
    }
    private void HandleMouseRotation()
    {
        if (!Input.GetKey(KeyCode.LeftShift)) return;

        if (Input.GetMouseButtonDown(2))
        {
            _rotateStart = Input.mousePosition;
        }
        if (Input.GetMouseButton(2))
        {
            _rotateCurrent = Input.mousePosition;

            Vector3 difference = _rotateStart - _rotateCurrent;

            _rotateStart = _rotateCurrent;

            var x = Mathf.Clamp(difference.x, -20, 20);
            _newRotation *= Quaternion.Euler(Vector3.up * (-x / 5f));
        }
    }

    public void ResetCamera()
    {
        _newPos = new Vector3(3.3f, 0, 0.3f);
        _newRotation = Quaternion.identity;
        _newZoom = new Vector3(0, 58, - 58);

        _background.localPosition = new Vector3(-28, 0, 600);
    }

    public void SetPixelization(float value)
    {
        _pixelPerfect.enabled = value==0? false : true;
        if (value == 0) return;
        _pixelPerfect.refResolutionX = 640 / (int)value;
        _pixelPerfect.refResolutionY = 360 / (int)value;
    }
}
