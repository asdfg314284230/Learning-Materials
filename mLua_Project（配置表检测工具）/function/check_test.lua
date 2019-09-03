-- 固定写法，不可更改
local M = {}
local C = {}
local L = {}

-- 假设该脚本是检测奖励表是否有误
function M.init()
    local item_config = C["item"]
    local reware_config = C["reward"]

    -- 防空
    if item_config and reware_config then
        -- 调用检查函数
        M.check_data(item_config, reware_config)
    else
        print("配置表为空,请检查下配置表是否正常，查看下Log文件")
    end
end

-- 举例检查函数
function M.check_data(item_config, reware_config)
    -- 需求: 检查reward配置表中的奖励，item表中是否存在该道具
    -- 遍历奖励表

    -- 无简约版本
    -- for k, v in pairs(reware_config) do
    --     -- 获取表中的reward_table
    --     for i, j in pairs(v) do
    --         -- 确保找到的是reward类型
    --         if i == "reward" and type(j) == "table" then
    --             -- 再次遍历reward长度
    --             for a, b in pairs(j) do
    --                 -- 判断是否是道具类型
    --                 if b[1] == "item" then
    --                     -- 是道具类型，遍历道具表，查找是否存在该道具（参数:配置表,要查找的参数）
    --                     local tag = L.get_check_table(item_config, b[2])

    --                     -- 说明没有查找到
    --                     if tag == false then
    --                         -- 打印
    --                         print("该道具在道具表中没有查询到" .. b[2])
    --                         -- 考虑是否需要输出到log文件
    --                     end
    --                 end
    --             end
    --         end
    --     end
    -- end

    -- 简约版本
    -- 获取全部reward列表
    local reward_list = L.get_check_reward(reware_config)
    -- 遍历
    for k, v in pairs(reward_list) do
        if v[1] == "item" then
            -- 调用查找函数，遍历传过去的配置表，给个ID，返回结果
            local tag = L.get_check_table(item_config, v[2])
            -- 说明没有查找到
            if tag == false then
                -- 打印
                print("reward奖励格式中有异常,异常ID:" .. v[2])
                -- 考虑是否需要输出到log文件
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
