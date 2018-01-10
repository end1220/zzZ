
--unityengine
WWW = CS.UnityEngine.WWW;
GameObject = CS.UnityEngine.GameObject;
Vector3 = CS.UnityEngine.Vector3;
Vector2 = CS.UnityEngine.Vector2;
Color = CS.UnityEngine.Color;
Quaternion = CS.UnityEngine.Quaternion;

require "Common"


Game = {}

function Game.OnInitOK()
	print("this is lua")
end

function Game.New(scriptPath)
	print(scriptPath)
	local cls = require (""..scriptPath)
	print(cls)
	return cls.new()
end