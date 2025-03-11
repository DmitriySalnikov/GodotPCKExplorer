@echo off

set godot=Godot_v4.2.2-stable_win64.exe
set ver=422

echo Place custom templates with encryption key and godot editor here
echo Required:
echo * %godot%
echo * godot.linuxbsd.template_debug.x86_64
echo * godot.windows.template_debug.x86_64.exe

set test_win=%~dp0Test%ver%\win
set test_linux=%~dp0Test%ver%\linux

mkdir %test_win%
mkdir %test_linux%

:: Import resources
%godot% -e --headless --quit --path v%ver%

title win test
%godot% -e --headless --quit --path v%ver% --export-debug win_test %test_win%\Test.exe
title win embedded
%godot% -e --headless --quit --path v%ver% --export-debug win_test_emb %test_win%\TestEmbedded.exe
title win encrypted
%godot% -e --headless --quit --path v%ver% --export-pack win_enc %test_win%\TestEncrypted.pck

title linux test
%godot% -e --headless --quit --path v%ver% --export-debug linux_test %test_linux%\Test
title linux embedded
%godot% -e --headless --quit --path v%ver% --export-debug linux_test_emb %test_linux%\TestEmbedded
title linux encrypted
%godot% -e --headless --quit --path v%ver% --export-pack linux_enc %test_linux%\TestEncrypted.pck
