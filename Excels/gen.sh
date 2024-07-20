#!/bin/bash
#

#LUBAN_DLL=$WORKSPACE/Tools/Luban/Luban.dll
#CONF_ROOT=.
#
#dotnet $LUBAN_DLL \
#    -t all \
#    -d json \
#    --conf $CONF_ROOT/luban.conf \
#    -x outputDataDir=Assets/Resource/Table/


WORKSPACE=..
# 设置 Luban.dll(mac) 的路径
LUBAN_DLL=$WORKSPACE/Tools/Luban/Luban.dll

# 设置 DataTables 的路径
CONF_ROOT=.

# 设置 代码输出目录 的路径
CODE_OUTPUT=../Assets/Game/Tables/Gen 
# 设置 json数据输出目录 的路径
DATA_OUTPUT=../Assets/Resource/Tables/

TARGET_GROUP=client

CODE_TARGET=cs-simple-json

DATA_TARGET=json

# 运行 Luban
dotnet "$LUBAN_DLL" \
    -t "$TARGET_GROUP" \
    -c "$CODE_TARGET" \
    -d "$DATA_TARGET" \
    --conf "$CONF_ROOT/luban.conf" \
    -x outputCodeDir="$CODE_OUTPUT" \
    -x outputDataDir="$DATA_OUTPUT" \
