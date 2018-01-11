
Type = CS.System.Type
GameObject = CS.UnityEngine.GameObject
Vector3 = CS.UnityEngine.Vector3
Vector2 = CS.UnityEngine.Vector2
Quaternion = CS.UnityEngine.Quaternion
Color = CS.UnityEngine.Color

require "Common"
ModelBehaviour = require("ModelBehaviour")

Game = 
{
	currentModel = nil,
}

function Game.OnInit()
	
end

function Game.OnUpdate()
	if currentModel ~= nil then
		currentModel:Update()
	end
end

function Game.OnLoadModel(gameObject, moduleName)
	local mod = require(''..moduleName)
	currentModel = mod.new()
	currentModel:Init(gameObject)
end

function Game.OnUnloadModel(gameObject, moduleName)
	currentModel = nil
end