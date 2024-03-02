using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _movementLerp;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _rotationLerp;
    [SerializeField] private float _zoomSpeed;
    [SerializeField] private float _zoomLerp;

    private Vector3 _newPos;
    private Quaternion _newRotation;
    private Vector3 _newZoom;
    private Vector3 _dragStart;
    private Vector3 _dragCurrent;
    private Vector3 _rotateStart;
    private Vector3 _rotateCurrent;

    private Transform _cameraTransform;
     
    void Start()
    {
        _cameraTransform = Camera.main.transform;

        _newPos = transform.position;
        _newRotation = transform.rotation;
        _newZoom = _cameraTransform.localPosition;
    }

    void LateUpdate()
    {
        HandleMovementInput();
        HandleRotationInput();
        HandleZoomInput();

        HandleMousePan();
        HandleMouseRotation();

        transform.position = Vector3.Lerp(transform.position, _newPos, _movementLerp * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, _newRotation, _rotationLerp * Time.deltaTime);
    }

    void HandleMovementInput()
    {
        var horInput = Input.GetAxis("Horizontal");
        var verInput = Input.GetAxis("Vertical");

        _newPos += transform.forward * _movementSpeed *Time.deltaTime * verInput;
        _newPos += transform.right * _movementSpeed * Time.deltaTime * horInput;
    }
    void HandleRotationInput()
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
    void HandleZoomInput()
    {
        if (Input.mouseScrollDelta.y!=0)
        {
            _newZoom += -Input.mouseScrollDelta.y * new Vector3(0,_zoomSpeed,-_zoomSpeed) * Time.deltaTime;
        }

        _newZoom = new Vector3(0, Mathf.Clamp(_newZoom.y, 8, 300), Mathf.Clamp(_newZoom.z, -300, -8));
        _cameraTransform.localPosition = Vector3.Lerp(_cameraTransform.localPosition, _newZoom, _zoomLerp * Time.deltaTime);
    }
    void HandleMousePan()
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
    void HandleMouseRotation()
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
}
