# 🚀 Deployment Guide

Ahmed OS can be hosted using **Docker Containers**, **Azure App Service**, or a **Linux/Windows Virtual Machine**.

---

## 🐳 Option 1: Docker Compose (Recommended)

The easiest and most reliable way to run Ahmed OS in production with SQL Server.

### 1. Clone & Configure Environment
```bash
git clone https://github.com/AhmedElngar5/To-Do-project.git
cd To-Do-project
cp .env.example .env
```
Edit `.env` to set your strong `MSSQL_SA_PASSWORD` and `INITIAL_ADMIN_PASSWORD`.

### 2. Launch Containers
```bash
docker compose up -d --build
```

### 3. Verify Health
```bash
curl http://localhost:5000/health
```
Open your browser at `http://localhost:5000`.

---

## 💻 Option 2: Local Windows / Development
```bash
# Ensure .NET 10 SDK is installed
dotnet restore AhmedOS.slnx
dotnet build AhmedOS.slnx
dotnet run --project src/AhmedOS.Web
```
Default URL: `http://localhost:5086`

---

## ☁️ Option 3: Production Linux VM (systemd + Nginx)

### 1. Publish Release Build
```bash
dotnet publish src/AhmedOS.Web/AhmedOS.Web.csproj -c Release -o /var/www/ahmedos
```

### 2. Configure systemd service (`/etc/systemd/system/ahmedos.service`)
```ini
[Unit]
Description=Ahmed OS Web Application
After=network.target

[Service]
WorkingDirectory=/var/www/ahmedos
ExecStart=/usr/bin/dotnet /var/www/ahmedos/AhmedOS.Web.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=ahmedos
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

### 3. Start Service
```bash
sudo systemctl enable ahmedos
sudo systemctl start ahmedos
```
