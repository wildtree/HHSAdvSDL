# HHSAdvPico
High High School Adventure for SDL2

## 概要
いつもの、ハイハイスクールアドベンチャーの移植版です。
[SDL2](https://github.com/libsdl-org/SDL.git) 向けのものです。

## 遊び方

~~データファイルを AppData\Local\HHSAdvSDL の下に置いてください。
Linuxでは ~/.local/share/HHSADvSDLになります。~~

データは必要に応じて自動的にデータフォルダーにコピーされます。

ビルドには net8.0 フレームワークへの依存性があります。
また SDL2-CS.dllにも依存しています。

実行時には SDL2-CS.dll, SDL2.dll, SDL2_ttf.dll, SDL2_image.dll, SDL2_mixer.dllへの依存があるので、それらをバイナリと同じフォルダーに置いてください。
Linux環境では、SDL2-CS.dllの名前解決のロジックに問題があるため、バイナリのあるフォルダから、libsdl2-2.0-0.so, libsdl2_image-2.0-0.so, libsdl2_mixer-2.0-0.so, libsdl2_ttf-2.0-0.soにそれぞれ、SDL2.dll.so, SDL2_image.dll.so, SDL2_mixer.dll.so, SDL2_ttf.dll.soという名前でシンボリックリンクをはってください。

ビルドした HHSDAdvSDL.exe を実行するか、または dotnet run で実行してください。

あとは、いつものように、さまよってください。

## 設定ファイル
アプリの終了時にデータファイル内のフォルダに、HHSAdvSDL.json という名前で設定ファイルが作られます。
FontPath、OpeningRoll、PlaySoundなどの属性があり、それぞれ編集することで任意のフォントや、オープニングのストーリー画面の表示・非表示、音を鳴らす・鳴らさないなどを設定できます。


