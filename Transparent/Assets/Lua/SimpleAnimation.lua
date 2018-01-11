local SimpleAnimation = class("SimpleAnimation", ModelBehaviour)

function SimpleAnimation:OnInit()
	--print('simple oninit')
	self.animator = self.gameObject:GetComponent("Animator")
	print(self.animator)
end

function SimpleAnimation:OnUpdate()
	--print('simple onupdate')
end

return SimpleAnimation