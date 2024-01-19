@echo off
echo Don't forget to apply patches!!!

cd godot_3
set SCRIPT_AES256_ENCRYPTION_KEY=7FDBF68B69B838194A6F1055395225BBA3F1C5689D08D71DCD620A7068F61CBA
scons arch="x64" bits="64" platform="windows" windows_subsystem="console" fast_unsafe="yes" tools="no" target="release_debug" no_editor_splash="YES" optimize="size"