@echo off

set godot=Godot_v3.5.3-stable_win64.exe

echo Place custom templates with encryption key and godot editor here
echo Required:
echo * %godot%
echo * godot_server.x11.opt.debug.64
echo * godot.windows.opt.debug.x64.exe

set test_win=%~dp0Test3\win
set test_linux=%~dp0Test3\linux

mkdir %test_win%
mkdir %test_linux%

:: Import resources
%godot% -e --headless --quit --path v3

title win test
%godot% -e --headless --quit --path v3 --export-debug win_test %test_win%\Test.exe
title win embedded
%godot% -e --headless --quit --path v3 --export-debug win_test_emb %test_win%\TestEmbedded.exe
title win encrypted
%godot% -e --headless --quit --path v3 --export-pack win_enc %test_win%\TestEncrypted.pck

title linux test
%godot% -e --headless --quit --path v3 --export-debug linux_test %test_linux%\Test
title linux embedded
%godot% -e --headless --quit --path v3 --export-debug linux_test_emb %test_linux%\TestEmbedded
title linux encrypted
%godot% -e --headless --quit --path v3 --export-pack linux_enc %test_linux%\TestEncrypted.pck
