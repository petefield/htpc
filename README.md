# HtpcVibes v1.1.0

HtpcVibes is a 10-foot user interface launcher for your home theatre PC.  It provides
a simple, couch-friendly dashboard for launching games and media apps,
with controller navigation support and a small web remote for phones and tablets.

### Features

- **Cross-platform:** Runs on Windows, Linux and macOS via Avalonia.
- **Controller input:** Uses SDL2 on Linux/macOS and XInput on Windows to handle
  gamepad navigation.  The D-pad/left stick moves focus, **A** selects and
  **B** backs out or quits (hold to quit).
- **Launch anything:** Define your launch targets in a `apps.json` file and
  they appear as tiles in the UI.  Each tile can specify a separate
  browser profile for services like Netflix and Prime Video.
- **Hot-reload:** Changes to `apps.json` are detected at runtime and update
  the tile grid without restarting the app.
- **Web remote:** A tiny web server runs on `127.0.0.1:7777` that exposes a
  minimalist remote control UI; use your phone to send up/down/left/right
  and A/B commands to the app.
- **Service files:** Includes example systemd unit and GNOME autostart entry
  for auto-launching on login.

See `CHANGELOG.md` for a history of changes.
