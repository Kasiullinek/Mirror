using UnityEngine;
using Mirror;
using Cinemachine;
using Inputs;
using System.Collections;

public class PlayerCameraContoroller : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private Vector2 maxFollowOffset = new Vector2(-1f, 6f);
    [SerializeField] private Vector2 cameraVelocity = new Vector2(4f, 025f);
    [SerializeField] private Transform playerTransform = null;

    public CinemachineVirtualCamera virtualCameraFirstView = null;
    public CinemachineVirtualCamera virtualCameraThirdView = null;

    private Controls controls;
    private Controls Controls
    {
        get
        {
            if (controls != null) { return controls; }
            return controls = new Controls();
        }
    }

    private CinemachineTransposer transposerFirstView;
    private IEnumerator WaitForFourSeconds()
    {
        yield return new WaitForSeconds(4);

        virtualCameraFirstView.gameObject.SetActive(true);
        virtualCameraThirdView.gameObject.SetActive(false);

        StopCoroutine("WaitForFourSeconds");
    }

    public override void OnStartAuthority()
    {
        transposerFirstView = virtualCameraFirstView.GetCinemachineComponent<CinemachineTransposer>();

        virtualCameraThirdView.gameObject.SetActive(true);

        StartCoroutine("WaitForFourSeconds");

        enabled = true;

        Controls.Player.Look.performed += ctx => Look(ctx.ReadValue<Vector2>());
    }

    [ClientCallback]
    private void OnEnable()
    {
        Controls.Enable();
    }

    [ClientCallback]
    private void OnDisable()
    {
        Controls.Disable();
    }

    private void Look(Vector2 lookAxis)
    {
         float deltaTime = Time.deltaTime;
         float followOffset = Mathf.Clamp(transposerFirstView.m_FollowOffset.y - (lookAxis.y * cameraVelocity.y * deltaTime), maxFollowOffset.x, maxFollowOffset.y);
         transposerFirstView.m_FollowOffset.y = followOffset;
         playerTransform.Rotate(0f, lookAxis.x * cameraVelocity.x * deltaTime, 0f);
    }
}
