setlocal enabledelayedexpansion
set "count=316"
for /f "delims=*" %%f in ('dir /b /o:-d /tc *.xnb') do (
    ren %%f Wall_!count!.xnb
    set /a count+=1
)