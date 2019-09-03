local dir_tool = {}


-- 去除扩展名
function dir_tool.stripextension(filename)
	local idx = filename:match(".+()%.%w+$")
	if(idx) then
		return filename:sub(1, idx-1)
	else
		return filename
	end
end
-- 截取字符串
function dir_tool.getPathFileName( strurl, strchar, bafter)  
  
    local ts = string.reverse(strurl)  
    local param1, param2 = string.find(ts, strchar)  -- 这里以"/"为例  
    local m = string.len(strurl) - param2 + 1     
    local result  
    if (bafter == true) then  
        result = string.sub(strurl, m+1, string.len(strurl))   
    else  
        result = string.sub(strurl, 1, m-1)   
    end  
  
    return result  
end  

-- 返回截取好的路径名称和上层目录
-- 目前只支持双层结构
function dir_tool.get_dir_path(path)

    -- 获取文件名称
    local name = dir_tool.stripextension(string.match(path, ".+\\([^\\]*%.%w+)$"))
    -- 获取中间路径文件夹
    local m_path = string.match(path, "(.+)\\[^\\]*%.%w+$")
    -- 截取下字符串从路径中提出名字
    local file_name = dir_tool.getPathFileName(m_path, "\\", true)
    -- 拼接加载名称
    local load_name = "config." .. file_name .. "." .. name

    return load_name,name
end

-- 打印table
function dir_tool.PrintTable( tbl , level, filteDefault)
    local msg = ""
    filteDefault = filteDefault or true --默认过滤关键字（DeleteMe, _class_type）
    level = level or 1
    local indent_str = ""
    for i = 1, level do
      indent_str = indent_str.."  "
    end
  
    -- print(indent_str .. "{")
    for k,v in pairs(tbl) do
      if filteDefault then
        if k ~= "_class_type" and k ~= "DeleteMe" then
          local item_str = string.format("%s%s = %s", indent_str .. " ",tostring(k), tostring(v))
          print(item_str)
          -- 这里是递归打印更深的配置表table
        --   if type(v) == "table" then
        --     dir_tool.PrintTable(v, level + 1)
        --   end
        end
      else
        local item_str = string.format("%s%s = %s", indent_str .. " ",tostring(k), tostring(v))
        print(item_str)
        -- 这里是递归打印更深的配置表table
        -- if type(v) == "table" then
        --     dir_tool.PrintTable(v, level + 1)
        -- end
      end
    end
    -- print(indent_str .. "}")
  end






return dir_tool
