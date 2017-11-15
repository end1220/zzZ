
-- require "3rd/pbc/protobuf"
local table = table
Network = class("Network")
local HttpClient = CS.TWHttpClient.Instance
local Client = CS.TwFramework.Client.Inst
local httpProtocoel = {
    [actionCodes.IDLogin] = true,
    [actionCodes.IDFastLogin] = true,
}

function MsgUnpack(body,msg)
    return protobuf.decode("proto2."..body, msg)
end

function Network:ctor()
    self.once_messages = {}
    self.global_message = {}
end

function Network:sendData(actionCode , msgBody , info)
    local encode
    if msgBody then
        encode = protobuf.encode("proto2."..msgBody, info)
    end
    --http protocol special handle
    if httpProtocoel[actionCode] then
        if not msgBody then 
            HttpClient:StartGet(actionCode)
        else
            HttpClient:StartPut(actionCode,encode)
        end
    else
        Client:LuaSendMessage(actionCode,encode)
    end
    -- log("sendData actionCode:"..actionCode)
end

function Network:addResponseHandler(actionCode, callback)
    assert(actionCode ~= nil)
    self.once_messages[actionCode] = {["callback"] = callback}
end

function Network:addGlobalResponseHandler(actionCode, callback)
    assert(actionCode ~= nil)
    self.global_message[actionCode] = {["callback"] = callback}
end

function Network:removeResponseHandler(actionCode)
    if self.global_message[actionCode] then 
        self.global_message[actionCode] = nil
    end
end

function Network:OnMessage(actionCode,respData,respCode)
    -- log("OnMessage actionCode:"..actionCode)
    if self.once_messages[actionCode] then
        local message = self.once_messages[actionCode]
        if message.callback then 
            message.callback(respData,respCode)
        end
        self.once_messages[actionCode] = nil
    end

    if self.global_message[actionCode] then 
        local message = self.global_message[actionCode]
        if message.callback then 
            message.callback(respData,respCode)
        end
    end

end


return Network

