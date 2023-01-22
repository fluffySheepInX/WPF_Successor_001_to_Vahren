namespace WPF_Successor_001_to_Vahren._010_Enum
{
    public enum Situation
    {
        Title
        , MainMenu
        , SelectGroup
        , TextWindow_Conversation
        , PlayerTurn
        , EnemyTurn
        , InfoWindowMini
        , DebugGame
        , PlayerTurnEnemyCityLeftClick
        , PlayerTurnPlayerCityLeftClick
        , Battle_InfoWindowMini
        , Battle
        , BattleStop

        , Game
        , GenusList
        , ToolList
        , GameStop
        , PreparationBattle
        , PreparationBattle_UnitList
        , PreparationBattle_MiniWindow
    }

    public enum ButtonType
    {
        Scenario
        , Mail
        , Internet
    }

    public enum PowerType
    {
        I
        , Com
        , None
        , Boss
    }
    public enum Formation
    {
        F
        , M
        , B
    }
    public enum SkillFunc
    {
        missile
            ,
        sword
            ,
        heal
            ,
        summon
            ,
        charge
            ,
        status
    }
    public enum MapTipObjectType
    {
        WALL2
        , GATE
    }

    #region BattleWhichIsThePlayer
    /// <summary>
    /// 戦闘でどちらがプレイヤーか示す
    /// </summary>
    public enum BattleWhichIsThePlayer
    {
        Sortie
            ,
        Def
            ,
        None
    }
    #endregion

    #region aStarStatus
    /// <summary>
    /// A*アルゴリズムで使用
    /// </summary>
    public enum aStarStatus
    {
        None
            ,
        Open
            ,
        Closed
    }
    #endregion

    #region FlagBattleMapUnit
    /// <summary>
    /// 部隊チップの種別
    /// flag = 0 なら「ユニットの識別名」として扱う。
    /// flag = 1 なら「@文字変数」として扱う。
    /// flag = 2 なら「特殊な文字列」として扱う。
    /// 
    /// ユニットの識別名 同名のunit/class構造体ユニットが配置されます
    /// 
    /// @文字変数 @が接頭辞の文字列は文字変数と見なされます。代入スクリプトで防衛施設を変化できます
    /// 
    /// 特殊な文字列
    /// 「@」 防衛部隊の位置。
    /// 「@@」 侵攻部隊の位置。
    /// 「@ESC@」 城兵の退却位置になります。
    /// 
    /// </summary>
    public enum FlagBattleMapUnit
    {
        Unit = 0
            ,
        Var = 1
            ,
        Spe = 2
    }
    #endregion

    #region PowerFix
    /// <summary>
    /// off	普通に侵攻する（デフォルト）
    /// on	他国に侵攻しない
    /// home	homeデータに列挙された領地のみ侵攻する
    /// hold	領内の部隊を動かさず、他国にも侵攻しない
    /// warlike	兵力差に関係なく常に攻め込む
    /// freeze	ターン行動を完全スルーする
    /// </summary>
    public enum FlagPowerFix
    {
        on = 0
            ,
        off = 1
            ,
        home = 2
            ,
        hold = 3
            ,
        warlike = 4
            ,
        freeze = 5
    }
    #endregion

    #region Gender
    public enum Gender
    {
        Male = 0
            ,
        Female = 1
            ,
        Neuter = 2
            ,
        Androgynous = 3
            ,
        infertile = 4
    }
    #endregion
}
