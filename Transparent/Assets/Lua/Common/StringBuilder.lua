StringBuilder = {}
StringBuilder.__index = StringBuilder


function StringBuilder:new()
	--print("string builder new")
	local self = {}
	setmetatable(self, StringBuilder)
	self.rules = {}
	return self
end

function StringBuilder:delete()
	--print("string builder delete")
	self = nil
end

--change $str1$ with str2
function StringBuilder:Set(str1, str2)
	self.rules["%$" .. str1 .. "%$"] = str2
end

--change $str$ with num
function StringBuilder:SetNum(str, num)
	self.rules["%$" .. str .. "%$"] = tostring(num)
end

function StringBuilder:GetString(str)
	local resultStr = str
	if not resultStr then return "" end 
	for i,v in pairs(self.rules) do
		if v then
			if type(v) == "string" or type(v) == "number" then
				resultStr = string.gsub(resultStr, i, v)
			else
				logError("[LUA ERROR:] StringBuilder GetString!!! "..resultStr)
				return resultStr
			end
		end
	end
	return resultStr
end

function StringBuilder.split(str,sep)
	local t = {}
	for w in string.gfind(str, "[^"..sep.."]+")do
		table.insert(t, w)
	end
	return t
end

--给定的字符串是带格式的情况时,将其颜色全都替换为给定颜色
--给定的字符串不带格式时,将其格式为给定颜色
function StringBuilder.GetFormatStringReplaceColor(strMsg, strColor)
	local strResult = ""
	local posS,posE = string.find(strMsg, "<")
	local strLast = string.sub(strMsg, -1)
	if posS and posE and posS == 1 and strLast and strLast == ">" then
		strResult = string.gsub(strMsg, "c%s*=%s*\"%w+\"", "c=\"" .. strColor .. "\"" )
	else
		strResult = strMsg--StringBuilder.GetFormatString(strMsg, strColor)
	end
	return strResult
	
end

return StringBuilder
