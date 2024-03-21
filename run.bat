set "app_path=MVC_Products"
cd /d "%app_path%"
start "" dotnet run
ping -n 5 127.0.0.1 > nul
start http://localhost:5235

