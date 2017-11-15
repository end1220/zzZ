
local RepeatForever = class("RepeatForever")

function RepeatForever:ctor(params)
    self.valid = true
    self.duration = 0
    self.action = params

end

function RepeatForever:setGameObj(gameObj)
    self.gameObj = gameObj
    self.action:setGameObj(gameObj)
end


function RepeatForever:update(dt)
    if self.action:update(dt) then
        self.action:reset()
    end

end


return RepeatForever