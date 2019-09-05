local tool_screen = {}

dir_tool = require "lib.dir_tool"
----------------------------------------------------------------------------基础遍历工具函数-----------------------------------------------

-- 遍历tabale
function tool_check_table(data, base, name)
    -- dir_tool.PrintTable(table)
    local check_err = {}

    -- 最外一层的table
    for k, v in pairs(data) do
        for a, b in pairs(v) do
            -- 遍历需要检查的字段
            for i, j in pairs(base) do
                -- 相同字段
                if a == j then
                    -- 判断是否为空字符串
                    if b == "" or b == nil then
                        -- print(k .. "的值是空字符串")
                        local str = name .. "配置表中的" .. k .. "的值是空字符串"
                        table.insert(check_err, str)
                    end
                end
            end
        end
    end

    -- 返回检查错误log集合
    return check_err
end

-- 遍历查找空值
function tool_check_number(data, name)
    local check_err = {}

    for k, v in pairs(data) do
        -- 判断值是否是空值
        if v == nil or v == "" then
            local str = "配置表" .. name .. "/" .. k .. "值为空,请检查"
            table.insert(check_err, str)
        elseif type(v) == "table" then
            local data = tool_check_number(v, name)
            for i = 1, #data do
                table.insert(check_err, data[i])
            end
        end
    end

    return check_err
end

----------------------------------------------------------------------------------End------------------------------------------------------------

----------------------------------------------------------------------------------通用检查函数---------------------------------------------------------

-- 通用检测主入口
function tool_screen.check_main(data, name)
    -- 通用需要检查的空值的字符串字段，需要检查空值的字段在这里加（不支持内嵌table）
    local base = {
        "id",
        "name"
    }

    -- 调用遍历函数
    local err_list = tool_check_table(data, base, name)

    -- 调用遍历检查空值函数
    local err_list_number = tool_check_number(data, name)

    -- 拼接
    for i = 1, #err_list_number do
        table.insert(err_list, err_list_number[i])
    end


    if #err_list ~= 0 then
        print("------------------------------------打印检测类----------------------------------------------")
        dir_tool.PrintTable(err_list_number)
    end

    return err_list
end


return tool_screen
