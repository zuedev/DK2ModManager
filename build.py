import os
import platform

# --- Configuration ---

# The path to the Godot executable. Change this to match your system.
if platform.system() == "Windows":
    godot_executable = "C:\\Program Files\\Godot\\Godot_v4.5-rc1_mono_win64.exe"
elif platform.system() == "Darwin":
    godot_executable = "/Applications/Godot.app/Contents/MacOS/Godot"
else:
    godot_executable = "/usr/bin/godot"

# The export presets to build.
export_presets = [
    "Linux",
    "Windows Desktop",
    "macOS",
]

# --- Build Script ---

for preset in export_presets:
    print(f"Building {preset}...")
    os.system(f'"{godot_executable}" --headless --path . --export-release "{preset}"')

print("Build complete!")
