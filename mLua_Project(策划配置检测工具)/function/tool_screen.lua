local tool_screen = {}

dir_tool = require "lib.dir_tool"
----------------------------------------------------------------------------基础遍历工具函数-----------------------------------------------

-- 遍历tabale
function tool_check_table(data, base,name)
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
                        local str = name .. "配置表中的" ..  k .. "的值是空字符串"
                        table.insert(check_err,str)
                    end
                end
            end
        end
    end

    -- 返回检查错误log集合
    return check_err
end

----------------------------------------------------------------------------------End------------------------------------------------------------

-- 检测是否有该item类型存在
function tool_screen.check_item(id)
end

-- 检查是否有字符串为空值
function tool_screen.check_string(data,name)
    -- 通用需要检查的空值的字符串字段，需要检查空值的字段在这里加（不支持内嵌table）
    local base = {
        "id",
        "name"
    }

    -- 调用遍历函数
    local err_list = tool_check_table(data, base,name)

    return err_list

end

-- 检查是否有int值为空
function tool_screen.check_int(data)
end

----------------------------------------------------------------解析表----------------------------------------------------------------------------------------

-----------------------------------------------------------------以下是给策划针对某一个类型表写的解析检查函数-----------------------------------------------------

-- 检查奖励表中道具是否为空
function tool_screen.check_item_to_reward(item_config, reward_config)
    if not item_config or not reward_config then
        print("奖励配置表异常，没有该配置表")
        return
    end

    print("执行检查奖励表")
end

-- 测试用检查车道具表中是否有该车存在
function tool_screen.check_car_to_item(car_config, item_config)
    if not car_config or not item_config then
        print("车辆配置表异常，没有该配置表")
        return
    end

    print("执行检查车道具表")

    -- 遍历item配置表，检查车辆配置表中的车id是否存在，验证车辆是否合法
end

return tool_screen
