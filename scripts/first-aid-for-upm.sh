#!/bin/bash

cat package.json | jq -M 'del(.dependencies."@umm/unirx") | del(.dependencies."@umm/zenject") | del(.dependencies."@umm/moq")' | jq -M . > package.json.tmp
mv package.json.tmp package.json
rm -Rf Assets/Modules/umm@{unirx,zenject,moq}

if type gsed > /dev/null 2>&1; then
    find Assets -name "*.asmdef" | xargs gsed -i 's/"umm@unirx"/"UniRx"/'
    find Assets -name "*.asmdef" | xargs gsed -i 's/"umm@unirx-Async"/"UniRx.Async"/'
else
    find Assets -name "*.asmdef" | xargs sed -i '' 's/"umm@unirx"/"UniRx"/g'
    find Assets -name "*.asmdef" | xargs sed -i '' 's/"umm@unirx-Async"/"UniRx.Async"/g'
fi
