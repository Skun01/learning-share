# Sprint 1: Authentication & User Management - API Documentation

> **Base URL**: `http://localhost:5000/api`
>
> **Authentication**: Các endpoint có đánh dấu 🔒 yêu cầu gửi header `Authorization: Bearer <access_token>`

---

## Table of Contents

1. [Response Format](#response-format)
2. [Error Codes](#error-codes)
3. [Authentication APIs](#1-authentication-apis)
4. [User Profile APIs](#2-user-profile-apis)
5. [User Settings APIs](#3-user-settings-apis)
6. [Media APIs](#4-media-apis)

---

## Response Format

Tất cả API responses đều tuân theo format sau:

> [!IMPORTANT] > **HTTP Status luôn trả về 200** cho các lỗi business logic có thể dự đoán được.
> Kiểm tra field `success` để xác định request có thành công hay không.
> Field `code` trong response body chỉ định loại lỗi cụ thể.

```json
{
  "code": 200,
  "success": true,
  "message": "Optional message",
  "data": { ... },
  "metaData": null
}
```

### Success Response

| Field      | Type      | Description              |
| ---------- | --------- | ------------------------ |
| `code`     | `number`  | `200` khi thành công     |
| `success`  | `boolean` | `true` nếu thành công    |
| `message`  | `string?` | Message optional         |
| `data`     | `object`  | Response data            |
| `metaData` | `object?` | Pagination info (nếu có) |

### Error Response

```json
{
  "code": 200,
  "success": false,
  "message": "Error_Code",
  "data": null,
  "metaData": null
}
```

> [!NOTE]
>
> - HTTP Status = `200` + `success: false` → Business logic error (đoán được)
> - HTTP Status = `401` → Token không hợp lệ/hết hạn (UnauthorizedAccessException)
> - HTTP Status = `404` → Resource không tồn tại (KeyNotFoundException)
> - HTTP Status = `500` → Lỗi server không đoán được

---

## Error Codes

> **Lưu ý**: Các error codes dưới đây được trả về trong field `message` của response body.
> HTTP Status vẫn là `200` cho hầu hết các lỗi business logic.

### Authentication Errors

| Message                            | Description                     |
| ---------------------------------- | ------------------------------- |
| `Invalid_400`                      | Email hoặc mật khẩu không đúng  |
| `Reset_Password_Token_Expired_400` | Token reset password đã hết hạn |
| `Current_Password_Invalid_400`     | Mật khẩu hiện tại không đúng    |
| `Invalid_Refresh_Token_400`        | Refresh token không hợp lệ      |
| `Refresh_Token_Expired_400`        | Refresh token đã hết hạn        |
| `User_Email_409`                   | Email đã tồn tại                |

### Common Errors

| Message      | Description             |
| ------------ | ----------------------- |
| `Common_404` | Không tìm thấy resource |
| `Common_400` | Request không hợp lệ    |
| `Common_505` | Lỗi server              |

---

## 1. Authentication APIs

### 1.1 Login

Đăng nhập và nhận tokens.

```
POST /auth/login
```

#### Request Body

```json
{
  "email": "user@example.com",
  "password": "your_password"
}
```

| Field      | Type     | Required | Description     |
| ---------- | -------- | -------- | --------------- |
| `email`    | `string` | ✅       | Email đăng nhập |
| `password` | `string` | ✅       | Mật khẩu        |

#### Response

```json
{
  "code": 200,
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4...",
    "user": {
      "id": 1,
      "username": "john_doe",
      "email": "user@example.com",
      "role": "Learner",
      "avatarUrl": "https://storage.example.com/avatars/1.jpg"
    }
  }
}
```

#### Errors

| Message       | Khi nào                 |
| ------------- | ----------------------- |
| `Invalid_400` | Email hoặc password sai |

---

### 1.2 Register

Đăng ký tài khoản mới.

```
POST /auth/register
```

#### Request Body

```json
{
  "username": "john_doe",
  "email": "user@example.com",
  "password": "secure_password123"
}
```

| Field      | Type     | Required | Description    |
| ---------- | -------- | -------- | -------------- |
| `username` | `string` | ✅       | Tên hiển thị   |
| `email`    | `string` | ✅       | Email (unique) |
| `password` | `string` | ✅       | Mật khẩu       |

#### Response

Giống như Login response.

#### Errors

| Message          | Khi nào               |
| ---------------- | --------------------- |
| `User_Email_409` | Email đã được sử dụng |

---

### 1.3 Forgot Password

Gửi email chứa link reset password.

```
POST /auth/forgot-password
```

#### Request Body

```json
{
  "email": "user@example.com"
}
```

| Field   | Type     | Required | Description     |
| ------- | -------- | -------- | --------------- |
| `email` | `string` | ✅       | Email tài khoản |

#### Response

```json
{
  "code": 200,
  "success": true,
  "data": true
}
```

> [!NOTE]
> API luôn trả về `true` để tránh leak thông tin email có tồn tại hay không.

---

### 1.4 Reset Password

Đặt lại mật khẩu với token từ email.

```
POST /auth/reset-password
```

#### Request Body

```json
{
  "token": "reset_token_from_email",
  "newPassword": "new_secure_password"
}
```

| Field         | Type     | Required | Description          |
| ------------- | -------- | -------- | -------------------- |
| `token`       | `string` | ✅       | Token từ email reset |
| `newPassword` | `string` | ✅       | Mật khẩu mới         |

#### Response

```json
{
  "code": 200,
  "success": true,
  "data": true
}
```

#### Errors

| Message                            | Khi nào                    |
| ---------------------------------- | -------------------------- |
| `Common_404`                       | Token không tồn tại        |
| `Reset_Password_Token_Expired_400` | Token đã hết hạn (15 phút) |

---

### 1.5 Refresh Token

Làm mới access token bằng refresh token.

```
POST /auth/refresh
```

#### Request Body

```json
{
  "refreshToken": "your_refresh_token"
}
```

| Field          | Type     | Required | Description            |
| -------------- | -------- | -------- | ---------------------- |
| `refreshToken` | `string` | ✅       | Refresh token hiện tại |

#### Response

```json
{
  "code": 200,
  "success": true,
  "data": {
    "accessToken": "new_access_token...",
    "refreshToken": "new_refresh_token...",
    "user": {
      "id": 1,
      "username": "john_doe",
      "email": "user@example.com",
      "role": "Learner",
      "avatarUrl": "https://storage.example.com/avatars/1.jpg"
    }
  }
}
```

> [!IMPORTANT]
> Refresh token cũ sẽ bị revoke sau khi gọi API này. Luôn lưu refresh token mới.

#### Errors

| Message                     | Khi nào                               |
| --------------------------- | ------------------------------------- |
| `Invalid_Refresh_Token_400` | Token không tồn tại hoặc đã bị revoke |
| `Refresh_Token_Expired_400` | Token đã hết hạn                      |

---

### 1.6 Revoke Token (Logout)

Thu hồi refresh token khi logout.

```
POST /auth/revoke
```

#### Request Body

```json
{
  "refreshToken": "your_refresh_token"
}
```

| Field          | Type     | Required | Description              |
| -------------- | -------- | -------- | ------------------------ |
| `refreshToken` | `string` | ✅       | Refresh token cần revoke |

#### Response

```json
{
  "code": 200,
  "success": true,
  "data": true
}
```

#### Errors

| Message                     | Khi nào             |
| --------------------------- | ------------------- |
| `Invalid_Refresh_Token_400` | Token không tồn tại |

---

## 2. User Profile APIs

### 2.1 Get Current User 🔒

Lấy thông tin profile của user đang đăng nhập.

```
GET /users/me
```

#### Headers

```
Authorization: Bearer <access_token>
```

#### Response

```json
{
  "code": 200,
  "success": true,
  "data": {
    "id": 1,
    "username": "john_doe",
    "email": "user@example.com",
    "role": "Learner",
    "avatarUrl": "https://storage.example.com/avatars/1.jpg",
    "settings": {
      "enableGhostMode": false,
      "dailyGoal": 20,
      "uiLanguage": "vi"
    }
  }
}
```

#### Response Schema: `UserProfileDTO`

| Field       | Type              | Description                |
| ----------- | ----------------- | -------------------------- |
| `id`        | `number`          | User ID                    |
| `username`  | `string`          | Tên hiển thị               |
| `email`     | `string`          | Email                      |
| `role`      | `string`          | `"Admin"` hoặc `"Learner"` |
| `avatarUrl` | `string?`         | URL avatar (có thể null)   |
| `settings`  | `UserSettingsDTO` | Cài đặt user               |

---

### 2.2 Update Profile 🔒

Cập nhật thông tin profile.

```
PATCH /users/info
```

#### Headers

```
Authorization: Bearer <access_token>
```

#### Request Body

```json
{
  "username": "new_username"
}
```

| Field      | Type     | Required | Description |
| ---------- | -------- | -------- | ----------- |
| `username` | `string` | ✅       | Tên mới     |

#### Response

```json
{
  "code": 200,
  "success": true,
  "data": true
}
```

---

### 2.3 Upload Avatar 🔒

Upload/thay đổi avatar.

```
POST /users/avatar
```

#### Headers

```
Authorization: Bearer <access_token>
Content-Type: multipart/form-data
```

#### Request Body (Form Data)

| Field  | Type   | Required | Description                    |
| ------ | ------ | -------- | ------------------------------ |
| `file` | `File` | ✅       | File ảnh (jpg, png, gif, webp) |

#### Response

```json
{
  "code": 200,
  "success": true,
  "data": "https://storage.example.com/avatars/1_new.jpg"
}
```

> [!TIP]
> Response trả về URL của avatar mới để frontend update UI.

---

### 2.4 Change Password 🔒

Thay đổi mật khẩu (khi đã đăng nhập).

```
PATCH /users/password
```

#### Headers

```
Authorization: Bearer <access_token>
```

#### Request Body

```json
{
  "currentPassword": "old_password",
  "newPassword": "new_secure_password"
}
```

| Field             | Type     | Required | Description       |
| ----------------- | -------- | -------- | ----------------- |
| `currentPassword` | `string` | ✅       | Mật khẩu hiện tại |
| `newPassword`     | `string` | ✅       | Mật khẩu mới      |

#### Response

```json
{
  "code": 200,
  "success": true,
  "data": true
}
```

#### Errors

| Message                        | Khi nào                      |
| ------------------------------ | ---------------------------- |
| `Current_Password_Invalid_400` | Mật khẩu hiện tại không đúng |

---

## 3. User Settings APIs

### 3.1 Get Settings 🔒

Lấy cài đặt user.

```
GET /settings
```

#### Headers

```
Authorization: Bearer <access_token>
```

#### Response

```json
{
  "code": 200,
  "success": true,
  "data": {
    "enableGhostMode": false,
    "dailyGoal": 20,
    "uiLanguage": "vi"
  }
}
```

#### Response Schema: `UserSettingsDTO`

| Field             | Type      | Description                        |
| ----------------- | --------- | ---------------------------------- |
| `enableGhostMode` | `boolean` | Chế độ ẩn danh                     |
| `dailyGoal`       | `number`  | Mục tiêu học/ngày                  |
| `uiLanguage`      | `string`  | Ngôn ngữ UI (`"vi"`, `"en"`, etc.) |

---

### 3.2 Update Ghost Mode 🔒

Bật/tắt chế độ ẩn danh.

```
PATCH /settings/ghost-mode
```

#### Headers

```
Authorization: Bearer <access_token>
```

#### Request Body

```json
{
  "enabled": true
}
```

| Field     | Type      | Required | Description           |
| --------- | --------- | -------- | --------------------- |
| `enabled` | `boolean` | ✅       | Trạng thái ghost mode |

#### Response

```json
{
  "code": 200,
  "success": true,
  "data": true
}
```

---

### 3.3 Update Daily Goal 🔒

Thay đổi mục tiêu học hàng ngày.

```
PATCH /settings/daily-goal
```

#### Headers

```
Authorization: Bearer <access_token>
```

#### Request Body

```json
{
  "goal": 30
}
```

| Field  | Type     | Required | Description          |
| ------ | -------- | -------- | -------------------- |
| `goal` | `number` | ✅       | Số thẻ mục tiêu/ngày |

#### Response

```json
{
  "code": 200,
  "success": true,
  "data": true
}
```

---

### 3.4 Update Language 🔒

Thay đổi ngôn ngữ giao diện.

```
PATCH /settings/language
```

#### Headers

```
Authorization: Bearer <access_token>
```

#### Request Body

```json
{
  "language": "en"
}
```

| Field      | Type     | Required | Description                  |
| ---------- | -------- | -------- | ---------------------------- |
| `language` | `string` | ✅       | Mã ngôn ngữ (`"vi"`, `"en"`) |

#### Response

```json
{
  "code": 200,
  "success": true,
  "data": true
}
```

#### Errors

| Message                    | Khi nào                    |
| -------------------------- | -------------------------- |
| `Language_Not_Support_400` | Ngôn ngữ không được hỗ trợ |

---

## 4. Media APIs

### 4.1 Upload Image 🔒

Upload hình ảnh (dùng cho cards, examples, etc.)

```
POST /media/image
```

#### Headers

```
Authorization: Bearer <access_token>
Content-Type: multipart/form-data
```

#### Request Body (Form Data)

| Field  | Type   | Required | Description |
| ------ | ------ | -------- | ----------- |
| `file` | `File` | ✅       | File ảnh    |

#### Response

```json
{
  "code": 200,
  "success": true,
  "data": {
    "id": 123,
    "url": "https://storage.example.com/images/123.jpg",
    "type": "Image"
  }
}
```

#### Response Schema: `MediaUploadResponse`

| Field  | Type     | Description                      |
| ------ | -------- | -------------------------------- |
| `id`   | `number` | Media ID (dùng để link vào card) |
| `url`  | `string` | URL public của file              |
| `type` | `string` | `"Image"` hoặc `"Audio"`         |

---

## Frontend Implementation Notes

### Token Management

- Sau khi login/register thành công, lưu `accessToken` và `refreshToken` vào localStorage hoặc secure storage
- Gửi `accessToken` trong header `Authorization: Bearer <token>` cho mọi request cần authentication
- Khi nhận HTTP 401, tự động gọi `/auth/refresh` để lấy token mới
- Nếu refresh thất bại, redirect về trang login

### Logout Flow

1. Gọi `POST /auth/revoke` với `refreshToken` để thu hồi token phía server
2. Xóa `accessToken` và `refreshToken` khỏi storage
3. Redirect về trang login

---

## UI Components Checklist

- [ ] **LoginPage** - Form đăng nhập
- [ ] **RegisterPage** - Form đăng ký
- [ ] **ForgotPasswordPage** - Form quên mật khẩu
- [ ] **ResetPasswordPage** - Form reset mật khẩu (với token từ URL)
- [ ] **ProfilePage** - Hiển thị & chỉnh sửa profile
- [ ] **SettingsPage** - Quản lý cài đặt
- [ ] **AvatarUpload** - Component upload avatar
- [ ] **ChangePasswordModal** - Modal đổi mật khẩu
