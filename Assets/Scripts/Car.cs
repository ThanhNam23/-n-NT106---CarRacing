using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Cinemachine;
using TMPro;

public class Car : NetworkBehaviour
{
    [SerializeField]
    private float tocDoXe = 800f;
    [SerializeField]
    private float lucReXe = 150f;
    [SerializeField]
    private float lucPhanh = 50f;
    [SerializeField]
    private GameObject hieuUngPhanh;

    [SerializeField] float boostSpeed = 2500f;
    [SerializeField] float reduceSpeed = 500f;
    [SerializeField] float normalSpeed = 800f;

    private float timer_Speed_Increase = 0f;
    private float timer_Speed_Decrease = 0f;
    private bool isPauseSI = true;
    private bool isPauseSD = true;
    private Rigidbody rb;

    [SerializeField] private TextMeshProUGUI playerNameUI;
    private Vector3 lastCheckpointPosition;

    [SerializeField] CinemachineVirtualCamera vc;
    [SerializeField] AudioListener listener;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }



    public void SetPlayerName(string name)
    {
        playerNameUI.text = name;
    }


    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            listener.enabled = true;
            vc.Priority = 1;
        }
        else
        {
            vc.Priority = 0;
        }
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            float moveInput = Input.GetAxis("Vertical");
            float turnInput = Input.GetAxis("Horizontal");
            HandleSpeedEffects();

            // Gửi điều khiển đầu vào lên server
            SendMovementInputServerRpc(moveInput, turnInput);

            if (moveInput > 0 && Input.GetKey(KeyCode.LeftShift))
            {
                PhanhXeServerRpc();  // Gọi phanh từ server
            }
        }
    }

    private bool time_Manager(bool Pausing)
    {
        Pausing = !Pausing;
        return Pausing;
    }

    private void HandleSpeedEffects()
    {
        // Xử lý tăng tốc độ
        if (!isPauseSI)
        {
            timer_Speed_Increase -= Time.deltaTime;
            if (timer_Speed_Increase <= 0f)
            {
                isPauseSI = time_Manager(isPauseSI);
                tocDoXe = normalSpeed; // Đưa tốc độ về bình thường
            }
        }

        // Xử lý giảm tốc độ
        if (!isPauseSD)
        {
            timer_Speed_Decrease -= Time.deltaTime;
            if (timer_Speed_Decrease <= 0f)
            {
                isPauseSD = time_Manager(isPauseSD);
                tocDoXe = normalSpeed; // Đưa tốc độ về bình thường
            }
        }
    }


    [ServerRpc]
    private void SendMovementInputServerRpc(float moveInput, float turnInput)
    {
        // Tính toán vị trí và hướng mới
        DiChuyenXe(moveInput);
        ReXe(turnInput);

        // Gọi ClientRpc để cập nhật cho các client khác
        UpdatePositionClientRpc(transform.position, transform.rotation);
    }

    [ClientRpc]
    private void UpdatePositionClientRpc(Vector3 position, Quaternion rotation)
    {
        if (!IsOwner)  // Không thực thi trên client sở hữu
        {
            transform.position = position;
            transform.rotation = rotation;
        }
    }

    [ServerRpc]
    private void PhanhXeServerRpc()
    {
        if (rb.velocity.z != 0)
        {
            rb.AddRelativeForce(-Vector3.forward * lucPhanh);
            hieuUngPhanh.SetActive(true);
            UpdateBrakeEffectClientRpc();
        }
    }

    [ClientRpc]
    private void UpdateBrakeEffectClientRpc()
    {
        if (!IsOwner)
        {
            hieuUngPhanh.SetActive(true);
        }
    }


    private void PhanhXe()
    {
        if (rb.velocity.z != 0)
        {
            rb.AddRelativeForce(-Vector3.forward * lucPhanh);
            hieuUngPhanh.SetActive(true);
        }
    }

    public void DiChuyenXe(float diChuyen)
    {
        rb.AddRelativeForce(Vector3.forward * diChuyen * tocDoXe);
        hieuUngPhanh.SetActive(false);
    }

    public void ReXe(float re)
    {
        Quaternion xoay = Quaternion.Euler(Vector3.up * re * lucReXe * Time.deltaTime);
        rb.MoveRotation(rb.rotation * xoay);
    }

    private void LateUpdate()
    {
        playerNameUI.transform.LookAt(Camera.main.transform);
        playerNameUI.transform.Rotate(0, 180, 0); // Quay ngược lại để hiển thị đúng
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacles")
        {
            tocDoXe = normalSpeed;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Barriers" || collision.gameObject.tag == "Player")
        {
            tocDoXe = normalSpeed;
        }
        else if (collision.gameObject.tag == "Ground")
        {
            transform.position = lastCheckpointPosition;
        }
    }
    private IEnumerator SpeedBoost(float newSpeed, float duration)
    {
        float originalSpeed = tocDoXe;
        tocDoXe = newSpeed;
        yield return new WaitForSeconds(duration);
        tocDoXe = normalSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Speed_Increase")
        {
            StartCoroutine(SpeedBoost(boostSpeed, 10f));
            Destroy(other.gameObject);
        }
        else if (other.tag == "Speed_Descrease")
        {
            StartCoroutine(SpeedBoost(reduceSpeed, 10f));
            Destroy(other.gameObject);
        }
        else if (other.tag == "Checkpoint")
        {
            lastCheckpointPosition = other.transform.position;
        }
    }
}
