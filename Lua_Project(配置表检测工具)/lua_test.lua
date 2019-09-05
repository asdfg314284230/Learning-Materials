package.path = "./lua;" .. package.path
-- 这里也是需要设置绝对路径的
package.path = "D:/Project_Unity/S_producer/config_library/Lua_Project/?.lua;" .. package.path

-- dir是打印输出路径，config是配置表文件夹
-- 获取外部传进来的参数
local dir = ...

--[[
    1.以下两个路径为绝对路径，需要根据电脑的实际路径设置
    2.上面的package.path 也是需要设置路径的
]]

-- 配置表绝对路径(这里可以修改成策划导表完后的执行工具)
local config_path = "D:\\Project_Unity\\S_producer\\config_library\\Lua_Project\\config"
-- 策划解析文件绝对路径
local parse_path = "D:\\Project_Unity\\S_producer\\config_library\\Lua_Project\\function"

-- 加载工具类库
local dir_help = require "lib.dir_help"
local dir_tool = require "lib.dir_tool"
-- 加载供策划编写的函数类库
local tool_screen = require "lib.tool_screen"

-- 加载lib
require "lib.extend.extend"

-- 全配置表数据
local config_List = {}

-- 增加通用输出log日志函数
function out_log(str)
    local path = io.open(dir .. "/log/check.txt", "a")
    path:write(str .. "\n")
    path:close()
end


-- 执行寻找策划所有的lua配置文件并执行
function parse_data()
    local parse_List = {}

    -- 获取当前目录下的所有子目录下的配置文件
    local path_table = dir_help.GetAllFiles(parse_path)
    for k, v in pairs(path_table) do
        -- 获取加载名称和配置表名称
        local load_name, name = dir_tool.get_dir_path("function", v)
        -- 加载脚本
        local data = require(load_name)
        -- 把所有脚本装载进来
        table.insert(parse_List, data)
    end

    -- 组装下需要传递的参数
    local tool_data = {}
    tool_data.C = config_List
    tool_data.L = dir_tool

    -- 遍历集合，传进基础需要的函数
    for k, v in pairs(parse_List) do
        -- 执行固定格式中的init函数并传入配置表数据
        v.load(tool_data)
    end
end

-- 程序主入口
function prase_main()

    -- 获取当前目录下的所有子目录下的配置文件
    local path_table = dir_help.GetAllFiles(config_path)

    -- 获取全部配置表数据
    for k, v in pairs(path_table) do
        -- 获取加载名称和配置表名称
        local load_name, name = dir_tool.get_dir_path("config", v)

        -- 加载脚本
        local data = require(load_name)

        -- 先走通用验证逻辑
        local err = tool_screen.check_main(data, name)

        if #err ~= 0 then
            -- 输出到log文件
            local check_err = io.open(dir .. "/log/check.txt", "w")
            print("------------------------------------配置表出现异常----------------------------------------------")
            print(name .. "配置表通用检测异常，详情请查看Log文件")
            for k, v in pairs(err) do
                check_err:write(v .. "\n")
            end
            check_err:close()
        else
            -- 把加载到的数据装载进去
            config_List[name] = data
        end
    end

    -- 执行策划那边写好的lua检测文件
    parse_data()
end

local show, err =
    pcall(
    function()
        prase_main()
    end
)

-- 处理报错异常
if err then
    print("报错异常")
    print(err)
    -- 输出到err.log文件
    local log_err = io.open(dir .. "/log/err.txt", "w")
    log_err:write(err .. "\n")
    log_err:close()
end
