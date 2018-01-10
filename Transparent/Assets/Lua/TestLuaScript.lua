local TestLuaScript = class("TestLuaScript", MonoBehaviour)

function TestLuaScript:Start()
	print("TestLuaScript start")
end

function TestLuaScript:Update()
	print("TestLuaScript update")
end

return TestLuaScript