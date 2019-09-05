local lfs = require "lfs"

local dir_help = {}

-- 打印lfs库的版本
function dir_help.PrintLfsVersion( ... )
    print(lfs._VERSION)
end

-- 打印当前目录地址
function dir_help.PrintCurrrentDir( ... )
    local rootPath = lfs.currentdir()
    print(rootPath)
end

-- 获取当前目录
function dir_help.GetCurrrentDir( ... )
    return lfs.currentdir()
end

-- 打印目录的下一层子目录地址
function dir_help.PrintDirChildren(rootPath)
    for entry in lfs.dir(rootPath) do
        if entry~='.' and entry~='..' then
            local path = rootPath.."\\"..entry
            local attr = lfs.attributes(path)
            assert(type(attr)=="table") --如果获取不到属性表则报错
            -- PrintTable(attr)
            if(attr.mode == "directory") then
                print("Dir:",path)
            elseif attr.mode=="file" then
                print("File:",path)
            end
        end
    end
end

-- 获取目录的下一层子目录地址
function dir_help.GetAllFiles(rootPath)
    local allFilePath = {}
    -- print("rootPath = " .. tostring(rootPath))
    local function _GetAllFiles(_path)
        for entry in lfs.dir(_path) do
            if entry~='.' and entry~='..' then
                local path = _path.."\\"..entry
                local attr = lfs.attributes(path)
                
                assert(type(attr)=="table") --如果获取不到属性表则报错
                -- PrintTable(attr)
                if(attr.mode == "directory") then
                    -- print("Dir:",path)
                    _GetAllFiles(path) --自调用遍历子目录
                elseif attr.mode=="file" then
                    -- print(attr.mode,path)
                    table.insert(allFilePath,path)
                end
            end
        end
    end
    _GetAllFiles(rootPath)
    return allFilePath
end

return dir_help
