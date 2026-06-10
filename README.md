<div align="center">

# Wang Ngan HR

**ระบบรับสมัครงานครบวงจร — Desktop · Web · API**

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?style=for-the-badge&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Redis](https://img.shields.io/badge/Redis-7-DC382D?style=for-the-badge&logo=redis&logoColor=white)](https://redis.io/)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=for-the-badge&logo=docker&logoColor=white)](https://www.docker.com/)
[![WPF](https://img.shields.io/badge/Desktop-WPF-0078D4?style=for-the-badge&logo=windows&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
[![Blazor](https://img.shields.io/badge/Web-Blazor-512BD4?style=for-the-badge&logo=blazor&logoColor=white)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)

<br/>

<img src="docs/screenshots/desktop-job-postings.png" alt="Wang Ngan HR Dashboard" width="720"/>

*HR Dashboard — จัดการประกาศรับสมัคร ใบสมัคร และนัดสัมภาษณ์ในที่เดียว*

</div>

---

## ✨ ฟีเจอร์หลัก

| โมดูล | รายละเอียด |
|--------|------------|
| **ประกาศรับสมัคร** | สร้าง · แก้ไข · เผยแพร่ · ปิดรับ · สร้าง QR Code |
| **ใบสมัคร** | ติดตามสถานะ · ค้นหา · กรองตามตำแหน่ง/วันที่ |
| **ปฏิทินสัมภาษณ์** | นัด Online / Onsite · ดูตารางรายวัน |
| **พอร์ทัลผู้สมัคร** | ดูตำแหน่งว่าง · สมัครออนไลน์ · ติดตามสถานะด้วยรหัสอ้างอิง |
| **หลายภาษา** | ไทย · อังกฤษ · 日本語 |

---

## 🖥️ Screenshots

### HR Dashboard (Desktop)

<table>
<tr>
<td width="50%">

**ใบสมัคร — Applications**

<img src="docs/screenshots/desktop-applications.png" alt="Applications" width="100%"/>

สรุปสถิติรายวัน · ค้นหา · กรองสถานะ

</td>
<td width="50%">

**ประกาศรับสมัคร — Job Postings**

<img src="docs/screenshots/desktop-job-postings.png" alt="Job Postings" width="100%"/>

Draft / Open · เงินเดือน · QR Code

</td>
</tr>
<tr>
<td width="50%">

**สร้างประกาศ — Create Job**

<img src="docs/screenshots/desktop-create-job.png" alt="Create Job Posting" width="100%"/>

ฟอร์ม 2 ขั้นตอน · หน้าที่ · คุณสมบัติ

</td>
<td width="50%">

**ปฏิทินสัมภาษณ์ — Interview Calendar**

<img src="docs/screenshots/desktop-interview-calendar.png" alt="Interview Calendar" width="100%"/>

นัด Online / Onsite · สถานะ Scheduled

</td>
</tr>
</table>

### พอร์ทัลผู้สมัคร (Web)

<table>
<tr>
<td width="50%">

**ตำแหน่งงานว่าง (TH)**

<img src="docs/screenshots/web-jobs-th.png" alt="Web Jobs Thai" width="100%"/>

สแกน QR ที่ป้อมยาม หรือสมัครผ่านเว็บ

</td>
<td width="50%">

**Open Positions (EN)**

<img src="docs/screenshots/web-jobs-en.png" alt="Web Jobs English" width="100%"/>

รองรับหลายภาษา

</td>
</tr>
<tr>
<td width="50%">

**รายละเอียดตำแหน่ง**

<img src="docs/screenshots/web-job-detail-th.png" alt="Job Detail" width="100%"/>

หน้าที่ · คุณสมบัติ · สวัสดิการ

</td>
<td width="50%">

**ฟอร์มสมัครงาน**

<img src="docs/screenshots/web-apply-form.png" alt="Application Form" width="100%"/>

5 ขั้นตอน — ข้อมูลส่วนตัว → ส่งใบสมัคร

</td>
</tr>
<tr>
<td width="50%">

**ส่งใบสมัครสำเร็จ**

<img src="docs/screenshots/web-application-submitted.png" alt="Application Submitted" width="100%"/>

รับรหัสอ้างอิง WNG-XXXXX

</td>
<td width="50%">

**ติดตามสถานะ**

<img src="docs/screenshots/web-track-status.png" alt="Track Application" width="100%"/>

ค้นหาด้วยรหัส หรือชื่อ + เบอร์โทร

</td>
</tr>
</table>

---


| โปรเจกต์ | เทคโนโลยี | พอร์ต |
|----------|-----------|-------|
| `WangNganHR.API` | ASP.NET Core · EF Core · JWT · Swagger | `5083` |
| `WangNganHR.Desktop` | WPF · MVVM · FontAwesome | — |
| `WangNganHR.Web` | Blazor WebAssembly | `5203` |
| `WangNganHR.Shared` | DTOs · Models ร่วม | — |

**ฐานข้อมูล:** PostgreSQL 16 · **Cache / Jobs:** Redis 7 · **Background jobs:** Hangfire

---

## 🚀 Quick Start

### ความต้องการของระบบ

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Windows (สำหรับ Desktop app)

### รันด้วยคำสั่งเดียว

```powershell
cd C:\Projects\WangNganHR
.\start.ps1          # API + Desktop
.\start.ps1 -Web     # + Web portal
```

สคริปต์จะทำให้อัตโนมัติ:
1. เปิด PostgreSQL + Redis (Docker)
2. ตั้งค่า dev certificate (Desktop signing)
3. รัน API ที่ `http://localhost:5083`
4. Build & เปิด Desktop app

### คำสั่งอื่น ๆ

| คำสั่ง | หน้าที่ |
|--------|--------|
| `.\scripts\run-api.ps1` | รัน API อย่างเดียว |
| `.\scripts\stop-api.ps1` | หยุด API |
| `.\scripts\run-desktop.ps1` | Build + เปิด Desktop |
| `.\scripts\run-web.ps1` | รัน Web portal |
| `.\scripts\connect-db.ps1` | เข้า PostgreSQL ผ่าน psql |

### URL สำคัญ

| บริการ | URL |
|--------|-----|
| API / Swagger | http://localhost:5083/swagger |
| Web Portal | http://localhost:5203 |
| PostgreSQL | `localhost:5432` (db: `janomehr`) |

---

## 🔐 บัญชีทดสอบ

| Username | Password | Role |
|----------|----------|------|
| `admin` | `admin123` | Admin |
| `hr` | `hr123` | HR |
| `manager` | `manager123` | Manager |

> Manager ดูประกาศรับสมัครได้ — สร้าง/แก้ไข/เผยแพร่ ต้องใช้ HR หรือ Admin

---

## 📁 โครงสร้างโปรเจกต์

```
WangNganHR/
├── WangNganHR.API/          # REST API + EF Core migrations
├── WangNganHR.Desktop/      # WPF HR Dashboard
├── WangNganHR.Web/          # Blazor หน้ารับสมัครสาธารณะ
├── WangNganHR.Shared/       # Shared models & DTOs
├── scripts/                 # PowerShell dev scripts
├── docs/screenshots/        # README screenshots
├── docker-compose.yml       # PostgreSQL + Redis
└── start.ps1                # จุดเริ่มต้นหลัก
```

---

## ⚠️ หมายเหตุ

- **อย่าปิดหน้าต่าง API** ขณะใช้ Desktop / Web
- หลังแก้ UI Desktop ให้รัน `.\scripts\run-desktop.ps1` เพื่อ rebuild
- อย่ารัน `dotnet run` ซ้ำสำหรับ API — ใช้ `run-api.ps1` แทน

---

<div align="center">

**Wang Ngan** · v1.0 · ระบบรับสมัครงาน

</div>
