-- Seed default users (plain-text passwords match AuthService TODO; use BCrypt later)
INSERT INTO "Users" ("Id", "Username", "Email", "PasswordHash", "Role", "FullName", "IsActive", "CreatedAt")
VALUES
  (gen_random_uuid(), 'admin', 'admin@janome.com', 'admin1234', 'Admin', 'ผู้ดูแลระบบ', true, NOW()),
  (gen_random_uuid(), 'hr01', 'hr01@janome.com', 'hr1234', 'HR', 'คุณสมศรี ฝ่ายบุคคล', true, NOW()),
  (gen_random_uuid(), 'manager01', 'manager@janome.com', 'manager1234', 'Manager', 'คุณวิชัย หัวหน้าผลิต', true, NOW())
ON CONFLICT ("Username") DO NOTHING;
