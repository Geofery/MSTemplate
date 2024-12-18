#!/bin/bash

# Function to open a new terminal and run a command
open_new_terminal() {
  osascript <<EOF
tell application "Terminal"
    do script "$1; exit"
end tell
EOF
}

# Start Service1 in a new terminal
echo "Starting Service1..."
open_new_terminal "cd $(pwd)/Service1/Service1.Web && dotnet run --urls http://localhost:5001"

# Start Service2 in a new terminal
echo "Starting Service2..."
open_new_terminal "cd $(pwd)/Service2/Service2.Web && dotnet run --urls http://localhost:5002"

# Start Service3 in a new terminal
echo "Starting Service3..."
open_new_terminal "cd $(pwd)/Service3/Service3.Web && dotnet run --urls http://localhost:5003"

# Wait briefly to ensure services are up
sleep 6

# Open Swagger UI in Safari for Service1
echo "Opening Swagger UI for Service1 in Safari..."
open -a "Safari" "http://localhost:5001/swagger"

# Open Swagger UI in Safari for Service2
echo "Opening Swagger UI for Service2 in Safari..."
open -a "Safari" "http://localhost:5002/swagger"

# Open Swagger UI in Safari for Service3
echo "Opening Swagger UI for Service3 in Safari..."
open -a "Safari" "http://localhost:5003/swagger"

echo "All services started. Terminals are running each service, and Swagger UIs are open in Safari."
#!/bin/bash

echo "Starting Service1..."
dotnet run --project Service1/Service1.Web/Service1.Web.csproj &

echo "Starting Service2..."
dotnet run --project Service2/Service2.Web/Service2.Web.csproj &

echo "Starting Service3..."
dotnet run --project Service3/Service3.Web/Service3.Web.csproj &

wait
echo "All services are running."

