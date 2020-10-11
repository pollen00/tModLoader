setlocal enabledelayedexpansion
set "count=378"
for /f "delims=*" %%f in ('dir /b /o:-d /tc *.xnb') do (
    ren %%f NPC_!count!.xnb
    set /a count+=1
)