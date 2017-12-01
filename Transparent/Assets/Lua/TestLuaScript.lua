local TestLuaScript = class("TestLuaScript")
function TestLuaScript:Start()
	print("start")
end

function TestLuaScript:Update()
	print("update")
end

return TestLuaScript