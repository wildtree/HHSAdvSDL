# HHSAdvPico
High High School Adventure for SDL2

## 概要
いつもの、ハイハイスクールアドベンチャーの移植版です。
[SDL2](https://github.com/libsdl-org/SDL.git) 向けのものです。

## 遊び方
データファイルを AppData\Local\HHSAdvSDL の下に置いてください。

ビルドには net8.0 フレームワークへの依存性があります。
また SDL2-CS.dllにも依存しています。

実行時には SDL2-CS.dll, SDL2.dll, SDL2_ttf.dll, SDL2_image.dll, SDL2_mixer.dllへの依存があるので、それらをバイナリと同じフォルダーに置いてください。

ビルドした HHSDAdvSDL.exe を実行するか、または dotnet run で実行してください。

あとは、いつものように、さまよってください。
