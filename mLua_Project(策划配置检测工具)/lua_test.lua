package.path = "./lua;" .. package.path

-- dir是打印输出路径，config是配置表文件夹
-- 获取外部传进来的参数
local dir, config_path = ...

print(dir, config_path)

-- 加载工具类库
local dir_help = require "lib.dir_help"
local dir_tool = require "lib.dir_tool"
-- 加载供策划编写的函数类库
local tool_screen = require "function.tool_screen"

-- 加载lib
require "lib.extend.extend"

-- 全配置表数据
local config_List = {}

-- 根据名称添加需要的解析函数
function parse_name_fun(name)
    -- 这里是根据配置表需要的执行什么解析函数在这里添加，对应的具体实现逻辑需要在tool_screen类中实现
    if name == "item" then
        tool_screen.check_item_to_reward(config_List["item"], config_List["reward"])
    elseif name == "car" then
        tool_screen.check_car_to_item(config_List["car"], config_List["item"])
    end
end

-- 根据名称获取解析函数
function parse_data()
    -- 遍历所有配置表列表，查找对应解析函数
    for k, v in pairs(config_List) do
        -- 调用查找对应函数的解析类
        parse_name_fun(k)
    end
end

-- 程序主入口
function prase_main()
    print("读取配置表")

    -- 打印当前目录下的所有子目录
    -- dir_help.PrintDirChildren(config_path)

    -- 获取当前目录下的所有子目录下的配置文件
    local path_table = dir_help.GetAllFiles(config_path)

    -- 获取全部配置表数据
    for k, v in pairs(path_table) do
        -- 获取加载名称和配置表名称
        local load_name, name = dir_tool.get_dir_path(v)

        -- 加载脚本
        local data = require(load_name)

        -- 先走通用验证逻辑
        local err = tool_screen.check_string(data,name)

        if #err > 1 then
            -- 输出到log文件
            local check_err = io.open(dir .. "/log/check.txt", "w")

            for k,v in pairs(err) do
                check_err:write(v .. "\n")
            end
            check_err:close()
        else
            -- 把加载到的数据装载进去
            config_List[name] = data
        end
    end

    dir_tool.PrintTable(config_List)

    -- 执行解析函数
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
