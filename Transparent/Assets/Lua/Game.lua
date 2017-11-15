
--unityengine
WWW = CS.UnityEngine.WWW;
GameObject = CS.UnityEngine.GameObject;
Vector3 = CS.UnityEngine.Vector3;
Vector2 = CS.UnityEngine.Vector2;
Color = CS.UnityEngine.Color;
Quaternion = CS.UnityEngine.Quaternion;

--framework
Framework = CS.TwFramework.GameFramework;
Utils = CS.TwFramework.AppUtils;
UnityUtils = CS.TwFramework.UnityUtils;
Log = CS.TwFramework.Log;
AppConst = CS.TwFramework.AppConst;
GameTimer = CS.TwFramework.GameTimer
LuaHelper = CS.TwFramework.LuaHelper;
ByteBuffer = CS.TwFramework.ByteBuffer;
UIEventListener = CS.TwUI.UIEventListener
panelMgr = Framework.GetPanelManager();
soundMgr = Framework.GetSoundManager();
networkMgr = Framework.GetNetworkManager();
UnityUtils = CS.TwFramework.UnityUtils
gGame = CS.TwGame.Game.Instance
soundSettings = CS.SoundSettings.Instance
DataManager = Framework.GetDataManager();
ActorManager = CS.TwGame.ActorManager.Instance

require "Common/functions"
require "ProtocolCode"
gNetwork = (require "Common/Network").new()

--require
require "Common/LuaUtils"
require "GameConfig"
require "HeroManager"
require "ItemManager"
require "RankManager"
require "ChatManager"
require "MailManager"
require "MatchManager"
require "UIDefine"
require 'CSharpPort'
require "Common/UITween"
require "GlobalVar"
require "Common/OperateManager"
require "Component/Components"
require "Common/StringBuilder"


globalVar = require "GlobalVar"
UIBase = require("UI/UIBase")
Sequence = require "Common/Sequence"
Spawn = require "Common/Spawn"
RepeatForever = require "Common/RepeatForever"
gTweenMgr  = (require "Common/UITweenManager").new()
gUIManager = (require "UIManager").new()
-- MsgPack = require "Common/MessagePack"
protobuf = require 'Common/protobuf'
require "UI/ProtocalHandler"

--game
Game = {}

local function RegisterProtobuf()
	local list = UnityUtils.GetProtocolName()
	for index = 0,list.Count-1,1 do 
		local cel = list[index]
		local res = UnityUtils.LoadProtocolData(cel).bytes
		protobuf.register(res)
	end
end

function Game.OnInitOK()
	Game.mainPlayer = (require "MainPlayer").new()

	local game = CS.TwGame.Game.Instance
	if game.NetMode then 
		--
	else
		local Session = CS.TwFramework.Session.Inst
		Session:LaunchDone()
		gUIManager:LoadMainWindow(GameUI.login,true)
	end

	RegisterProtobuf()

end

function Game.OnDestroy()
	--logWarn('OnDestroy--->>>');
end

function Game.SetNetMode(mode)
	local game = CS.TwGame.Game.Instance
	game.NetMode = mode
end

function Game.EnterBattleScene(sceneID, ...)
	local game = CS.TwGame.Game.Instance

	local Session = CS.TwFramework.Session.Inst
	Session:LoadBattleMap(sceneID)
	gUIManager:OpenWindow(GameUI.waiting,...)
end

function Game.GetGameMode()
    local game = CS.TwGame.Game.Instance
    if game.NetMode == true then 
        return 1
    elseif game.NetMode == false and LuaUtils.IsDebug == false then 
        return 2
    else 
        return 3
    end
end
