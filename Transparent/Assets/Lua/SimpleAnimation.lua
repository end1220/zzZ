local SimpleAnimation = class("SimpleAnimation", MonoBehaviour)

function SimpleAnimation:Start()
	print(self)
	self.animator = self.gameObject:GetComponent("UnityEngine.Animator")
	print(self.animator)
end

return SimpleAnimation