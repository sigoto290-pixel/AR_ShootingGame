# ARShooting

## 概要

このプロジェクトは、Unityで制作されたAR（拡張現実）シューティングゲームです。
デバイスのカメラ映像を現実空間の背景として利用し、ジャイロセンサーで視点操作を行う**3DoF (Three Degrees of Freedom)** のAR体験を提供します。プレイヤーは物理的にデバイスを傾けて照準を合わせ、画面に出現するターゲットを破壊してハイスコアを目指します。

### デモプレイ

このプロジェクトによるゲームは [unityroom](https://unityroom.com/games/gun_swap_shooter) でプレイできます。

## ゲームプレイ

1.  **タイトルと難易度選択:** ゲームを開始すると、タイトル画面が表示され、プレイヤーは難易度を選択します。
2.  **ターゲット出現:** ゲームが始まると、静止しているターゲット、移動するターゲット、赤と青の撃ち分けが必要なターゲットなど、様々な種類のターゲットが空間に動的に生成されます。
3.  **照準と射撃:** プレイヤーはデバイス本体を動かして2丁の銃の照準をターゲットに合わせ、画面をタップして弾を発射します。
4.  **スコアリング:** ターゲットを破壊するとスコアが加算されます。連続してターゲットを破壊することでスコア倍率が上昇するコンボシステムが特徴です。

## AR機能の実装方式

このプロジェクトは、AR Foundationのような高度なARフレームワークを使用せず、Unityの標準機能を用いてAR体験を実現しています。

-   **パススルーAR:** `WebCameraImage.cs` が `GameManager.cs` で管理される `WebCamTexture` を使用し、デバイスのカメラ映像を取得してUIの背景として常時表示します。
-   **3DoFコントロール:** `GameManager.cs` がUnityの `Input System` に含まれる `AttitudeSensor` からデバイスの回転情報を取得します。その情報を `CameraController.cs` がカメラの向きに反映させることで、プレイヤーの視点操作を実現しています。

## プロジェクトのアーキテクチャ

コンポーネントベースのアーキテクチャで構築されており、役割ごとにスクリプトが明確に分離されています。

-   **Manager (`Assets/Script/Manager`):**
    -   `GameManager.cs`: ゲーム全体の状態（難易度、`WebCamTexture`、`AttitudeSensor`の管理）、シーン遷移を担うシングルトン。
    -   `StageManager.cs`: ステージの進行、スコア、制限時間などを管理。
    -   `SoundManager.cs`: 効果音やBGMの再生を管理。
    -   `StageUI_manager.cs`, `Title1UI_Manager.cs`など: 各シーンのUIを管理。
-   **Player (`Assets/Script/Player`):**
    -   `Player.cs`: 弾の発射、赤と青の銃の切り替え、照準合わせのロジックを管理。
    -   `CameraController.cs`: `GameManager`から受け取った姿勢情報をもとに、カメラの回転を制御。
    -   `Bullet.cs`: 弾の振る舞いを定義。
-   **PointObjects (`Assets/Script/PointObjects`):**
    -   `PointObjectGenerator2.cs`: 難易度やコスト計算、Perlinノイズなどを用いて、ターゲットを動的に生成・配置するゲームプレイの中核。
    -   `PointObject.cs`: 全てのターゲットの振る舞いを定義する抽象基底クラス。被弾処理や時間経過、消滅時のアニメーションなどを管理。
    -   `RedTarget.cs`, `BlueTarget.cs`, `MoveRedTarget.cs`など: `PointObject`を継承した具体的なターゲットクラス群。

## 制作環境

-   **Unity:** `6000.0.60f1`
-   **主要なパッケージ:**
    -   `com.unity.inputsystem`: プレイヤーの入力およびデバイスのセンサー情報取得に使用。
    -   `com.unity.render-pipelines.universal`: レンダリングパイプラインとしてURPを使用。
    -   `com.unityroom.client`: [unityroom](https://unityroom.com/) のランキング機能との連携に使用。

## セットアップと実行

1.  Unity Editor `6000.0.60f1`以降でこのプロジェクトを開きます。
2.  `Assets/Scenes/title1.unity` または `title2.unity` シーンを開きます。
3.  再生ボタンを押してゲームを開始します。