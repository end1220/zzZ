
local Spawn = class("Sequence")

function Spawn:ctor(params)
    self.valid = true
    self.duration = 0
    self.actions = {}
    self.maxSpawnId = table.nums(params)

    self:initSpawn(params)
end

function Spawn:reset()
    self.duration = 0
    for key,action in pairs(self.actions) do
        action.action:reset()
    end
end

function Spawn:initSpawn(actions)

    for key,action in pairs(actions) do
        self.actions[key] = {
            ["action"] = action,
            ["time"] = action.duration
        }      
    end
end

function Spawn:setGameObj(gameObj)
    self.gameObj = gameObj
    for key,action in pairs(self.actions) do
        action.action:setGameObj(gameObj)
    end
end

function Spawn:update(dt)
    local point = 0
    for key,action in pairs(self.actions) do
        if self.duration < action.time  then
            local callfunc = action.action
            callfunc:update(dt)
        else
            point = point + 1
        end
    end

    if point == self.maxSpawnId then
        return true
    end

    self.duration = self.duration + dt 
end

return Spawn