# HtpcVibes (Ubuntu GNOME + Xbox pad)

A 10-foot HTPC launcher built with **.NET 9 + Avalonia**.

## Features
- Big-tile launcher with Xbox controller navigation (SDL2)
- Netflix & Prime Video tiles in Chromium/Chrome kiosk mode (per-app profiles)
- Hot-reloadable tiles via `apps.json`
- Optional phone remote on `http://127.0.0.1:7777`
- Ubuntu GNOME autostart via **systemd user service** or **XDG autostart**

## Prereqs (Ubuntu)
```bash
sudo apt update
sudo apt install -y dotnet-sdk-9.0 libsdl2-2.0-0
sudo apt install -y chromium-browser || sudo apt install -y chromium
# or install google-chrome-stable via Google's repo
```

## Build & run
```bash
cd src/HtpcVibes
dotnet restore
dotnet run
```

## Autostart (systemd user service)
```bash
dotnet publish -c Release -r linux-x64 --self-contained true -o ~/apps/HtpcVibes
mkdir -p ~/.config/systemd/user
cp ../../../systemd/htpcvibes.service ~/.config/systemd/user/
systemctl --user daemon-reload
systemctl --user enable --now htpcvibes.service
```

## GNOME Autostart
```bash
mkdir -p ~/.config/autostart
cp ../..//autostart/htpcvibes.desktop ~/.config/autostart/
sed -i "s|/home/YOURUSER|$HOME|g" ~/.config/autostart/htpcvibes.desktop
```

## GitHub
```bash
git init
git add .
git commit -m "Initial commit: Avalonia HTPC launcher"
git branch -M main
git remote add origin <YOUR_REPO_URL>
git push -u origin main
```
