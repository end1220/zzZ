
local Sequence = class("Sequence")

function Sequence:ctor(params)
    self.valid = true
    self.duration = 0
    self.sequenceId = 1
    self.actions = {}
    self.maxSequenceId = table.nums(params)

    self:initSequence(params)
end

function Sequence:reset()
    self.sequenceId = 1
    self.duration = 0
    for key,action in pairs(self.actions) do
        action.action:reset()
    end
end

function Sequence:setGameObj(gameObj)
    self.gameObj = gameObj
    for key,action in pairs(self.actions) do
        action.action:setGameObj(gameObj)
    end
end

function Sequence:initSequence(actions)
    local tempTime = 0
    local id = 0
    for key,action in pairs(actions) do
        id = id + 1
        tempTime = tempTime + (action.duration or 0)
        self.actions[id] = {
            ["action"] = action,
            ["id"] = id,
            ["time"] = tempTime
        }      
    end
end

function Sequence:update(dt)
    if self.sequenceId > self.maxSequenceId then
        return true
    else
        local data = self.actions[self.sequenceId]
        local action = data.action
        action:update(dt)

        if self.duration > data.time  then
            self.sequenceId = self.sequenceId + 1
        end
    end
    self.duration = self.duration + dt 
end


return Sequence