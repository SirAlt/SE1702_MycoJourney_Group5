using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;          // Tốc độ di chuyển của Player
    public float jumpForce = 7f;          // Lực nhảy của Player
    public int maxJumpCount = 2;          // Số lần nhảy tối đa (nhảy đôi)

    private Rigidbody2D rb;               // Thành phần Rigidbody2D của Player
    private bool isGrounded;              // Kiểm tra xem Player có chạm đất không
    private int jumpCount;                // Đếm số lần nhảy

    public Transform groundCheck;         // Điểm kiểm tra nếu chạm đất
    public float checkRadius = 0.2f;      // Bán kính để kiểm tra chạm đất
    public LayerMask groundLayer;         // Lớp đất để kiểm tra chạm đất

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Lấy thành phần Rigidbody2D
    }

    void Update()
    {
        // Xử lý di chuyển trái/phải
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Kiểm tra nếu đang chạm đất
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        if (isGrounded)
        {
            // Reset lại số lần nhảy khi chạm đất
            jumpCount = 0;
        }

        // Xử lý nhảy
        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || jumpCount < maxJumpCount))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
        }
    }
}
