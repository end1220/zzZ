local MonoBehaviour = class("MonoBehaviour")

function MonoBehaviour:Awake()
	print("MonoBehaviour update")
end

function MonoBehaviour:Start()
	print("MonoBehaviour update")
end

function MonoBehaviour:Update()
	print("MonoBehaviour update")
end

return MonoBehaviour