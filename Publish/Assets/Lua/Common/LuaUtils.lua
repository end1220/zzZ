local DataManager = CS.TwFramework.DataManager.Instance;
local UltimateTip = CS.UltimateTip.Instance

--输出日志--
function log(str)
    Log.Info(str);
end

--错误日志--
function logError(str) 
	Log.Error(str);
end

--警告日志--
function logWarn(str) 
	Log.Warning(str);
end

--查找对象--
function find(str)
	return GameObject.Find(str);
end

function destroy(obj)
	GameObject.Destroy(obj);
end

function newObject(prefab)
	return GameObject.Instantiate(prefab);
end

--创建面板--
function createPanel(name)
	PanelManager:CreatePanel(name);
end

function child(str)
	return transform:FindChild(str);
end

function subGet(childNode, typeName)		
	return child(childNode):GetComponent(typeName);
end

function findPanel(str) 
	local obj = find(str);
	if obj == nil then
		error(str.." is null");
		return nil;
	end
	return obj:GetComponent("BaseLua");
end

function ShowTip(str, color)
	UltimateTip:AppendTip(CS.UltimateTip.TipType.Float, str, color)
end

LuaUtils = {}

LuaUtils.IsDebug = false

function LuaUtils.SetOnClick(obj,func,instance) 
	assert(obj ~= nil)
    UIEventListener.SetOnClick(obj, func, instance)
end

function LuaUtils.SetButtonEvent(obj,func,eventtriggertype,instance) 
	assert(obj ~= nil)
    UIEventListener.SetButtonEvent(obj, func,eventtriggertype, instance)
end

function LuaUtils.AddTick(name,time,func,instance) 
    gTweenMgr:addAction(name,
            RepeatForever.new(
                Sequence.new({
                    DelayTime.new(time),
                    CallFunc.new(
                        func,
                        instance,
                        nil
                    ),       
                })
            )
        )
end

function LuaUtils.AddDelayTimeFunc(name, time, func, instance, param)
    if time <= 0 then
        func(instance,param)
        return 
    end
    gTweenMgr:addAction(name,
                    Sequence.new({
                        DelayTime.new(time),
                        CallFunc.new(func,
                        instance,
                        param
                        ),       
                    })
                )
end

function LuaUtils.RemoveAction(name) 
    gTweenMgr:StopAction(name)
end

function LuaUtils.GetParam(id,idx) 
    local vl = DataManager:GetTemplate("ValueEntry",id)
    if vl then
        if idx == 1 then
            return vl.param1
        elseif idx == 2 then
            return vl.param2
        elseif idx == 3 then
            return vl.param3
        elseif idx == 4 then
            return vl.param4
        elseif idx == 5 then
            return vl.param5
        elseif idx == 6 then
            return vl.param6
        elseif idx == 7 then
            return vl.param7
        elseif idx == 8 then
            return vl.param8
        end
    end
    return nil
end

function LuaUtils.intToTimeHMS(time)
	local hour = math.floor(time / 3600)
	local min = math.floor((time %3600) / 60)
	local second = math.floor(time % 60)
	if hour > 0 then
		return string.format("%02d:%02d:%02d", hour, min, second)
	else
		return string.format("%02d:%02d", min, second)
	end
end

function LuaUtils.GetSystemTime() 
    return UnityUtils.GetSystemTime()
end

function LuaUtils.intToTimeHMSInTable(time)
    local timeTable = {};
	local hour = ( math.floor(time / 3600 ))%24
	local min = math.floor((time %3600) / 60)
	local second = math.floor(time % 60)
    timeTable.hour = hour
    timeTable.min = min
    timeTable.second = second
    return timeTable
end

function LuaUtils.intToTimeDHM(time)
	local day = math.floor(time/(3600*24))
	local hour = math.floor(time%(3600*24)/3600)
	local min = math.floor((time%(3600)) / 60)
	local hourstr = LuaUtils.GetStringWithParam(13);
	local minstr = LuaUtils.GetStringWithParam(20);
	if day > 0 then
		local daystr = LuaUtils.GetStringWithParam(12);
		return string.format("%d"..tostring(daystr).."%02d"..tostring(hourstr).."%02d"..tostring(minstr), day, hour, min)
	else
		return string.format("%02d"..tostring(hourstr).."%02d"..tostring(minstr), hour, min)
	end
end

function LuaUtils.intToTime(time)
    local extraTime = time
    local day = math.floor(extraTime/(3600*24))
    extraTime = extraTime - day * 3600*24
    local hour = math.floor( extraTime/(3600) )
    extraTime = extraTime - hour * 3600
    local min = math.floor( extraTime/(60) )
    extraTime = extraTime - min * 60
    local second = extraTime
    return {day = day , hour = hour ,min = min , second = second}
end

function LuaUtils.GetString(msgid)
	local linfo = DataManager:GetTemplate("LanguageEntry",msgid)
	if linfo == null then
		return ""
	end

	return linfo.Content
end

function LuaUtils.GetStringWithParam(msgid, ...)
	local arg = {...}
	arg.n = #{...}
	
	if 0 == arg.n then
		return LuaUtils.GetString(msgid)
	end
	
	local sb = StringBuilder:new()
	for i = 0, arg.n - 1 do
		local ss = "parameter"..tostring(i + 1)
		sb:Set(ss, arg[i + 1])
	end
	
    local msgstr = sb:GetString(LuaUtils.GetString(msgid))
    sb:delete()
	return msgstr
end

function LuaUtils.GetStringWithParamList(msgid, arglist)
	local arg = {}
	arg.n = 0
    if arglist then
        arg = arglist
	    arg.n = #arglist
    end
	
	if 0 == arg.n then
		return LuaUtils.GetString(msgid)
	end
	
	local sb = StringBuilder:new()
	for i = 0, arg.n - 1 do
		local ss = "parameter"..tostring(i + 1)
		sb:Set(ss, arg[i + 1])
	end
	
    local msgstr = sb:GetString(LuaUtils.GetString(msgid))
    sb:delete()
	return msgstr
end

function LuaUtils.GetTemplate(temp, id)
    return DataManager:GetTemplate(temp,id)
end

function LuaUtils.GetActorTemplateFromSkinId(skinid)
    local templatenum = DataManager:GetListNum("ActorEntry")
    for i = 1,templatenum do
        local actinfo = DataManager:GetTemplate("ActorEntry",i)
        for i = 0, actinfo.Skin.Length - 1 do
            if actinfo.Skin[i] == skinid then
                return actinfo
            end
        end
    end
    return null 
end

function LuaUtils.addResponseHandler(actionCode,callback)
	gNetwork:addResponseHandler(actionCode,callback)
end

function LuaUtils.addGlobalResponseHandler(actionCode,callback)
	gNetwork:addGlobalResponseHandler(actionCode,callback)
end

function LuaUtils.sendData(actionCode , msgBody , info)
	gNetwork:sendData(actionCode, msgBody, info)
end

function LuaUtils.IsPassiveAbility(skillid)
    local templatenum = DataManager:GetListNum("ActorEntry")
    for i = 1,templatenum do
        local tp = DataManager:GetTemplate("ActorEntry",i)
        if tp then
            if tp.PassiveAbility == skillid then
                return true
            end
        end
    end
    return false
end

return LuaUtils