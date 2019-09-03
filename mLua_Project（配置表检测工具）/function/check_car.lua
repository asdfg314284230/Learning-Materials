-- 固定写法，不可更改
local M = {}
local C = {}
local L = {}

-- 假设该脚本是检测奖励表是否有误
function M.init()

    local car_config = C["car"]

    -- 防空
    if  car_config then
        -- 调用检查函数
        M.check_data( car_config)
    else
        print("配置表为空,请检查下配置表是否正常，查看下Log文件")
    end
end

-- 举例检查函数
function M.check_data( car_config)


    -- 测试检查最基础的车辆等级是否超过上线
    for k,v in pairs(car_config) do
        for a,b in pairs(v) do
            if a == "lv" then
                if b > 3 then
                    -- 值大于三，异常
                    print(k .. "中的lv值有异常")
                end
            end
        end
    end


end

function M:load()
    -- 外部传入固定工具和配置表
    -- 固定写法
    C = self.C
    L = self.L

    -- 加载初始化完成
    M.init()
end

return M
