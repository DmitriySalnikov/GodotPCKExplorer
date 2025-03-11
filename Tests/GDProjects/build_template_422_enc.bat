@echo off
echo Don't forget to apply patches!!!

cd godot_422
set SCRIPT_AES256_ENCRYPTION_KEY=7FDBF68B69B838194A6F1055395225BBA3F1C5689D08D71DCD620A7068F61CBA
scons arch=x86_64 platform=windows windows_subsystem=console fast_unsafe=yes target=template_debug no_editor_splash=yes optimize=size lto=full vulkan=no use_volk=no disable_3d=yes disable_advanced_gui=yes openxr=no module_csg_enabled=no module_enet_enabled=no module_meshoptimizer_enabled=no module_mobile_vr_enabled=no module_navigation_enabled=no module_multiplayer_enabled=no module_openxr_enabled=no module_raycast_enabled=no module_webrtc_enabled=no module_websocket_enabled=no module_webxr_enabled=no