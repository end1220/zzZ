local ModelBehaviour = class("ModelBehaviour")

function ModelBehaviour:Init(gameObject)
	self.gameObject = gameObject
	self:OnInit()
end

function ModelBehaviour:Update()
	self:OnUpdate()
end

function ModelBehaviour:OnInit()
	
end

function ModelBehaviour:OnUpdate()
	
end

return ModelBehaviour